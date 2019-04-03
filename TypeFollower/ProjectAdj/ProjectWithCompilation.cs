using Microsoft.CodeAnalysis;

namespace ProjectAdj
{
    internal class ProjectWithCompilation
    {
        public Project Project { get; }
        public Compilation Compilation { get; }
        public Solution Solution => Project.Solution;

        public ProjectWithCompilation(Project project, Compilation compilation)
        {
            Project = project;
            Compilation = compilation;
        }
    }
}
