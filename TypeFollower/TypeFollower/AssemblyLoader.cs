using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeFollower
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private Assembly _assembly;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void LoadAssembly(string path)
        {
            _assembly = Assembly.LoadFrom(/*AssemblyName.GetAssemblyName(path)*/path);
        }

        public string GetAssemblyName()
        {
            return _assembly == null ? null : _assembly.GetName().Name;
        }

        public void ParseTypes(ref Dictionary<string, List<AssemblyObject>> mapTypes, ref Dictionary<string, List<AssemblyObject>> mapMethods)
        {
            var types = _assembly.DefinedTypes;

            foreach (Type tp in types.Where(t => t.IsPublic))
            {
                List<AssemblyObject> assemblies = new List<AssemblyObject>();
                string typeName = $"{GetAssemblyName()}|{tp.Namespace}.{tp.Name}";
                if (mapTypes.ContainsKey(typeName))
                {
                    assemblies = mapTypes[typeName];
                }
                assemblies.Add(new AssemblyObject { AssemblyName = GetAssemblyName(), TypeName = tp.Name, TypeNamespace = tp.Namespace });
                mapTypes[typeName] = assemblies;

                var methods = tp.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in methods)
                {
                    List<AssemblyObject> mal = new List<AssemblyObject>();
                    string methodName = $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})";
                    if (mapMethods.ContainsKey(methodName))
                    {
                        mal = mapMethods[methodName];
                    }
                    mal.Add(new AssemblyObject { AssemblyName = GetAssemblyName(), TypeName = tp.Name, TypeNamespace = tp.Namespace, MethodName = method.Name });
                    mapMethods[methodName] = mal;
                }
            }
        }
       
    }
}
