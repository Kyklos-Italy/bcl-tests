using Common.Logging;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Newtonsoft.Json;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Xml;
using static Kyklos.Kernel.FSharp.KReactive;

namespace ProjectAdj
{
    public class ProjectMapper : IDisposable
    {
        private string _sourceProjectFilePath;
        private string _destinationFolder;
        private string _mapFilePath;
        private MSBuildWorkspace _workspace;
        private Solution _solution;
        private Project _project;

        private List<CompareTypeResult> _listTypeMap;
        private List<CompareMethodResult> _listMethodMap;

        private class CacheResult
        {
            public IPackage Package { get; }
            public bool AlreadyInstalled { get; set; }

            public CacheResult(IPackage package, bool alreadyInstalled)
            {
                Package = package;
                AlreadyInstalled = alreadyInstalled;
            }
        }

        private KCache<string, CacheResult> _cache;

        ILog Logger = LogManager.GetLogger(typeof(ProjectMapper));

        public ProjectMapper(string sourceProjectFilePath, string mapFilePath)
        {
            _sourceProjectFilePath = sourceProjectFilePath;
            _destinationFolder = Path.GetDirectoryName(sourceProjectFilePath);
            _mapFilePath = mapFilePath;

            int timeToLiveInMillis = 100000;
            _cache = new KCache<string, CacheResult>(timeToLiveInMillis, true);

            LoadTypeMap();

            LoadWorkspace();
        }

        private void LoadWorkspace()
        {
            MSBuildLocator.RegisterDefaults();
            _workspace = MSBuildWorkspace.Create();
            _workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
        }

        public async Task MapProjectAsync(string nugetApiEndpoint)
        {
            try
            {
                // DirectoryInfo destFolder = Directory.CreateDirectory(_destinationFolder);                
                string sourceDir = Path.GetDirectoryName(_sourceProjectFilePath);
                //CopyFolder(sourceDir, _destinationFolder);
                string destProjPath = Path.Combine(_destinationFolder, Path.GetFileName(_sourceProjectFilePath));

                _project = await
                    _workspace
                    .OpenProjectAsync(destProjPath)
                    .ConfigureAwait(false);

                _solution = _project.Solution;

                var replacedMethods = await ReplaceRenamedStaticMethodReferences(destProjPath).ConfigureAwait(false);
                var replacedTypes = await ReplaceRenamedTypeReferences(destProjPath).ConfigureAwait(false);

                var affectedAssemplies =
                    replacedTypes
                    .Union
                    (
                        replacedMethods
                    );


                await ReplaceNugetPackages(destProjPath, nugetApiEndpoint, affectedAssemplies).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogFullExceptionTree(ex);
            }
        }

        private void LogFullExceptionTree(Exception ex)
        {
            Logger.Error(ex.Message, ex);
            //Exception innerException = ex.InnerException;
            //while (innerException != null)
            //{
            //    Logger.Error(innerException.Message);
            //    innerException = innerException.InnerException;
            //}
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Logger.Error(e.Diagnostic.Message);
        }

        private void CheckInput()
        {
            if (!File.Exists(_sourceProjectFilePath))
                throw new Exception($"Source project file {_sourceProjectFilePath} not found!");

            if (!File.Exists(_mapFilePath))
                throw new Exception($"Mapping file {_mapFilePath} not found!");
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            foreach (string dir in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(destFolder, dir.Substring(sourceFolder.Length + 1)));
            }

            foreach (string file_name in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
            {
                File.Copy(file_name, Path.Combine(destFolder, file_name.Substring(sourceFolder.Length + 1)), true);
            }
        }

        private void LoadTypeMap()
        {
            var paths = File.ReadAllLines(_mapFilePath);
            if (paths.Any())
            {
                string typeMapJson = paths.First();
                _listTypeMap = JsonConvert.DeserializeObject<List<CompareTypeResult>>(typeMapJson);
                if (paths.Count() > 1)
                {
                    string methodMapJson = paths[1];
                    _listMethodMap = JsonConvert.DeserializeObject<List<CompareMethodResult>>(methodMapJson);
                }
            }
        }

