using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kyklos.Kernel.Core.KLinq;
using Kyklos.Kernel.Core.Support;

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
                    GenerateComparationResultHtml(pathDestination);
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
                .Except
                (
                    _typeNameMap,
                    KyklosEqualityComparer
                    .GetEqualityComparer<TypeFollowResult>
                    (
                        (x, y) =>
                            x.SourceAssemblyObject.IsType
                            && y.SourceAssemblyObject.IsType
                            && x.SourceAssemblyObject.TypeName == y.SourceAssemblyObject.TypeName
                            && x.SourceAssemblyObject.AssemblyName == y.SourceAssemblyObject.AssemblyName
                            && x.SourceAssemblyObject.TypeNamespace == y.SourceAssemblyObject.TypeNamespace,
                        x => x.SourceAssemblyObject.GetHashCode()
                    )
                )
                .Union(_typeNameMap);

            return ReorderData(mappedTypes, false).ToList();
        }

        private static IEnumerable<TypeFollowResult> ReorderData(IEnumerable<TypeFollowResult> data, bool isMethod)
        {
            var missingTypes =
                data
                .Where(x => x.TargetAssemblyObject == null);

            var matchingTypes =
                data
                .Where(x => x.TargetAssemblyObject != null);

            return
                SortTypeFollowResult(missingTypes, isMethod)
                .Concat(SortTypeFollowResult(matchingTypes, isMethod))
                .ToList();
        }

        private static IEnumerable<TypeFollowResult> SortTypeFollowResult(IEnumerable<TypeFollowResult> data, bool isMethod)
        {
            if (isMethod)
            {
                return
                    data
                    .OrderBy(x => x.SourceAssemblyObject.MethodName)
                    .ThenBy(x => x.SourceAssemblyObject.FullTypeName);
            }
            return
                data
                .OrderBy(x => x.SourceAssemblyObject.AssemblyName)
                .ThenBy(x => x.SourceAssemblyObject.FullTypeName);
        }

        private IList<TypeFollowResult> CreateMethodFollowResult()
        {
            var comparer = KyklosEqualityComparer
                    .GetEqualityComparer<TypeFollowResult>
                    (
                        (x, y) =>
                            x.SourceAssemblyObject.IsMethod
                            && y.SourceAssemblyObject.IsMethod
                            && x.SourceAssemblyObject.TypeName == y.SourceAssemblyObject.TypeName
                            && x.SourceAssemblyObject.AssemblyName == y.SourceAssemblyObject.AssemblyName
                            && x.SourceAssemblyObject.TypeNamespace == y.SourceAssemblyObject.TypeNamespace
                            && x.SourceAssemblyObject.MethodName == y.SourceAssemblyObject.MethodName,
                        x => x.SourceAssemblyObject.GetHashCode()
                    );

            var mappedMethods =
                _sourceMapping
                .MethodDefinitions
                .LeftJoin
                (
                    _targetMapping.MethodDefinitions,
                    x => $"{x.TypeName}.{x.MethodName}",
                    x => $"{x.TypeName}.{x.MethodName}",
                    (x, y) => new TypeFollowResult(x, y)
                );

            var mappedByNameOnly =
                mappedMethods
                .Where(x => x.TargetAssemblyObject == null)
                .Join
                (
                    _targetMapping.MethodDefinitions,
                    x => $"{x.SourceAssemblyObject.MethodName}",
                    x => $"{x.MethodName}",
                    (x, y) => new TypeFollowResult(x.SourceAssemblyObject, y)
                );

            var data = 
                mappedMethods
                .Where(x => x.TargetAssemblyObject != null)
                .Concat(mappedByNameOnly)
                .Except
                (
                    _methodNameMap,
                    comparer
                )
                .Union(_methodNameMap)
                .ToArray();

            return ReorderData(data, true).ToList();
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
                .Select(x => x.Trim())
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

        private void GenerateComparationResultHtml(string pathDestination)
        {            
            string typesHtml = GetTypeMapTableHtml();            
            string methodsHtml = GetMethodMapTableHtml();

            WriteComparationResultHtml($"{pathDestination}-types.html", typesHtml);
            WriteComparationResultHtml($"{pathDestination}-methods.html", methodsHtml);
        }

        private void WriteComparationResultHtml(string pathDestination, string html)
        {
            List<string> htmlContent = new List<string>();
            htmlContent.Add($"<html><body>");
            htmlContent.Add(html);
            htmlContent.Add($"</body></html>");
            File.WriteAllLines(pathDestination, htmlContent);
        }

        private string GetTypeMapTableHtml()
        {
            string html = @"<table border=1 cellpadding=""5px;"">
          <tr>
        	<th>Original Type Name</th>
        	<th>Original Namespace</th>
        	<th>Original Assembly</th>
            <th>New Type Name</th>        	
            <th>New Namespace</th>        	
        	<th>New Assembly</th>
        </tr>";

            List<CompareTypeResult> mapList =
                _typeFollowResult
                .Select(x => x.CreateResult())
                .ToList();

            foreach (var map in mapList)
            {
                    string tablerowHtml = $@"<tr>
        					<td>{map.OriginalType}</td>
        					<td>{map.OriginalNamespace}</td>
        					<td>{map.OriginalAssemblyName}</td>
                            <td>{map.NewType}</td>
        					<td>{map.NewNamespace}</td>
        					<td>{map.NewAssemblyName}</td>
        			</tr>";
                    html += tablerowHtml;
            }

            html += "</table>";

            return html;
        }

        private string GetMethodMapTableHtml()
        {
            string html = @"<table border=1 cellpadding=""5px;"">
          <tr>
        	<th>Original Method Name</th>
            <th>Original Type Name</th>
        	<th>Original Namespace</th>
        	<th>Original Assembly</th>
            <th>New Method Name</th>        	
            <th>New Type Name</th>        	
            <th>New Namespace</th>        	
        	<th>New Assembly</th>
        </tr>";

            List<CompareMethodResult> mapList =
                _methodFollowResult
                .Select(x => x.CreateResult() as CompareMethodResult)
                .ToList();

            foreach (var map in mapList)
            {
                string tablerowHtml = $@"<tr>
        					<td>{map.OriginalMethodName}</td>
                            <td>{map.OriginalType}</td>
        					<td>{map.OriginalNamespace}</td>
        					<td>{map.OriginalAssemblyName}</td>
                            <td>{map.NewMethodName}</td>
                            <td>{map.NewType}</td>
        					<td>{map.NewNamespace}</td>
        					<td>{map.NewAssemblyName}</td>
        			</tr>";
                html += tablerowHtml;
            }

            html += "</table>";

            return html;
        }

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

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }

        private string GetMethodMapJson()
        {
            List<CompareMethodResult> mapList =
                _methodFollowResult
                .Select(x => x.CreateResult() as CompareMethodResult)
                .ToList();

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }
    }
}
