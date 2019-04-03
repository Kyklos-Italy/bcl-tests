using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeFollower
{
    [Serializable]
    public class AssemblyObjectContainer
    {
        public IReadOnlyCollection<AssemblyObject> TypeDefinitions { get; }
        public IReadOnlyCollection<AssemblyObject> MethodDefinitions { get; }

        public AssemblyObjectContainer(IEnumerable<AssemblyObject> typeDefinitions, IEnumerable<AssemblyObject> methodDefinitions)
        {
            TypeDefinitions = Array.AsReadOnly(typeDefinitions.ToArray());
            MethodDefinitions = Array.AsReadOnly(methodDefinitions.ToArray());
        }

        public AssemblyObjectContainer Merge(AssemblyObjectContainer other)
        {
            if (other == null)
            {
                return this;
            }

            return
                new AssemblyObjectContainer
                (
                    Array.AsReadOnly(TypeDefinitions.Concat(other.TypeDefinitions).ToArray()),
                    Array.AsReadOnly(MethodDefinitions.Concat(other.MethodDefinitions).ToArray())
                );
        }

        public static AssemblyObjectContainer Empty { get; } = new AssemblyObjectContainer(Enumerable.Empty<AssemblyObject>(), Enumerable.Empty<AssemblyObject>());

    }
}
