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
        private IList<INamedTypeSymbol> _usedTypes;

        #region Nested classes

        private class ProjectWithCompilation
        {
            public Project Project { get; }
            public Compilation Compilation { get; }

            public ProjectWithCompilation(Project project, Compilation compilation)
            {
                Project = project;
                Compilation = compilation;
            }
        }

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

        #endregion
        
        private KCache<string, CacheResult> _cache;

        
        public SolutionAdj(string solutionFilePath, string mapFilePath, string nugetApiEndpoint)
        {
            SolutionFilePath = solutionFilePath;
            MapFilePath = mapFilePath;
            NugetApiEndpoint = nugetApiEndpoint;

            int timeToLiveInSeconds = 3600;
            _cache = new KCache<string, CacheResult>(timeToLiveInSeconds * 1000, true);

            _compilations = new Dictionary<string, ProjectWithCompilation>();
        }

        public async Task Adjust()
        {
            await LoadData().ConfigureAwait(false);
            await CompileSolution().ConfigureAwait(false);
            await LoadUsedTypes().ConfigureAwait(false);
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

        private async Task LoadUsedTypes()
        {
            var compilations =
                _compilations
                .Values
                .Select(x => x.Compilation);

            var allUsedTypes = await
                compilations
                .GetAllUsedTypesInCompilations()
                .ConfigureAwait(false);

            _usedTypes = allUsedTypes.ExcludeSystemTypes();
        }

        private IList<string> DetermineMissingNugets()
        {
            return
                ListTypeMap
                .Join
                (
                    _usedTypes,
                    x => $"{x.OriginalNamespace}.{x.OriginalType}@{x.OriginalAssemblyName}",
                    x => $"{x.ContainingNamespace.ToString()}.{x.Name}@{x.ContainingAssembly.Name}",
                    (x, y) => x.NewAssemblyName
                )
                .Distinct()
                .ToList();
        }


        public void Dispose()
        {
            Workspace?.Dispose();
        }
    }
}