        private async Task<IList<CompareTypeResult>> ReplaceRenamedTypeReferences(string destProjPath)
        {
            List<CompareTypeResult> replacedTypes = new List<CompareTypeResult>();

            Logger.Debug($"Replacing renamed type usage...");

            Compilation compilation = await _project.GetCompilationAsync().ConfigureAwait(false);

            IList<INamedTypeSymbol> usedTypes = (await 
                compilation
                .GetAllUsedTypesInCompilation().ConfigureAwait(false))
                .ExcludeSystemTypes();



            foreach (CompareTypeResult comparation in _listTypeMap.Where(c => c.IsChanged()))
            {
                try
                {
                    //SymbolFinder.FindDeclarationsAsync(_project)

                    var allSymbols = 
                        compilation
                        .GetAllSymbols()
                        .Where(x => x?.DeclaringSyntaxReferences != null && x.DeclaringSyntaxReferences.Any())
                        .ToArray();

                    //bool exists = compilation.ContainsSymbolsWithName(x => x.Contains(comparation.OriginalType), SymbolFilter.All);
                    //compilation.GetSemanticModel(compilation.SyntaxTrees.First()).
                    var classTypeOriginal = compilation.GetTypeByMetadataName($"{comparation.OriginalNamespace}.{comparation.OriginalType}");
                    if (classTypeOriginal != null && classTypeOriginal.DeclaringSyntaxReferences.Any())
                    {
                        replacedTypes.Add(comparation);
                        _solution = await Renamer.RenameSymbolAsync(_solution, classTypeOriginal, comparation.NewType, _solution.Options).ConfigureAwait(false);
                        var result = _workspace.TryApplyChanges(_solution);
                    }

                }
                catch (Exception ex)
                {
                    LogFullExceptionTree(ex);
                }
            }

            Logger.Debug($"Adjusting using directives to renamed types...");
            //using (var workspace = MSBuildWorkspace.Create())
            {
                //workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                //var projectDestination = workspace.OpenProjectAsync(destProjPath).Result;
                //Solution solution = projectDestination.Solution;

                var docs =
                    _project
                    .Documents
                    .Where
                    (
                        d =>
                            d.SourceCodeKind == SourceCodeKind.Regular
                            && d.SupportsSyntaxTree
                            && d.FilePath.StartsWith(Path.GetDirectoryName(d.Project.FilePath))
                    );

                foreach (var doc in docs)
                {
                    Logger.Debug($"Processing source file: {doc.FilePath}");
                    var syntaxTree = doc.GetSyntaxTreeAsync().Result;
                    var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
                    foreach (var comparation in _listTypeMap.Where(c => c.IsChanged()))
                    {
                        bool bRootChanged = false;
                        for (int i = 0; i < root.Usings.Count; i++)
                        {
                            var usng = root.Usings[i];
                            if (usng.Name.ToString() == comparation.OriginalNamespace)
                            {
                                var newusng = usng.WithName(SyntaxFactory.ParseName(comparation.NewNamespace));
                                root = root.ReplaceNode(usng, newusng);
                                bRootChanged = true;
                                break;
                            }
                        }

                        if (!bRootChanged)
                        {
                            _solution = _solution.WithDocumentSyntaxRoot(doc.Id, root);
                        }
                    }
                }
                var result = _workspace.TryApplyChanges(_solution);
            }

            return replacedTypes;
        }

        private async Task<IList<CompareMethodResult>> ReplaceRenamedStaticMethodReferences(string destProjPath)
        {
            List<CompareMethodResult> replacedMethods = new List<CompareMethodResult>();

            if (_listMethodMap != null)
            {
                Logger.Debug($"Replace static methods");

                var docs =
                    _project
                    .Documents
                    .Where
                    (
                        d =>
                            d.SourceCodeKind == SourceCodeKind.Regular
                            && d.SupportsSyntaxTree
                            && d.FilePath.StartsWith(Path.GetDirectoryName(d.Project.FilePath))
                    );

                var collapsedData =
                    _listMethodMap
                    .Where(x => x.IsChanged())
                    .GroupBy(x => x.OriginalMethodName)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var doc in docs)
                {
                    Logger.Debug($"Processing source file: {doc.FilePath}...");
                    SyntaxTree syntaxTree = await doc.GetSyntaxTreeAsync().ConfigureAwait(false);
                    var root = syntaxTree.GetRoot();
                    var rewriter = new StaticMethodInvocationRewriter(collapsedData);
                    var newnode = rewriter.Visit(root);
                    replacedMethods.AddRange(rewriter.ReplacedMethods);
                    _solution = _solution.WithDocumentSyntaxRoot(doc.Id, newnode);
                }

                var result = _workspace.TryApplyChanges(_solution);
            }
            return replacedMethods;
        }

