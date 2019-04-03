using System;

namespace TypeFollower
{
    public class TypeFollowResult
    {
        public AssemblyObject SourceAssemblyObject { get; }
        public AssemblyObject TargetAssemblyObject { get; }

        public TypeFollowResult(AssemblyObject sourceAssemblyObject, AssemblyObject targetAssemblyObject)
        {
            SourceAssemblyObject = sourceAssemblyObject ?? throw new ArgumentNullException(nameof(sourceAssemblyObject));
            TargetAssemblyObject = targetAssemblyObject;

            if (targetAssemblyObject != null)
            {
                if (sourceAssemblyObject.TypeOrMethod != targetAssemblyObject.TypeOrMethod)
                {
                    throw new ArgumentException($"Incompatible assembly objects. Source: {SourceAssemblyObject.ToString()} - Target {TargetAssemblyObject.ToString()}");
                }
            }
        }

        public CompareTypeResult CreateResult()
        {
            if (SourceAssemblyObject.IsType)
            {
                return
                    new CompareTypeResult
                    {
                        OriginalAssemblyName = SourceAssemblyObject.AssemblyName,
                        OriginalNamespace = SourceAssemblyObject.TypeNamespace,
                        OriginalType = SourceAssemblyObject.TypeName,
                        NewAssemblyName = TargetAssemblyObject?.AssemblyName ?? string.Empty,
                        NewNamespace = TargetAssemblyObject?.TypeNamespace ?? string.Empty,
                        NewType = TargetAssemblyObject?.TypeName ?? string.Empty
                    };
            }

            return
                new CompareMethodResult
                {
                    OriginalAssemblyName = SourceAssemblyObject.AssemblyName,
                    OriginalNamespace = SourceAssemblyObject.TypeNamespace,
                    OriginalType = SourceAssemblyObject.TypeName,
                    OriginalMethodName = SourceAssemblyObject.MethodName,
                    NewAssemblyName = TargetAssemblyObject?.AssemblyName ?? string.Empty,
                    NewNamespace = TargetAssemblyObject?.TypeNamespace ?? string.Empty,
                    NewType = TargetAssemblyObject?.TypeName ?? string.Empty,
                    NewMethodName = TargetAssemblyObject?.MethodName ?? string.Empty
                };
        }
    }
}
