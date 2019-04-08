using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using NuGet;

namespace ProjectAdj
{
    internal class PrjAdj
    {
        private SolutionAdj SolutionAdj { get; }

        private ILog Logger { get; } = LogManager.GetLogger(typeof(SolutionAdj));

        //private ProjectWithCompilation ProjectWithCompilation => ProjectWithCompilationHistory.CurrentProjectWithCompilation;
        private Project Project => ProjectWithCompilationHistory.CurrentProject;
        private Compilation Compilation => ProjectWithCompilationHistory.CurrentCompilation;

        private MSBuildWorkspace Workspace { get; }
        private Solution Solution => ProjectWithCompilationHistory.CurrentSolution;

        private IList<CompareTypeResult> ListTypeMap => SolutionAdj.ListTypeMap;
        private IList<CompareMethodResult> ListMethodMap => SolutionAdj.ListMethodMap;

        private string SolutionFilePath => SolutionAdj.SolutionFilePath;
        private string SolutionFolderPath => Path.GetDirectoryName(SolutionAdj.SolutionFilePath);
        private string ProjectFolderPath => Path.GetDirectoryName(Project.FilePath);
        private string MapFilePath => SolutionAdj.MapFilePath;
        private string NugetApiEndpoint => SolutionAdj.NugetApiEndpoint;

        private IList<INamedTypeSymbol> _usedTypes;
        private ISet<string> _removedDlls = new HashSet<string>();

        private ProjectWithCompilationHistory ProjectWithCompilationHistory { get; }

        public PrjAdj(MSBuildWorkspace workspace, SolutionAdj solutionAdj, ProjectWithCompilation projectWithCompilation)
        {
            Workspace = workspace;
            SolutionAdj = solutionAdj;
            ProjectWithCompilationHistory = new ProjectWithCompilationHistory(projectWithCompilation);
        }

        public async Task<Solution> Adjust()
        {
            await LoadUsedTypes().ConfigureAwait(false);
            await ReplaceRenamedStaticMethodReferences().ConfigureAwait(false);
            await ReplaceRenamedTypeReferences().ConfigureAwait(false);
            ReplaceUsings();
            await ReplaceNugetPackages().ConfigureAwait(false);

            return Solution;
        }

        private async Task LoadUsedTypes()
        {
            var allUsedTypes = await
                Compilation
                .GetAllUsedTypesInCompilation()
                .ConfigureAwait(false);

            _usedTypes = allUsedTypes.ExcludeSystemTypes();
        }

        private IList<CompareTypeResult> GetMissingNugets()
        {
            OutputKind outputKind = DetermineProjectOutputKind();
            if (outputKind == OutputKind.DynamicallyLinkedLibrary || outputKind == OutputKind.NetModule)
            {
                return GetMissingNugetsForDlls();
            }
            else
            {
                return GetMissingNugetsForStartingApp();
            }
        }

        private static string[] TestDlls =
                new string[]
                {
                    "microsoft.visualstudio.testplatform.testframework",
                    "xunit.runner.visualstudio"
                };

        private OutputKind DetermineProjectOutputKind()
        {
            OutputKind outputKind = Project.CompilationOptions.OutputKind;
            if (outputKind == OutputKind.DynamicallyLinkedLibrary)
            {
                // do the best to determine if it's a MSTest or XUnit project
                var references =
                    Project
                    .MetadataReferences
                    .Select(x => Path.GetFileNameWithoutExtension(x.Display).ToLower());

                bool isTest =
                    references
                    .Join
                    (
                        TestDlls,
                        x => x,
                        x => x,
                        (x, y) => x
                    )
                    .Any();

                if (isTest)
                {
                    return OutputKind.ConsoleApplication;
                }
            }
            return outputKind;
        }

