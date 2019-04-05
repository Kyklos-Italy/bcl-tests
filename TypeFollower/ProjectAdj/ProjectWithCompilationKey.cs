namespace ProjectAdj
{
    internal class ProjectWithCompilationKey
    {
        public const string OriginalKey = "original";
        private const string RemoveReferenceKey = "remove_reference_";
        private const string AddReferenceKey = "add_reference_";
        private const string ReplaceStaticethodsKey = "replace_static_methods_";
        private const string ReplaceTypesKey = "replace_types_";
        private const string ReplaceUsingsKey = "replace_usings_";
        

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

        public static string BuilReplaceStaticMethodsKey(string docName)
        {
            return $"{ReplaceStaticethodsKey}_{docName}";
        }

        public static string BuilReplaceTypesKey(string prjName)
        {
            return $"{ReplaceTypesKey}_{prjName}";
        }

        public static string BuilReplaceUsingsKey(string docName)
        {
            return $"{ReplaceUsingsKey}_{docName}";
        }
    }
}
