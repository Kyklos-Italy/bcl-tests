using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kyklos.Kernel.Core.KLinq;

namespace TypeFollower
{
    public class TypeFollow
    {
        private string _sourceAssemblyFolder;
        private string _sourceAssemblyListPath;
        private string _targetAssemblyFolder;
        private string _targetAssemblyListPath;
        private string _typeNameMapPath;

        private IList<TypeFollowResult> _typeNameMap = new List<TypeFollowResult>();
        private IList<TypeFollowResult> _methodNameMap = new List<TypeFollowResult>();

        private AssemblyObjectContainer _sourceMapping;
        private AssemblyObjectContainer _targetMapping;

        private IList<TypeFollowResult> _typeFollowResult = new List<TypeFollowResult>();
        private IList<TypeFollowResult> _methodFollowResult = new List<TypeFollowResult>();

        //Dictionary<string, Tuple<string, string>> _typeNameMap = new Dictionary<string, Tuple<string, string>>();
        //Dictionary<string, Tuple<string, string>> _methodNameMap = new Dictionary<string, Tuple<string, string>>();


        public TypeFollow(string sourceFolderPath, string sourceListFilePath, string targetFolderPath, string targetListFilePath, string typeNameMapPath)
        {
            _sourceAssemblyFolder = sourceFolderPath;
            _sourceAssemblyListPath = sourceListFilePath;
            _targetAssemblyFolder = targetFolderPath;
            _targetAssemblyListPath = targetListFilePath;
            _typeNameMapPath = typeNameMapPath;
        }

        public void GenerateComparationResult(ComparationResultType resultType, string pathDestination)
        {
            _typeNameMap = FillTypeNameMap(_typeNameMapPath);
            _methodNameMap = FillMethodNameMap(_typeNameMapPath);

            _sourceMapping = FillTypeMap(_sourceAssemblyFolder, _sourceAssemblyListPath); //, out _typeMapSource, out _methodMapSource);            
            _targetMapping = FillTypeMap(_targetAssemblyFolder, _targetAssemblyListPath); //, out _typeMapTarget, out _methodMapTarget);


            _typeFollowResult = CreateTypeFollowResult();
            _methodFollowResult = CreateMethodFollowResult();

            switch (resultType)
            {
                case ComparationResultType.Console:
                    //GenerateComparationResultConsole(pathDestination);
                    break;
                case ComparationResultType.Html:
                    //GenerateComparationResultHtml(pathDestination);
                    break;
                case ComparationResultType.JSON:
                    GenerateComparationResultJSON(pathDestination);
                    break;
                default:
                    break;
            }
        }

        private IList<TypeFollowResult> CreateTypeFollowResult()
        {
            var mappedTypes =
                _sourceMapping
                .TypeDefinitions
                .LeftJoin
                (
                    _targetMapping.TypeDefinitions,
                    x => x.TypeName,
                    x => x.TypeName,
                    (x, y) => new TypeFollowResult(x, y)
                )
                .Union(_typeNameMap)
                .ToList();

            return mappedTypes;
        }

        private IList<TypeFollowResult> CreateMethodFollowResult()
        {
            var mappedMethods =
                _sourceMapping
                .MethodDefinitions
                .LeftJoin
                (
                    _sourceMapping.MethodDefinitions,
                    x => $"{x.TypeName}.{x.MethodName}",
                    x => $"{x.TypeName}.{x.MethodName}",
                    (x, y) => new TypeFollowResult(x, y)
                )
                .Union(_methodNameMap)
                .ToList();

            return mappedMethods;
        }