        private IList<CompareTypeResult> GetMissingNugetsForDlls()
        {
            return
                ListTypeMap
                .Join
                (
                    _usedTypes,
                    x => $"{x.OriginalNamespace}.{x.OriginalType}@{x.OriginalAssemblyName}",
                    x => $"{x.ContainingNamespace.ToString()}.{x.Name}@{x.ContainingAssembly.Name}",
                    (x, y) => x
                )
                .GroupBy(x => x.NewAssemblyName)
                .Select(x => x.First())
                .Where(x => !string.IsNullOrEmpty(x.NewAssemblyName))
                .ToList();
        }

        private IList<CompareTypeResult> GetMissingNugetsForStartingApp()
        {
            var allExternalAssemblies =
                Project
                .MetadataReferences
                .Select(x => Path.GetFileNameWithoutExtension(x.Display))
                .ToArray();


            return
                ListTypeMap
                .Join
                (
                    allExternalAssemblies,
                    x => $"{x.OriginalAssemblyName}",
                    x => x,
                    (x, y) => x
                )
                .Where(x => !string.IsNullOrEmpty(x.NewAssemblyName))
                .GroupBy(x => x.NewAssemblyName)
                .Select(x => x.First())
                .ToList();
        }

        private async Task ReplaceNugetPackages()
        {
            CompareTypeResult[] missingNugetPackages = GetMissingNugets().ToArray();
            string rootPath = SolutionFolderPath;
            string packageInstallPath = Path.Combine(rootPath, "packages");

            foreach (var comparation in missingNugetPackages)
            {
                try
                {
                    await InstallNugetPackage(comparation).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }
        }

        private async Task InstallNugetPackage(CompareTypeResult comparation)
        {
            string rootPath = SolutionFolderPath;
            string packageInstallPath = Path.Combine(rootPath, "packages");

            string packageName = comparation.NewAssemblyName;
            CacheResult cacheResult = await SolutionAdj.GetNugetPackageV2(packageName, packageInstallPath).ConfigureAwait(false);
            if (cacheResult != null)
            {
                var packageDownloaded = cacheResult.Package;
                if (packageDownloaded == null)
                {
                    throw new Exception($"Could not download package '{packageName}'");
                }

                UpdatePackagesConfigFile(Path.Combine(ProjectFolderPath, "packages.config"), packageDownloaded, comparation.OriginalAssemblyName);
                RemoveReferenceFromProject($"{comparation.OriginalAssemblyName}");
                AddReferenceToProject(packageInstallPath, packageDownloaded);
            }
            else
            {
                Logger.Warn($"CacheResult is null while downloading '{packageName}'");
            }
        }

        private void UpdatePackagesConfigFile(string configPath, IPackage package, string originalPackageName)
        {
            Logger.Debug($"Adding {package.Id} to 'packages.config'");

            var packageReferenceFile = new PackageReferenceFile(configPath);

            // Get the target framework of the current project to add --> targetframework="net452" attribute in the package.config file

            //var currentTargetFw = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
            //var targetFrameworkAttribute = ((TargetFrameworkAttribute[])currentTargetFw).FirstOrDefault();

            Logger.Debug($"Removing '{originalPackageName}' from 'packages.config'");
            packageReferenceFile.DeleteEntry(originalPackageName, null);

            packageReferenceFile.AddEntry(package.Id, SemanticVersion.Parse(package.Version.ToFullString()), false, new FrameworkName(".NETFramework,Version=v4.6.1"));

            Logger.Debug($"'packages.config' updated");
        }

        private void RemoveReferenceFromProject(string referenceName)
        {
            if (_removedDlls.Contains(referenceName))
            {
                return;
            }

            var metadataReference = Project.MetadataReferences.FirstOrDefault(x => x.Display.EndsWith($"{referenceName}.dll"));
            if (metadataReference == null)
            {
                throw new Exception($"No dll reference named '{referenceName}' found in project '{Project.Name}'");
            }
            var modifiedProject = Project.RemoveMetadataReference(metadataReference);
            ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuildRemoveReferenceKey(referenceName), modifiedProject);
            _removedDlls.Add(referenceName);
        }

