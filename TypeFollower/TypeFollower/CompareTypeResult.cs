namespace TypeFollower
{
    //public enum CompareObjectType
    //{
    //    Type,
    //    Property,
    //    Method
    //}

    public class CompareTypeResult
    {
        public string OriginalAssemblyName { get; set; }
        public string OriginalNamespace { get; set; }
        public string OriginalType { get; set; }
        public string NewAssemblyName { get; set; }
        public string NewNamespace { get; set; }
        public string NewType { get; set; }
        
    }
}
