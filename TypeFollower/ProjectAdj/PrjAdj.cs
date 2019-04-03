using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NuGet;

namespace ProjectAdj
{
    internal class PrjAdj
    {
        private SolutionAdj SolutionAdj { get; }

        private ILog Logger { get; } = LogManager.GetLogger(typeof(SolutionAdj));

        private ProjectWithCompilation ProjectWithCompilation => ProjectWithCompilationHistory.CurrentProjectWithCompilation;
        private Project Project => ProjectWithCompilationHistory.CurrentProject;
        private Compilation Compilation => ProjectWithCompilationHistory.CurrentCompilation;

        private MSBuildWorkspace Workspace { get; }
        private Solution Solution => ProjectWithCompilationHistory.CurrentProjectWithCompilation.Solution;

        private IList<CompareTypeResult> ListTypeMap => SolutionAdj.ListTypeMap;
        private IList<CompareMethodResult> ListMethodMap => SolutionAdj.ListMethodMap;

        private string SolutionFilePath => SolutionAdj.SolutionFilePath;
        private string SolutionFolderPath => Path.GetDirectoryName(SolutionAdj.SolutionFilePath);
        private string ProjectFolderPath => Path.GetDirectoryName(Project.FilePath);
        private string MapFilePath => SolutionAdj.MapFilePath;
        private string NugetApiEndpoint => SolutionAdj.NugetApiEndpoint;

        private IList<INamedTypeSymbol> _usedTypes;
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

        private IList<CompareTypeResult> DetermineMissingNugets()
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
                .ToList();
        }

        private async Task ReplaceNugetPackages()
        {
            IEnumerable<CompareTypeResult> missingNugetPackages = DetermineMissingNugets();
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
            if (cacheResult != null && !cacheResult.AlreadyInstalled)
            {
                var packageDownloaded = cacheResult.Package;
                if (packageDownloaded == null)
                {
                    throw new Exception($"Could not download package {packageName}");
                }

                UpdatePackagesConfigFile(Path.Combine(ProjectFolderPath, "packages.config"), packageDownloaded, comparation.OriginalAssemblyName);
                RemoveReferenceFromProject($"{comparation.OriginalAssemblyName}");
                AddReferenceToProject(packageInstallPath, packageDownloaded);
                cacheResult.AlreadyInstalled = true;
            }
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

        private void RemoveReferenceFromProject(string referenceName)
        {
            var metadataReference = Project.MetadataReferences.FirstOrDefault(x => x.Display.EndsWith($"{referenceName}.dll"));
            if (metadataReference == null)
            {
                throw new Exception($"No dll reference named {referenceName} found in project {Project.Name}");
            }
            var modifiedProject = Project.RemoveMetadataReference(metadataReference);
            ProjectWithCompilationHistory.AddProjectToHistory(ProjectWithCompilationKey.BuildRemoveReferenceKey(referenceName), modifiedProject);
            //bool applied = Workspace.TryApplyChanges(Solution);
            //if (!applied)
            //{
            //    throw new Exception($"Could not apply changes to project {Project.Name} after removing dll {referenceName}");
            //}
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

    }
}
