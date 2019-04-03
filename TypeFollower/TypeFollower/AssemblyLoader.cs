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
            _assembly = Assembly.LoadFrom(path);
        }

        public string GetAssemblyName()
        {
            return _assembly?.GetName()?.Name;
        }

        public AssemblyObjectContainer ParseTypes() //ref Dictionary<string, List<AssemblyObject>> mapTypes, ref Dictionary<string, List<AssemblyObject>> mapMethods)
        {
            var types = _assembly.DefinedTypes;

            ISet<AssemblyObject> typeDefs = new HashSet<AssemblyObject>();
            ISet<AssemblyObject> methodDefs = new HashSet<AssemblyObject>();

            foreach (Type tp in types.Where(t => t.IsPublic))
            {
                //List<AssemblyObject> assemblies = new List<AssemblyObject>();
                //string typeName = $"{GetAssemblyName()}|{tp.Namespace}.{tp.Name}";
                //if (mapTypes.ContainsKey(typeName))
                //{
                //    assemblies = mapTypes[typeName];
                //}
                //assemblies.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace));
                //mapTypes[typeName] = assemblies;

                typeDefs.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace));


                var methods = tp.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => !x.IsSpecialName);
                foreach (var method in methods)
                {
                    ////string methodName = $"{GetAssemblyName()}|{tp.Namespace}.{tp.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.FullName}"))}):{method.ReturnType.Name}";
                    string methodName = $"{GetAssemblyName()}|{tp.Namespace}.{tp.Name}.{method.Name}";
                    //if (mapMethods.ContainsKey(methodName))
                    //{
                    //    mal = mapMethods[methodName];
                    //}
                    //mal.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace, method.Name));
                    //mapMethods[methodName] = mal;
                    methodDefs.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace, method.Name));
                }

                var props = tp.GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (var prop in props)
                {
                    methodDefs.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace, prop.Name));
                    //List<AssemblyObject> mal;
                    //string propName = $"{prop.PropertyType.Name} {prop.Name}";

                    //if (mapMethods.TryGetValue(propName, out mal))
                    //{
                    //    mal.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace, prop.Name));
                    //}
                    //else
                    //{
                    //    mal = new List<AssemblyObject>();
                    //    mal.Add(new AssemblyObject(GetAssemblyName(), tp.Name, tp.Namespace, prop.Name));
                    //    mapMethods.Add(propName, mal);
                    //}
                }
            }

            return new AssemblyObjectContainer(typeDefs, methodDefs);
        }
    }
}