        private void AddReferenceToProject(string packageInstallPath, IPackage packageDownloaded)
        {
            string path = Path.Combine(packageInstallPath, $"{packageDownloaded.Id}.{packageDownloaded.Version}");
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            string assemblyToAdd = files.FirstOrDefault(f => Path.GetFileName(f) == $"{packageDownloaded.Id}.dll");
            if (!string.IsNullOrEmpty(assemblyToAdd))
            {
                MetadataReference reference = MetadataReference.CreateFromFile(assemblyToAdd);
                if (reference != null)
                {
                    var projModified = Project.AddMetadataReference(reference);
                    ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuildAddReferenceKey(packageDownloaded.Id), projModified);
                }

                Logger.Debug($"{packageDownloaded.Id}.dll successfully added as reference to project {Project.Name}");
            }
            else
            {
                throw new Exception($"Could not add nuget {packageDownloaded.Id}.dll to project {Project.Name}");
            }
        }

        private async Task ReplaceRenamedStaticMethodReferences()
        {
            if (ListMethodMap != null)
            {
                Logger.Debug($"Start replacing static methods...");

                var docIds = GetDocumentsIdsForProject(Project);

                var collapsedData =
                    ListMethodMap
                    .Where(x => x.IsChanged())
                    .GroupBy(x => x.OriginalMethodName)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var docId in docIds)
                {
                    Document doc = Project.GetDocument(docId);
                    Logger.Debug($"Processing source file: {doc.FilePath}...");
                    SyntaxTree syntaxTree = await doc.GetSyntaxTreeAsync().ConfigureAwait(false);
                    var root = syntaxTree.GetRoot();
                    var rewriter = new StaticMethodInvocationRewriter(collapsedData);
                    var newnode = rewriter.Visit(root);
                    var newSolution = Solution.WithDocumentSyntaxRoot(docId, newnode);
                    var newProject = newSolution.GetProject(Project.Id);
                    ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuilReplaceStaticMethodsKey(doc.Name), newProject);
                }
            }
        }


        private async Task ReplaceRenamedTypeReferences()
        {
            Logger.Debug($" Start replacing renamed type usage...");

            var docIds = GetDocumentsIdsForProject(Project);

            var collapsedData =
                ListTypeMap
                .Where(x => x.IsChanged())
                .GroupBy(x => x.OriginalType)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var docId in docIds)
            {
                Document doc = Project.GetDocument(docId);
                Logger.Debug($"Processing source file: {doc.FilePath}...");
                SyntaxTree syntaxTree = await doc.GetSyntaxTreeAsync().ConfigureAwait(false);
                var root = syntaxTree.GetRoot();
                var rewriter = new OriginalTypesRewriter(doc.Name, collapsedData);
                var newnode = rewriter.Visit(root);
                var newSolution = Solution.WithDocumentSyntaxRoot(docId, newnode);
                var newProject = newSolution.GetProject(Project.Id);
                ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuilReplaceTypesKey(doc.Name), newProject);
            }
        }

        //private async Task ReplaceRenamedTypeReferences()
        //{
        //    Logger.Debug($" Start replacing renamed type usage...");

        //    foreach (INamedTypeSymbol ntSymbol in _usedTypes)
        //    {
        //        var typesToReplace =
        //            ListTypeMap
        //            .Where
        //            (
        //                c =>
        //                    c.IsChanged()
        //                    && c.OriginalType == ntSymbol.Name
        //                    && c.OriginalNamespace == ntSymbol.ContainingNamespace.ToString()
        //                    && c.OriginalAssemblyName == ntSymbol.ContainingAssembly.Name
        //            );

        //        foreach (CompareTypeResult comparation in typesToReplace)
        //        {
        //            try
        //            {
        //                Compilation compilation = await Project.GetCompilationAsync().ConfigureAwait(false);

        //                //var classTypeOriginal = compilation.GetTypeByMetadataName($"{comparation.OriginalNamespace}.{comparation.OriginalType}");
        //                var xxx = compilation.GetAllSymbols().Where(x => x != null && x.Name == comparation.OriginalType).ToArray();
        //                var classTypeOriginal = compilation.GetSymbolsWithName(ntSymbol.Name).FirstOrDefault();
        //                //var classTypeOriginal  = ProjectWithCompilationHistory.History.First().ProjectWithCompilation.Compilation.GetTypeByMetadataName($"{comparation.OriginalNamespace}.{comparation.OriginalType}");
        //                if (classTypeOriginal == null)
        //                {
        //                    continue;
        //                }

        //                var newSolution = await
        //                    Renamer
        //                    .RenameSymbolAsync(Solution, classTypeOriginal, comparation.NewType, Workspace.Options)
        //                    .ConfigureAwait(false);

        //                var newProject = newSolution.GetProject(Project.Id);

        //                ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuilReplaceTypesKey(Project.Name), newProject);
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Error(ex.Message, ex);
        //                throw;
        //            }
        //        }
        //    }
        //}

        private void ReplaceUsings()
        {
            Logger.Debug($"Adjusting using directives to renamed types...");

            var docIds = GetDocumentsIdsForProject(Project);

            IList<CompareTypeResult> newUsedDlls = GetMissingNugetsForDlls();

            var groupedNamespaces =
                ListTypeMap
                .Join
                (
                    newUsedDlls,
                    x => x.NewAssemblyName,
                    x => x.NewAssemblyName,
                    (x, y) => x
                )
                .GroupBy(x => x.OriginalNamespace)
                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var docId in docIds)
            {
                var doc = Project.GetDocument(docId);

                Logger.Debug($"Processing source file: {doc.FilePath}");
                var syntaxTree = doc.GetSyntaxTreeAsync().Result;
                var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

                var usingData =
                    groupedNamespaces
                    .Join
                    (
                        root.Usings,
                        x => x.Key,
                        x => x.Name.ToString(),
                        (x, y) =>
                            new
                            {
                                OriginalUsing = y,
                                NewUsings =
                                    SyntaxFactory
                                    .List
                                    (
                                        x
                                        .Value
                                        .Select
                                        (
                                            z => y.WithName(SyntaxFactory.ParseName(z.NewNamespace))
                                        )
                                    //.Concat(new[] {y} )
                                    )
                            }
                    );

                if (usingData.Any())
                {
                    var newUsings =
                        usingData
                        .SelectMany(x => x.NewUsings);

                    var oldUsingsToRemove =
                        usingData
                        .Select(x => x.OriginalUsing);

                    var remainingUsings =
                        root
                        .Usings
                        .Except
                        (
                            oldUsingsToRemove,
                            EqualityComparer<UsingDirectiveSyntax>.Default
                        )
                        .Concat(newUsings)
                        .GroupBy(x => x.Name.ToString())
                        .Select(x => x.First())
                        .OrderBy(x => x.Name.ToString());

                    root = root.WithUsings(SyntaxFactory.List(remainingUsings));

                    var newSolution = Solution.WithDocumentSyntaxRoot(docId, root);
                    var newProject = newSolution.GetProject(Project.Id);

                    ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuilReplaceUsingsKey(doc.Name), newProject);
                }
            }
        }

        private static IEnumerable<DocumentId> GetDocumentsIdsForProject(Project project)
        {
            return
                project
                .Documents
                .Where
                (
                    d =>
                        d.SourceCodeKind == SourceCodeKind.Regular
                        && d.SupportsSyntaxTree
                        && d.FilePath.StartsWith(Path.GetDirectoryName(d.Project.FilePath))
                )
                .Select(x => x.Id);
        }
    }
}
