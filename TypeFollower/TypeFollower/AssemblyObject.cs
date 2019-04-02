using System;

namespace TypeFollower
{
    [Serializable]
    public class AssemblyObject
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public string TypeNamespace { get; set; }
        public string MethodName { get; set; }
    }
}
