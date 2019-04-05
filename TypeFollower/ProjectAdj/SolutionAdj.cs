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
        public string SolutionFilePath { get; }
        public string MapFilePath { get; }
        public string NugetApiEndpoint { get; }

        private ILog Logger { get; } = LogManager.GetLogger(typeof(SolutionAdj));

        public MSBuildWorkspace Workspace { get; private set; }
        public Solution Solution { get; private set; }

        public IList<CompareTypeResult> ListTypeMap { get; private set; }
        public IList<CompareMethodResult> ListMethodMap { get; private set; }
        private Dictionary<string, ProjectWithCompilation> _compilations;
        private KCache<string, CacheResult> _cache;
        private IPackageRepository _nugetRepo;


        public SolutionAdj(string solutionFilePath, string mapFilePath, string nugetApiEndpoint)
        {
            SolutionFilePath = solutionFilePath;
            MapFilePath = mapFilePath;
            NugetApiEndpoint = nugetApiEndpoint;

            int timeToLiveInSeconds = 3600;
            _cache = new KCache<string, CacheResult>(timeToLiveInSeconds * 1000, true);
            _nugetRepo = PackageRepositoryFactory.Default.CreateRepository(nugetApiEndpoint);
            _compilations = new Dictionary<string, ProjectWithCompilation>();
        }

        public async Task Adjust()
        {
            await LoadData().ConfigureAwait(false);
            await CompileSolution().ConfigureAwait(false);
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
        }

        private async Task LoadSolution()
        {
            Solution = await Workspace.OpenSolutionAsync(SolutionFilePath).ConfigureAwait(false);
        }

        private async Task CompileSolution()
        {
            foreach (var project in Solution.Projects)
            {
                var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
                _compilations.Add(project.Name, new ProjectWithCompilation(project, compilation));
            }
        }

        internal async Task<CacheResult> GetNugetPackageV2(string packageName, string packageInstallPath)
        {
            var getAddRes = await _cache
                .GetOrAddWithFactoryTask
                (
                    packageName,
                    key =>
                    {
                        IPackage package = DownloadNugetPackageV2(key, packageInstallPath);
                        return new CacheResult(package, false);
                    }
                )
                .ConfigureAwait(false);

            return getAddRes.Result.Value;
        }

        private IPackage DownloadNugetPackageV2(string packageName, string packageInstallPath)
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

                packageManager.InstallPackage(packageName, nugetFound.Version, false, true);

                Logger.Debug($"{nugetFound.Id} nuget package downloaded");
            }

            return nugetFound;
        }

        private async Task AdjustProjects()
        {
            Solution originalSolution = Solution;
            foreach (var prjWithComp in _compilations.Values)
            {
                PrjAdj prjAdj = new PrjAdj(Workspace, this, prjWithComp);
                originalSolution = await prjAdj.Adjust().ConfigureAwait(false);
                var res = Workspace.TryApplyChanges(originalSolution);
            }            
        }


        public void Dispose()
        {
            Workspace?.Dispose();
        }
    }
}
