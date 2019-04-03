namespace ProjectAdj
{
    internal class ProjectWithCompilationKey
    {
        public const string OriginalKey = "original";
        private const string RemoveReferenceKey = "remove_reference_";
        private const string AddReferenceKey = "add_reference_";

        public string Key { get; }
        public ProjectWithCompilation ProjectWithCompilation { get; }

        public ProjectWithCompilationKey(string key, ProjectWithCompilation projectWithCompilation)
        {
            Key = key;
            ProjectWithCompilation = projectWithCompilation;
        }

        public static string BuildRemoveReferenceKey(string reference)
        {
            return $"{RemoveReferenceKey}_{reference}";
        }

        public static string BuildAddReferenceKey(string reference)
        {
            return $"{AddReferenceKey}_{reference}";
        }
    }
}