        private async Task ReplaceNugetPackages(string destProjPath, string repositoryApiEndpoint, IEnumerable<CompareTypeResult> affectedAssemplies)
        {
            string rootPath = GetAbsolutePathFromRelativePath(Path.GetDirectoryName(destProjPath));
            string packageInstallPath = GetAbsolutePathFromRelativePath($"{rootPath}\\packages");

            //foreach (var comparation in _listMethodMap.Where(c => c.IsChanged()))
            foreach (var comparation in affectedAssemplies)
            {
                try
                {
                    if (comparation.OriginalAssemblyName != comparation.NewAssemblyName)
                    {
                        string packageName = comparation.NewAssemblyName;
                        CacheResult cacheResult = await GetNugetPackageV2(packageName, repositoryApiEndpoint, packageInstallPath).ConfigureAwait(false);
                        if (cacheResult != null && !cacheResult.AlreadyInstalled)
                        {
                            var packageDownloaded = cacheResult.Package;
                            UpdatePackagesConfigFile($"{rootPath}\\packages.config", packageDownloaded, comparation.OriginalAssemblyName);
                            RemoveReferenceFromProject(destProjPath, $"{comparation.OriginalAssemblyName}");
                            AddReferenceToProject(destProjPath, packageInstallPath, packageDownloaded);
                            cacheResult.AlreadyInstalled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogFullExceptionTree(ex);
                }
            }
        }

        private async Task<CacheResult> GetNugetPackageV2(string packageName, string repositoryApiEndpointUrl, string packageInstallPath)
        {
            var getAddRes = await _cache
                .GetOrAddWithFactoryTask
                (
                    packageName,
                    key =>
                    {
                        IPackage package = DownloadNugetPackageV2(key, repositoryApiEndpointUrl, packageInstallPath);
                        return new CacheResult(package, false);
                    }
                )
                .ConfigureAwait(false);

            return getAddRes.Result.Value;
        }

        private IPackage DownloadNugetPackageV2(string packageName, string repositoryApiEndpointUrl, string packageInstallPath)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryApiEndpointUrl);

            List<IPackage> packages =
                repo
                .FindPackagesById(packageName)
                .OrderByDescending(x => x.Version)
                .ToList();

            var nugetFound = packages.FirstOrDefault();
            if (nugetFound != null)
            {
                Logger.Debug($"Downloading {nugetFound.GetFullName()} package...");

                PackageManager packageManager = new PackageManager(repo, packageInstallPath);

                packageManager.InstallPackage(packageName, nugetFound.Version, false, true);

                Logger.Debug($"{nugetFound.Id} nuget package downloaded");
            }

            return nugetFound;
        }

        private void UpdatePackagesConfigFile(string configPath, IPackage package, string originalPackageName)
        {
            Logger.Debug($"Adding {package.Id} to 'packages.config'");

            var packageReferenceFile = new PackageReferenceFile(configPath);

            // Get the target framework of the current project to add --> targetframework="net452" attribute in the package.config file
            var currentTargetFw = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
            var targetFrameworkAttribute = ((TargetFrameworkAttribute[])currentTargetFw).FirstOrDefault();
            packageReferenceFile.AddEntry(package.Id, SemanticVersion.Parse(package.Version.ToFullString()), false, new FrameworkName(targetFrameworkAttribute.FrameworkName));

            Logger.Debug($"Removing {originalPackageName} from 'packages.config'");
            packageReferenceFile.DeleteEntry(originalPackageName, null);

            Logger.Debug($"'packages.config' updated");
        }

        private void RemoveReferenceFromProject(string projectFilePath, string referenceName)
        {
            var projectFileXmlDoc = new XmlDocument();
            projectFileXmlDoc.Load(projectFilePath);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(projectFileXmlDoc.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
            var referenceNodes = projectFileXmlDoc.SelectNodes("//x:Project/x:ItemGroup/x:Reference", nsmgr);
            foreach (XmlNode node in referenceNodes)
            {
                if (node.Attributes["Include"].Value.Contains(referenceName))
                    node.ParentNode.RemoveChild(node);
            }
            projectFileXmlDoc.Save(projectFilePath);
        }

        private void AddReferenceToProject(string projectPath, string packageInstallPath, IPackage packageDownloaded)
        {
            string path = $"{packageInstallPath}\\{packageDownloaded.Id}.{packageDownloaded.Version}";
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            string assemblyToAdd = files.FirstOrDefault(f => Path.GetFileName(f) == $"{packageDownloaded.Id}.dll");
            if (!string.IsNullOrEmpty(assemblyToAdd))
            {
                //using (var workspace = MSBuildWorkspace.Create())
                {
                    //workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                    MetadataReference reference = MetadataReference.CreateFromFile(assemblyToAdd);
                    if (reference != null)
                    {
                        var projModified = _project.AddMetadataReference(reference);
                        var res = _workspace.TryApplyChanges(projModified.Solution);
                    }
                }
                Logger.Debug($"{packageDownloaded.Id}.dll successfully added as reference to {Path.GetFileNameWithoutExtension(projectPath)} project");
            }
            else
            {
                Logger.Error($"{packageDownloaded.Id}.dll was not found!");
            }
        }

        private string GetAbsolutePathFromRelativePath(string relativePath)
        {
            string absolutePath = relativePath;
            if (!(!string.IsNullOrEmpty(relativePath) && (relativePath.StartsWith("\\") || relativePath.IndexOf(':') == 1)))
            {
                string assemblyFolder = Assembly.GetExecutingAssembly().Location;
                FileInfo fiAssemblyFolder = new FileInfo(assemblyFolder);
                absolutePath = Path.Combine(fiAssemblyFolder.Directory.FullName, relativePath);
            }
            return absolutePath;
        }

        public void Dispose()
        {
            _workspace?.Dispose();
        }
    }
}
