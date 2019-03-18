﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypeFollower
{
    public class TypeFollow
    {
        string _sourceAssemblyListPath;
        string _targetAssemblyListPath;
        string _typeNameMapPath;

        Dictionary<string, List<AssemblyObject>> _typeMapSource = new Dictionary<string, List<AssemblyObject>>();
        Dictionary<string, List<AssemblyObject>> _typeMapTarget = new Dictionary<string, List<AssemblyObject>>();
        Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>> _typeFollowResult = new Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>>();

        Dictionary<string, List<AssemblyObject>> _methodMapSource = new Dictionary<string, List<AssemblyObject>>();
        Dictionary<string, List<AssemblyObject>> _methodMapTarget = new Dictionary<string, List<AssemblyObject>>();
        Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>> _methodFollowResult = new Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>>();

        Dictionary<string, Tuple<string, string>> _typeNameMap = new Dictionary<string, Tuple<string, string>>();

        public TypeFollow(string sourceListPath, string targetListPath, string typeNameMapPath)
        {
            _sourceAssemblyListPath = sourceListPath;
            _targetAssemblyListPath = targetListPath;
            _typeNameMapPath = typeNameMapPath;

            CompareTypeResult.TestMethod();
        }

        public void GenerateComparationResult(ComparationResultType resultType, string pathDestination)
        {
            FillTypeMap(_sourceAssemblyListPath, out _typeMapSource, out _methodMapSource);
            FillTypeNameMap(_typeNameMapPath);
            FillTypeMap(_targetAssemblyListPath, out _typeMapTarget, out _methodMapTarget);
            CreateTypeFollowResult();
            CreateMethodFollowResult();
            switch (resultType)
            {
                case ComparationResultType.Console:
                    GenerateComparationResultConsole(pathDestination);
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

        private void CreateTypeFollowResult()
        {
            _typeFollowResult = new Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>>();
            foreach (var typeKey in _typeMapSource.Keys)
            {
                List<AssemblyObject> targetAssemblies = null;
                if (!_typeMapTarget.TryGetValue(GetRenamedTypeKey(typeKey), out targetAssemblies))
                    targetAssemblies = new List<AssemblyObject>();
                Tuple<List<AssemblyObject>, List<AssemblyObject>> result = new Tuple<List<AssemblyObject>, List<AssemblyObject>>(_typeMapSource[typeKey], targetAssemblies);
                _typeFollowResult[typeKey] = result;
            }
        }

        private void CreateMethodFollowResult()
        {
            _methodFollowResult = new Dictionary<string, Tuple<List<AssemblyObject>, List<AssemblyObject>>>();
            foreach (var method in _methodMapSource.Keys)
            {
                List<AssemblyObject> targetAssemblies = null;
                if (!_methodMapTarget.TryGetValue(method, out targetAssemblies))
                    targetAssemblies = new List<AssemblyObject>();
                Tuple<List<AssemblyObject>, List<AssemblyObject>> result = new Tuple<List<AssemblyObject>, List<AssemblyObject>>(_methodMapSource[method], targetAssemblies);
                _methodFollowResult[method] = result;
            }
        }

        private void FillTypeMap(string pathAssemblyList, out Dictionary<string, List<AssemblyObject>> typeMap, out Dictionary<string, List<AssemblyObject>> methodMap)
        {
            typeMap = new Dictionary<string, List<AssemblyObject>>();
            methodMap = new Dictionary<string, List<AssemblyObject>>();
            var paths = File.ReadAllLines(pathAssemblyList);
            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    LoadAssemblyTypes(path, ref typeMap, ref methodMap);
                }
            }
        }

        private void FillTypeNameMap(string path)
        {
            _typeNameMap = new Dictionary<string, Tuple<string, string>>();
            var typeMappings = File.ReadAllLines(path);
            foreach (string typeMap in typeMappings)
            {
                if (!string.IsNullOrEmpty(typeMap))
                {
                    string[] types = typeMap.Split("=>".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (types.Count() > 1 && types[0].Trim().Length > 0 && types[1].Trim().Length > 0)
                    {
                        string mapKey = null;
                        string from = types[0].Trim().Replace("\"", "");
                        string to = types[1].Trim().Replace("\"", "");
                        string[] mapdetails = from.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (mapdetails.Count() > 1 && mapdetails[0].Trim().Length > 0 && mapdetails[1].Trim().Length > 0)
                        {
                            mapKey = $"{mapdetails[1].Trim()}|{mapdetails[0].Trim()}";
                        }

                        string[] renameddetails = to.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (renameddetails.Count() > 1 && renameddetails[0].Trim().Length > 0 && renameddetails[1].Trim().Length > 0)
                        {
                            if (!string.IsNullOrEmpty(mapKey))
                                _typeNameMap[mapKey] = new Tuple<string, string>(renameddetails[0].Trim(), renameddetails[1].Trim());
                        }
                    }
                }
            }
        }

        private string GetRenamedTypeKey(string originalTypeKey)
        {
            Tuple<string, string> map = null;
            if (!_typeNameMap.TryGetValue(originalTypeKey, out map))
                return originalTypeKey;
            return $"{map.Item2}|{map.Item1}";
        }

        private void LoadAssemblyTypes(string path, ref Dictionary<string, List<AssemblyObject>> mapTypes, ref Dictionary<string, List<AssemblyObject>> mapMethods)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"Loading assembly:{path}");
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                setup.ApplicationName = Path.GetDirectoryName(path);
                AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup);
                newDomain.AssemblyResolve += new ResolveEventHandler(ReferenceAssemblyLoader.ResolveAssemblyEventHandler);
                System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(AssemblyLoader).Assembly.FullName, typeof(AssemblyLoader).FullName);
                AssemblyLoader loader = (AssemblyLoader)obj.Unwrap();
                loader.LoadAssembly(path);
                loader.ParseTypes(ref mapTypes, ref mapMethods);
                AppDomain.Unload(newDomain);

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
            else
                Console.WriteLine($"File not found:{path}");
        }

        //private void ParseTypes(Assembly assembly, ref Dictionary<string, List<AssemblyObject>> mapTypes, ref Dictionary<string, List<AssemblyObject>> mapMethods)
        //{
        //    var types = assembly.DefinedTypes;

        //    foreach (Type tp in types.Where(t => t.IsPublic))
        //    {
        //        List<AssemblyObject> assemblies = new List<AssemblyObject>();
        //        string typeName = $"{assembly.GetName().Name}|{tp.Namespace}.{tp.Name}";
        //        if (mapTypes.ContainsKey(typeName))
        //        {
        //            assemblies = mapTypes[typeName];
        //        }
        //        assemblies.Add(new AssemblyObject { AssemblyName = assembly.GetName().Name, TypeName = tp.Name, TypeNamespace = tp.Namespace });
        //        mapTypes[typeName] = assemblies;

        //        var methods = tp.GetMethods(BindingFlags.Public | BindingFlags.Static);
        //        foreach (var method in methods)
        //        {
        //            List<AssemblyObject> mal = new List<AssemblyObject>();
        //            string methodName = $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})";
        //            if (mapMethods.ContainsKey(methodName))
        //            {
        //                mal = mapMethods[methodName];
        //            }
        //            mal.Add(new AssemblyObject { AssemblyName = assembly.GetName().Name, TypeName = tp.Name, TypeNamespace = tp.Namespace, MethodName = method.Name });
        //            mapMethods[methodName] = mal;
        //        }
        //    }
        //}

        private void GenerateComparationResultConsole(string pathDestination)
        {
            Console.WriteLine(pathDestination);
            foreach (var key in _typeFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
                Console.WriteLine($"{key}");
                Console.WriteLine($"\t{string.Join(", ", map.Item1.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
                Console.WriteLine($"\t\t{string.Join(", ", map.Item2.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
            }

            foreach (var key in _methodFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
                Console.WriteLine($"{key}");
                Console.WriteLine($"\t{string.Join(", ", map.Item1.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
                Console.WriteLine($"\t\t{string.Join(", ", map.Item2.Select(t => $"{t.TypeNamespace} {t.AssemblyName}"))}");
            }
        }

        private void GenerateComparationResultHtml(string pathDestination)
        {
            List<string> htmlContent = new List<string>();
            htmlContent.Add($"<html><body>");
            htmlContent.Add(GetTypeMapTableHtml());
            htmlContent.Add("<br/>");
            htmlContent.Add(GetMethodMapTableHtml());
            htmlContent.Add($"</body></html>");
            File.WriteAllLines(pathDestination, htmlContent);
        }

        private string GetTypeMapTableHtml()
        {
            string html = @"<table border=1 cellpadding=""5px;"">
							  <tr>
								<th>Type Name</th>
								<th>Original Namespace</th>
								<th>Original Assembly Name</th>
								<th>Final Namespace</th>
								<th>Final Type Name</th>
								<th>Final Assembly Name</th>
							</tr>";
            foreach (var key in _typeFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
                int newtypeCount = map.Item2.Count;
                int oldtypeCount = map.Item1.Count;
                for (int i = 0; i < map.Item1.Count; i++)
                {
                    string tablerowHtml = $@"<tr>
												<td>{map.Item1[i].TypeName}</td>
												<td>{map.Item1[i].TypeNamespace}</td>
												<td>{map.Item1[i].AssemblyName}</td>";
                    int j = i;
                    if (j >= newtypeCount)
                        j = newtypeCount - 1;

                    tablerowHtml += $@"<td>{(j >= 0 ? map.Item2[j].TypeNamespace : "")}</td>
										<td>{(j >= 0 ? map.Item2[j].TypeName : "")}</td>
										<td>{(j >= 0 ? map.Item2[j].AssemblyName : "")}</td>
										</tr>";
                    html += tablerowHtml;
                }
                //string tablerowHtml = $@"<tr>
                //							<td>{key}</td>
                //							<td>{string.Join("", map.Item1.Select(t => $"<div>{t.Type.Namespace}</div>"))}</td>
                //							<td>{string.Join("", map.Item1.Select(t => $"<div>{t.Assembly.GetName().Name}</div>"))}</td>
                //							<td>{string.Join("", map.Item2.Select(t => $"<div>{t.Type.Namespace}</div>"))}</td>
                //							<td>{string.Join("", map.Item2.Select(t => $"<div>{t.Assembly.GetName().Name}</div>"))}</td>
                //						</tr>";
                //html += tablerowHtml;

                if (newtypeCount > oldtypeCount)
                {
                    for (int j = oldtypeCount; j < newtypeCount; j++)
                    {
                        string tablerowHtml = $@"<tr>
												<td>{map.Item1[oldtypeCount - 1].TypeName}</td>
												<td>{map.Item1[oldtypeCount - 1].TypeNamespace}</td>
												<td>{map.Item1[oldtypeCount - 1].AssemblyName}</td>
												<td>{map.Item2[j].TypeNamespace}</td>
												<td>{map.Item2[j].TypeName}</td>
												<td>{map.Item2[j].AssemblyName}</td>
											</tr>";
                        html += tablerowHtml;
                    }
                }
            }

            html += "</table>";

            return html;
        }

        private string GetMethodMapTableHtml()
        {
            string html = @"<table border=1 cellpadding=""5px;"">
							<tr>
								<th>Method</th>
								<th>Original Namespace</th>
								<th>Original Type</th>
								<th>Original Assembly Name</th>
								<th>Final Namespace</th>
								<th>Final Type</th>
								<th>Final Assembly Name</th>
							</tr>";
            foreach (var key in _methodFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
                int newtypeCount = map.Item2.Count;
                int oldtypeCount = map.Item1.Count;
                for (int i = 0; i < map.Item1.Count; i++)
                {
                    string tablerowHtml = $@"<tr>
											<td>{key}</td>
											<td>{map.Item1[i].TypeNamespace}</td>
											<td>{map.Item1[i].TypeName}</td>
											<td>{map.Item1[i].AssemblyName}</td>";
                    int j = i;
                    if (j >= newtypeCount)
                        j = newtypeCount - 1;

                    tablerowHtml += $@"<td>{(j >= 0 ? map.Item2[j].TypeNamespace : "")}</td>
										<td>{(j >= 0 ? map.Item2[j].TypeName : "")}</td>
										<td>{(j >= 0 ? map.Item2[j].AssemblyName : "")}</td>
										</tr>";
                    html += tablerowHtml;
                }
                if (newtypeCount > oldtypeCount)
                {
                    for (int j = oldtypeCount; j < newtypeCount; j++)
                    {
                        string tablerowHtml = $@"<tr>
												<td>{key}</td>
												<td>{map.Item1[oldtypeCount - 1].TypeNamespace}</td>
												<td>{map.Item1[oldtypeCount - 1].TypeName}</td>
												<td>{map.Item1[oldtypeCount - 1].AssemblyName}</td>
												<td>{map.Item2[j].TypeNamespace}</td>
												<td>{map.Item2[j].TypeName}</td>
												<td>{map.Item2[j].AssemblyName}</td>
											</tr>";
                        html += tablerowHtml;
                    }
                }
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
            List<CompareTypeResult> mapList = new List<CompareTypeResult>();
            foreach (var key in _typeFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _typeFollowResult[key];
                int newtypeCount = map.Item2.Count;
                int oldtypeCount = map.Item1.Count;
                for (int i = 0; i < map.Item1.Count; i++)
                {
                    int j = i;
                    if (j >= newtypeCount)
                        j = newtypeCount - 1;

                    mapList.Add(
                        new CompareTypeResult
                        {
                            OriginalAssemblyName = map.Item1[i].AssemblyName,
                            OriginalNamespace = map.Item1[i].TypeNamespace,
                            OriginalType = map.Item1[i].TypeName,
                            NewAssemblyName = j >= 0 ? map.Item2[j].AssemblyName : string.Empty,
                            NewNamespace = j >= 0 ? map.Item2[j].TypeNamespace : string.Empty,
                            NewType = j >= 0 ? map.Item2[j].TypeName : string.Empty
                        }
                    );
                }
                if (newtypeCount > oldtypeCount)
                {
                    for (int j = oldtypeCount; j < newtypeCount; j++)
                    {
                        mapList.Add(
                            new CompareTypeResult
                            {
                                OriginalAssemblyName = map.Item1[oldtypeCount - 1].AssemblyName,
                                OriginalNamespace = map.Item1[oldtypeCount - 1].TypeNamespace,
                                OriginalType = map.Item1[oldtypeCount - 1].TypeName,
                                NewAssemblyName = map.Item2[j].AssemblyName,
                                NewNamespace = map.Item2[j].TypeNamespace,
                                NewType = map.Item2[j].TypeName
                            }
                        );
                    }
                }
            }

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }

        private string GetMethodMapJson()
        {
            List<CompareMethodResult> mapList = new List<CompareMethodResult>();
            foreach (var key in _methodFollowResult.Keys)
            {
                Tuple<List<AssemblyObject>, List<AssemblyObject>> map = _methodFollowResult[key];
                int newtypeCount = map.Item2.Count;
                int oldtypeCount = map.Item1.Count;
                for (int i = 0; i < map.Item1.Count; i++)
                {
                    int j = i;
                    if (j >= newtypeCount)
                        j = newtypeCount - 1;

                    mapList.Add(
                        new CompareMethodResult
                        {
                            OriginalAssemblyName = map.Item1[i].AssemblyName,
                            OriginalNamespace = map.Item1[i].TypeNamespace,
                            OriginalType = map.Item1[i].TypeName,
                            OriginalMethodName = map.Item1[i].MethodName,
                            NewAssemblyName = j >= 0 ? map.Item2[j].AssemblyName : string.Empty,
                            NewNamespace = j >= 0 ? map.Item2[j].TypeNamespace : string.Empty,
                            NewType = j >= 0 ? map.Item2[j].TypeName : string.Empty,
                            NewMethodName = j >= 0 ? map.Item2[j].MethodName : null
                        }
                    );
                }
                if (newtypeCount > oldtypeCount)
                {
                    for (int j = oldtypeCount; j < newtypeCount; j++)
                    {
                        mapList.Add(
                           new CompareMethodResult
                           {
                               OriginalAssemblyName = map.Item1[oldtypeCount - 1].AssemblyName,
                               OriginalNamespace = map.Item1[oldtypeCount - 1].TypeNamespace,
                               OriginalType = map.Item1[oldtypeCount - 1].TypeName,
                               OriginalMethodName = map.Item1[oldtypeCount - 1].MethodName,
                               NewAssemblyName = map.Item2[j].AssemblyName,
                               NewNamespace = map.Item2[j].TypeNamespace,
                               NewType = map.Item2[j].TypeName,
                               NewMethodName = map.Item2[j].MethodName
                           }
                       );
                    }
                }
            }

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
            return result;
        }
    }
}
