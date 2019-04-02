using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace ProjectAdj
{
    internal class PrjAdj
    {
        private SolutionAdj SolutionAdj { get; }
        public Project Project { get; }
        public Compilation Compilation { get; }

        private MSBuildWorkspace Workspace => SolutionAdj.Workspace;
        private Solution Solution => SolutionAdj.Solution;
        private IList<CompareTypeResult> ListTypeMap => SolutionAdj.ListTypeMap;
        private IList<CompareMethodResult> ListMethodMap => SolutionAdj.ListMethodMap;
        private string SolutionFilePath => SolutionAdj.SolutionFilePath;
        private string SolutionFolderPath => Path.GetDirectoryName(SolutionAdj.SolutionFilePath);
        private string MapFilePath => SolutionAdj.MapFilePath;
        private string NugetApiEndpoint => SolutionAdj.NugetApiEndpoint;


        private IList<INamedTypeSymbol> _usedTypes;

        public PrjAdj(SolutionAdj solutionAdj, Project project, Compilation compilation)
        {
            SolutionAdj = solutionAdj;
            Project = project;
            Compilation = compilation;
        }

        public async Task AddNugetPackages()
        {
            await LoadUsedTypes().ConfigureAwait(false);

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

        private async Task ReplaceNugetPackages(IEnumerable<CompareTypeResult> affectedAssemplies)
        {
            string rootPath = SolutionFolderPath;
            string packageInstallPath = Path.Combine(rootPath, "packages");

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

    }
}
