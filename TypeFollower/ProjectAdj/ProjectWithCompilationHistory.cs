using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ProjectAdj
{
    internal class ProjectWithCompilationHistory
    {
        public IList<ProjectWithCompilationKey> History { get; }

        public ProjectWithCompilationHistory(ProjectWithCompilation projectWithCompilation)
        {
            History = new List<ProjectWithCompilationKey>();
            AddProjectToHistory(ProjectWithCompilationKey.OriginalKey, projectWithCompilation);
        }

        public ProjectWithCompilationKey AddProjectToHistory(string key, Project project )
        {
            return AddProjectToHistory(key, project, null);
        }

        public ProjectWithCompilationKey AddProjectToHistory(string key, Project project, Compilation compilation)
        {
            return AddProjectToHistory(key, new ProjectWithCompilation(project, compilation));
        }

        public ProjectWithCompilationKey AddProjectToHistory(string key, ProjectWithCompilation projectWithCompilation)
        {
            ProjectWithCompilationKey projectWithCompilationKey = new ProjectWithCompilationKey(key, projectWithCompilation);

            History.Add(projectWithCompilationKey);

            return projectWithCompilationKey;
        }

        public Project CurrentProject => 
            History
            .Last()
            .ProjectWithCompilation
            .Project;

        public Compilation CurrentCompilation =>
            History
            .Last(x => x.ProjectWithCompilation.Compilation != null)
            .ProjectWithCompilation
            .Compilation;

        public Solution CurrentSolution =>
            History
            .Last(x => x.ProjectWithCompilation.Solution != null)
            .ProjectWithCompilation
            .Solution;


        //public ProjectWithCompilation CurrentProjectWithCompilation =>
        //    History
        //    .Last()
        //    .ProjectWithCompilation;
    }
}
