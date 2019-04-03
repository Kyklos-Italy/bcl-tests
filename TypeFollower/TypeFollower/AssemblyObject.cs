using System;
using System.Collections.Generic;

namespace TypeFollower
{
    [Serializable]
    public class AssemblyObject : IEquatable<AssemblyObject>
    {
        public string AssemblyName { get; }
        public string TypeName { get;  }
        public string TypeNamespace { get;  }
        public string MethodName { get;  }

        public bool IsType => string.IsNullOrEmpty(MethodName);
        public bool IsMethod => !IsType;
        public string TypeOrMethod => IsType ? "Type" : "Method";
        public string FullTypeName => $"{TypeNamespace}.{TypeName}";

        public AssemblyObject(string assemblyName, string typeName, string typeNamespace) 
            : this(assemblyName, typeName, typeNamespace, null)
        {
        }

        public AssemblyObject(string assemblyName, string typeName, string typeNamespace, string methodName)
        {
            AssemblyName = assemblyName;
            TypeName = typeName;
            TypeNamespace = typeNamespace;
            MethodName = methodName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AssemblyObject);
        }

        public bool Equals(AssemblyObject other)
        {
            return other != null &&
                   AssemblyName == other.AssemblyName &&
                   TypeName == other.TypeName &&
                   TypeNamespace == other.TypeNamespace &&
                   MethodName == other.MethodName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1509975503;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssemblyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeNamespace);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MethodName);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{FullTypeName}{(IsMethod ? $".{MethodName}" : string.Empty)}, {AssemblyName}";
        }
    }
}
