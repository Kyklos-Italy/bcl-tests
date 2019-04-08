using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;
using NuGet;
using static Kyklos.Kernel.FSharp.KReactive;

namespace ProjectAdj
{
    public class SolutionAdj : IDisposable
    {
        //private enum ProcessingMode
        //{
        //    BySolution,
        //    BySingleProject
        //}

        public string SolutionFilePath { get; }
        public string MapFilePath { get; }
        public string NugetApiEndpoint { get; }

        private ILog Logger { get; } = LogManager.GetLogger(typeof(SolutionAdj));

        public MSBuildWorkspace Workspace { get; private set; }
        public Solution Solution { get; private set; }

        public IList<CompareTypeResult> ListTypeMap { get; private set; }
        public IList<CompareMethodResult> ListMethodMap { get; private set; }
//        private Dictionary<string, ProjectWithCompilation> _compilations;
        private KCache<string, CacheResult> _cache;

        private Dictionary<string, CacheResult> _dictCache;
        private IPackageRepository _nugetRepo;
       // private ProcessingMode _processingMode;

        public SolutionAdj(string solutionFilePath, string mapFilePath, string nugetApiEndpoint)
        {
            SolutionFilePath = solutionFilePath;
            MapFilePath = mapFilePath;
            NugetApiEndpoint = nugetApiEndpoint;

            int timeToLiveInSeconds = 3600;
            _cache = new KCache<string, CacheResult>(timeToLiveInSeconds * 1000, true);
            _nugetRepo = PackageRepositoryFactory.Default.CreateRepository(nugetApiEndpoint);
            _dictCache = new Dictionary<string, CacheResult>();
            //_compilations = new Dictionary<string, ProjectWithCompilation>();
        }

        public async Task Adjust()
        {
            await LoadData().ConfigureAwait(false);
            //await CompileSolution().ConfigureAwait(false);
            //await LoadUsedTypes().ConfigureAwait(false);
            await AdjustProjects().ConfigureAwait(false);
        }

        private async Task LoadData()
        {
            LoadTypeMap();
            LoadWorkspace();
            await LoadSolution().ConfigureAwait(false);
        }

        private void LoadTypeMap()
        {
            var paths = File.ReadAllLines(MapFilePath);
            if (paths.Any())
            {
                string typeMapJson = paths.First();
                ListTypeMap = JsonConvert.DeserializeObject<List<CompareTypeResult>>(typeMapJson);
                if (paths.Count() > 1)
                {
                    string methodMapJson = paths[1];
                    ListMethodMap = JsonConvert.DeserializeObject<List<CompareMethodResult>>(methodMapJson);
                }
            }
        }

        private void LoadWorkspace()
        {
            MSBuildLocator.RegisterDefaults();
            Workspace = MSBuildWorkspace.Create();
            Workspace.WorkspaceFailed += (sender, e) => Logger.Error(e.Diagnostic.Message);
            Workspace.AssociateFileExtensionWithLanguage("fsproj", "FSharp");
            Workspace.SkipUnrecognizedProjects = true;
        }

        private async Task LoadSolution()
        {
            //if (SolutionContainsFSharpProjects(SolutionFilePath))
            //{
            //    _processingMode = ProcessingMode.BySingleProject;
            //}
            //else
            //{
            //    _processingMode = ProcessingMode.BySolution;
            //    Solution = await Workspace.OpenSolutionAsync(SolutionFilePath).ConfigureAwait(false);
            //}
            Solution = await Workspace.OpenSolutionAsync(SolutionFilePath).ConfigureAwait(false);
        }

        private static bool SolutionContainsFSharpProjects(string solutionFilePath)
        {
            return
                File
                .ReadAllLines(solutionFilePath)
                .Where(x => x.StartsWith("Project"))
                .Any(x => x.Contains(".fsproj"));
        }

        //internal async Task<CacheResult> GetNugetPackageV2(string packageName, string packageInstallPath)
        //{
        //    var getAddRes = await _cache
        //        .GetOrAddWithFactoryTask
        //        (
        //            packageName,
        //            key =>
        //            {
        //                IPackage package = DownloadNugetPackageV2(key, packageInstallPath);
        //                return new CacheResult(package, false);
        //            }
        //        )
        //        .ConfigureAwait(false);

        //    return getAddRes?.Result?.Value;
        //}

        internal Task<CacheResult> GetNugetPackageV2(string packageName, string packageInstallPath)
        {
            CacheResult cacheResult;
            if (!_dictCache.TryGetValue(packageName, out cacheResult))
            {
                IPackage package = DownloadNugetPackageV2(packageName, packageInstallPath);
                cacheResult = new CacheResult(package, false);
                _dictCache.Add(packageName, cacheResult);
            }
            
            return Task.FromResult(cacheResult);
        }

        internal IPackage DownloadNugetPackageV2(string packageName, string packageInstallPath)
        {
            List<IPackage> packages =
                _nugetRepo
                .FindPackagesById(packageName)
                .OrderByDescending(x => x.Version)
                .ToList();

            var nugetFound = packages.FirstOrDefault();
            if (nugetFound != null)
            {
                Logger.Debug($"Downloading {nugetFound.GetFullName()} package...");

                PackageManager packageManager = new PackageManager(_nugetRepo, packageInstallPath);

                packageManager.InstallPackage(packageName, nugetFound.Version, true, true);

                Logger.Debug($"{nugetFound.Id} nuget package downloaded");
            }

            return nugetFound;
        }

        private async Task AdjustProjects()
        {
            Solution originalSolution = Solution;

            foreach (var projectId in originalSolution.ProjectIds)
            {
                var project = originalSolution.GetProject(projectId);
                var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
                PrjAdj prjAdj = new PrjAdj(Workspace, this, new ProjectWithCompilation(project, compilation));
                originalSolution = await prjAdj.Adjust().ConfigureAwait(false);
            }
            var res = Workspace.TryApplyChanges(originalSolution);
        }


        public void Dispose()
        {
            Workspace?.CloseSolution();
            Workspace?.Dispose();
        }
    }
}