        private AssemblyObjectContainer FillTypeMap(string assemblyFolder, string assemblyListFilePath) //, out Dictionary<string, List<AssemblyObject>> typeMap, out Dictionary<string, List<AssemblyObject>> methodMap)
        {
            AssemblyObjectContainer assemblyObjectContainer = AssemblyObjectContainer.Empty;

            var assemblyNames = File.ReadAllLines(assemblyListFilePath);
            foreach (string assemblyName in assemblyNames)
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    string path = Path.Combine(assemblyFolder, assemblyName);
                    AssemblyObjectContainer tmpAssemblyObjectContainer = LoadAssemblyTypes(path); //, ref typeMap, ref methodMap);
                    assemblyObjectContainer = assemblyObjectContainer.Merge(tmpAssemblyObjectContainer);
                }
            }

            return assemblyObjectContainer;
        }

        private static IList<TypeFollowResult> FillTypeNameMap(string path)
        {
            List<TypeFollowResult> typeFollowResults = new List<TypeFollowResult>();

            var typeMappings =
                File
                .ReadAllLines(path)
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.StartsWith("t"))
                .Select(x => x.Substring(2));

            foreach (string typeMap in typeMappings)
            {
                string[] types = typeMap.Split("=>".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 1 && types[0].Trim().Length > 0 && types[1].Trim().Length > 0)
                {
                    string from = types[0].Trim().Replace("\"", "");
                    string to = types[1].Trim().Replace("\"", "");

                    AssemblyObject srcAssemblyObject = LineToAssemblyObject(from, false) ?? throw new Exception($"Invalid type map line {from} in {path}");
                    AssemblyObject targetAssemblyObject = LineToAssemblyObject(to, false) ?? throw new Exception($"Invalid type map line {to} in {path}");

                    typeFollowResults.Add(new TypeFollowResult(srcAssemblyObject, targetAssemblyObject));
                }
            }

            return typeFollowResults;
        }

        private static AssemblyObject LineToAssemblyObject(string line, bool isMethod)
        {
            string[] mapdetails = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (mapdetails.Count() > 1 && mapdetails[0].Trim().Length > 0 && mapdetails[1].Trim().Length > 0)
            {
                string typeOrMethodPart = mapdetails[0].Trim();
                int lastPoint = typeOrMethodPart.LastIndexOf('.');

                string methodName = null;
                string typeName = null;
                string namespaceName = null;

                if (isMethod)
                {
                    methodName = typeOrMethodPart.Substring(lastPoint + 1);
                    typeOrMethodPart = typeOrMethodPart.Substring(0, lastPoint);
                    lastPoint = typeOrMethodPart.LastIndexOf('.');
                }

                typeName = typeOrMethodPart.Substring(lastPoint + 1);
                namespaceName = typeOrMethodPart.Substring(0, lastPoint);

                return
                    new AssemblyObject
                    (
                        mapdetails[1].Trim(),
                        typeName,
                        namespaceName,
                        methodName
                    );
            }

            return null;
        }

        private static IList<TypeFollowResult> FillMethodNameMap(string path)
        {
            List<TypeFollowResult> typeFollowResults = new List<TypeFollowResult>();

            var methodMappings =
                File
                .ReadAllLines(path)
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.StartsWith("m"))
                .Select(x => x.Substring(2));

            foreach (string methodMap in methodMappings)
            {
                string[] types = methodMap.Split("=>".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 1 && types[0].Trim().Length > 0 && types[1].Trim().Length > 0)
                {
                    string from = types[0].Trim().Replace("\"", "");
                    string to = types[1].Trim().Replace("\"", "");
                    AssemblyObject srcAssemblyObject = LineToAssemblyObject(from, true) ?? throw new Exception($"Invalid method map line {from} in {path}");
                    AssemblyObject targetAssemblyObject = LineToAssemblyObject(to, true) ?? throw new Exception($"Invalid method map line {to} in {path}");

                    typeFollowResults.Add(new TypeFollowResult(srcAssemblyObject, targetAssemblyObject));
                }
            }

            return typeFollowResults;
        }

        //private string GetRenamedTypeKey(string originalTypeKey)
        //{
        //    Tuple<string, string> map = null;
        //    if (!_typeNameMap.TryGetValue(originalTypeKey, out map))
        //        return originalTypeKey;
        //    return $"{map.Item2}|{map.Item1}";
        //}

        //private string GetRenamedMethodKey(string originalTypeKey)
        //{
        //    Tuple<string, string> map = null;
        //    if (!_methodNameMap.TryGetValue(originalTypeKey, out map))
        //        return originalTypeKey;
        //    return $"{map.Item2}|{map.Item1}";
        //}

        private AssemblyObjectContainer LoadAssemblyTypes(string path) //, ref Dictionary<string, List<AssemblyObject>> mapTypes, ref Dictionary<string, List<AssemblyObject>> mapMethods)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"Loading assembly: {path}");
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                setup.ApplicationName = Path.GetDirectoryName(path);
                AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup);
                ReferenceAssemblyLoader.SourceAssemblyFolder = _sourceAssemblyFolder;
                ReferenceAssemblyLoader.TargetAssemblyFolder = _targetAssemblyFolder;
                newDomain.AssemblyResolve += new ResolveEventHandler(ReferenceAssemblyLoader.ResolveAssemblyEventHandler);
                System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(AssemblyLoader).Assembly.FullName, typeof(AssemblyLoader).FullName);
                AssemblyLoader loader = (AssemblyLoader)obj.Unwrap();
                loader.LoadAssembly(path);
                var assemblyObjectContainer = loader.ParseTypes(); // ref mapTypes, ref mapMethods);
                AppDomain.Unload(newDomain);
                return assemblyObjectContainer;
            }
            else
            {
                throw new Exception($"File not found:{path}");
            }
        }

        // private void GenerateComparationResultConsole(string pathDestination)
        // {
        //     Console.WriteLine(pathDestination);
        //     foreach (var key in _typeFollowResult.Keys)
        //     {
        //         Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
        //         Console.WriteLine($"{key}");
        //         Console.WriteLine($"\t{string.Join(", ", map.Item1.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
        //         Console.WriteLine($"\t\t{string.Join(", ", map.Item2.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
        //     }

        //     foreach (var key in _methodFollowResult.Keys)
        //     {
        //         Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
        //         Console.WriteLine($"{key}");
        //         Console.WriteLine($"\t{string.Join(", ", map.Item1.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
        //         Console.WriteLine($"\t\t{string.Join(", ", map.Item2.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
        //     }
        // }

        // private void GenerateComparationResultHtml(string pathDestination)
        // {
        //     List<string> htmlContent = new List<string>();
        //     htmlContent.Add($"<html><body>");
        //     htmlContent.Add(GetTypeMapTableHtml());
        //     htmlContent.Add("<br/>");
        //     htmlContent.Add(GetMethodMapTableHtml());
        //     htmlContent.Add($"</body></html>");
        //     File.WriteAllLines(pathDestination, htmlContent);
        // }

        // private string GetTypeMapTableHtml()
        // {
        //     string html = @"<table border=1 cellpadding=""5px;"">
        //  <tr>
        //	<th>Type Name</th>
        //	<th>Original Namespace</th>
        //	<th>Original Assembly Name</th>
        //	<th>Final Namespace</th>
        //	<th>Final Type Name</th>
        //	<th>Final Assembly Name</th>
        //</tr>";
        //     foreach (var key in _typeFollowResult.Keys)
        //     {
        //         Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
        //         int newtypeCount = map.Item2.Count;
        //         int oldtypeCount = map.Item1.Count;
        //         for (int i = 0; i < map.Item1.Count; i++)
        //         {
        //             string tablerowHtml = $@"<tr>
        //					<td>{map.Item1[i].TypeName}</td>
        //					<td>{map.Item1[i].TypeNamespace}</td>
        //					<td>{map.Item1[i].AssemblyName}</td>";
        //             int j = i;
        //             if (j >= newtypeCount)
        //                 j = newtypeCount - 1;

        //             tablerowHtml += $@"<td>{(j >= 0 ? map.Item2[j].TypeNamespace : "")}</td>
        //			<td>{(j >= 0 ? map.Item2[j].TypeName : "")}</td>
        //			<td>{(j >= 0 ? map.Item2[j].AssemblyName : "")}</td>
        //			</tr>";
        //             html += tablerowHtml;
        //         }

        //         if (newtypeCount > oldtypeCount)
        //         {
        //             for (int j = oldtypeCount; j < newtypeCount; j++)
        //             {
        //                 string tablerowHtml = $@"<tr>
        //					<td>{map.Item1[oldtypeCount - 1].TypeName}</td>
        //					<td>{map.Item1[oldtypeCount - 1].TypeNamespace}</td>
        //					<td>{map.Item1[oldtypeCount - 1].AssemblyName}</td>
        //					<td>{map.Item2[j].TypeNamespace}</td>
        //					<td>{map.Item2[j].TypeName}</td>
        //					<td>{map.Item2[j].AssemblyName}</td>
        //				</tr>";
        //                 html += tablerowHtml;
        //             }
        //         }
        //     }

        //     html += "</table>";

        //     return html;
        // }

        // private string GetMethodMapTableHtml()
        // {
        //     string html = @"<table border=1 cellpadding=""5px;"">
        //<tr>
        //	<th>Method</th>
        //	<th>Original Namespace</th>
        //	<th>Original Type</th>
        //	<th>Original Assembly Name</th>
        //	<th>Final Namespace</th>
        //	<th>Final Type</th>
        //	<th>Final Assembly Name</th>
        //</tr>";
        //     foreach (var key in _methodFollowResult.Keys)
        //     {
        //         Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
        //         int newtypeCount = map.Item2.Count;
        //         int oldtypeCount = map.Item1.Count;
        //         for (int i = 0; i < map.Item1.Count; i++)
        //         {
        //             string tablerowHtml = $@"<tr>
        //				<td>{key}</td>
        //				<td>{map.Item1[i].TypeNamespace}</td>
        //				<td>{map.Item1[i].TypeName}</td>
        //				<td>{map.Item1[i].AssemblyName}</td>";
        //             int j = i;
        //             if (j >= newtypeCount)
        //                 j = newtypeCount - 1;

        //             tablerowHtml += $@"<td>{(j >= 0 ? map.Item2[j].TypeNamespace : "")}</td>
        //			<td>{(j >= 0 ? map.Item2[j].TypeName : "")}</td>
        //			<td>{(j >= 0 ? map.Item2[j].AssemblyName : "")}</td>
        //			</tr>";
        //             html += tablerowHtml;
        //         }
        //         if (newtypeCount > oldtypeCount)
        //         {
        //             for (int j = oldtypeCount; j < newtypeCount; j++)
        //             {
        //                 string tablerowHtml = $@"<tr>
        //					<td>{key}</td>
        //					<td>{map.Item1[oldtypeCount - 1].TypeNamespace}</td>
        //					<td>{map.Item1[oldtypeCount - 1].TypeName}</td>
        //					<td>{map.Item1[oldtypeCount - 1].AssemblyName}</td>
        //					<td>{map.Item2[j].TypeNamespace}</td>
        //					<td>{map.Item2[j].TypeName}</td>
        //					<td>{map.Item2[j].AssemblyName}</td>
        //				</tr>";
        //                 html += tablerowHtml;
        //             }
        //         }
        //     }

        //     html += "</table>";

        //     return html;
        // }

        private void GenerateComparationResultJSON(string pathDestination)
        {
            string jsonTypes = GetTypeMapJson();
            string jsonMethods = GetMethodMapJson();
            File.WriteAllLines(pathDestination, new List<string> { jsonTypes, jsonMethods });
        }

        private string GetTypeMapJson()
        {
            List<CompareTypeResult> mapList =
                _typeFollowResult
                .Select(x => x.CreateResult())
                .ToList();

            //foreach (var key in _typeFollowResult.Keys)
            //{
            //    Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
            //    int newtypeCount = map.Item2.Count;
            //    int oldtypeCount = map.Item1.Count;
            //    for (int i = 0; i < map.Item1.Count; i++)
            //    {
            //        int j = i;
            //        if (j >= newtypeCount)
            //            j = newtypeCount - 1;

            //        mapList
            //            .Add
            //            (
            //                new CompareTypeResult
            //                {
            //                    OriginalAssemblyName = map.Item1[i].AssemblyName,
            //                    OriginalNamespace = map.Item1[i].TypeNamespace,
            //                    OriginalType = map.Item1[i].TypeName,
            //                    NewAssemblyName = j >= 0 ? map.Item2[j].AssemblyName : string.Empty,
            //                    NewNamespace = j >= 0 ? map.Item2[j].TypeNamespace : string.Empty,
            //                    NewType = j >= 0 ? map.Item2[j].TypeName : string.Empty
            //                }
            //            );
            //    }
            //    if (newtypeCount > oldtypeCount)
            //    {
            //        for (int j = oldtypeCount; j < newtypeCount; j++)
            //        {
            //            mapList.Add(
            //                new CompareTypeResult
            //                {
            //                    OriginalAssemblyName = map.Item1[oldtypeCount - 1].AssemblyName,
            //                    OriginalNamespace = map.Item1[oldtypeCount - 1].TypeNamespace,
            //                    OriginalType = map.Item1[oldtypeCount - 1].TypeName,
            //                    NewAssemblyName = map.Item2[j].AssemblyName,
            //                    NewNamespace = map.Item2[j].TypeNamespace,
            //                    NewType = map.Item2[j].TypeName
            //                }
            //            );
            //        }
            //    }
            //}

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }

        private string GetMethodMapJson()
        {
            List<CompareMethodResult> mapList =
                _methodFollowResult
                .Select(x => x.CreateResult() as CompareMethodResult)
                .ToList();

            //foreach (var key in _methodFollowResult.Keys)
            //{
            //    Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
            //    int newtypeCount = map.Item2.Count;
            //    int oldtypeCount = map.Item1.Count;
            //    for (int i = 0; i < map.Item1.Count; i++)
            //    {
            //        int j = i;
            //        if (j >= newtypeCount)
            //            j = newtypeCount - 1;

            //        mapList.Add(
            //            new CompareMethodResult
            //            {
            //                OriginalAssemblyName = map.Item1[i].AssemblyName,
            //                OriginalNamespace = map.Item1[i].TypeNamespace,
            //                OriginalType = map.Item1[i].TypeName,
            //                OriginalMethodName = map.Item1[i].MethodName,
            //                NewAssemblyName = j >= 0 ? map.Item2[j].AssemblyName : string.Empty,
            //                NewNamespace = j >= 0 ? map.Item2[j].TypeNamespace : string.Empty,
            //                NewType = j >= 0 ? map.Item2[j].TypeName : string.Empty,
            //                NewMethodName = j >= 0 ? map.Item2[j].MethodName : null
            //            }
            //        );
            //    }
            //    if (newtypeCount > oldtypeCount)
            //    {
            //        for (int j = oldtypeCount; j < newtypeCount; j++)
            //        {
            //            mapList.Add(
            //               new CompareMethodResult
            //               {
            //                   OriginalAssemblyName = map.Item1[oldtypeCount - 1].AssemblyName,
            //                   OriginalNamespace = map.Item1[oldtypeCount - 1].TypeNamespace,
            //                   OriginalType = map.Item1[oldtypeCount - 1].TypeName,
            //                   OriginalMethodName = map.Item1[oldtypeCount - 1].MethodName,
            //                   NewAssemblyName = map.Item2[j].AssemblyName,
            //                   NewNamespace = map.Item2[j].TypeNamespace,
            //                   NewType = map.Item2[j].TypeName,
            //                   NewMethodName = map.Item2[j].MethodName
            //               }
            //           );
            //        }
            //    }
            //}

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }
    }
}
