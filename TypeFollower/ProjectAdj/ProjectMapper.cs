using Common.Logging;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Newtonsoft.Json;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml;

namespace ProjectAdj
{
    public class ProjectMapper
    {
        string _sourceProjectPath;
        string _destinationFolder;
        string _mapFilePath;

        List<CompareTypeResult> _listTypeMap;
        List<CompareMethodResult> _listMethodMap;

        ILog Logger = LogManager.GetLogger(typeof(ProjectMapper));

        public ProjectMapper(string sourcePath, string destinationFolder, string mapFilePath)
        {
            _sourceProjectPath = sourcePath;
            _destinationFolder = destinationFolder;
            _mapFilePath = mapFilePath;

            LoadTypeMap();
        }

        public void MapProject(string nugetApiEndpoint)
        {
            try
            {
                DirectoryInfo destFolder = Directory.CreateDirectory(_destinationFolder);
                MSBuildLocator.RegisterDefaults();
                string sourceDir = Path.GetDirectoryName(_sourceProjectPath);
                CopyFolder(sourceDir, _destinationFolder);
                string destProjPath = Path.Combine(_destinationFolder, Path.GetFileName(_sourceProjectPath));
                ReplaceRenamedStaticMethodReferences(destProjPath);
                ReplaceRenamedTypeReferences(destProjPath);
                ReplaceNugetPackages(destProjPath, nugetApiEndpoint);
            }
            catch (Exception ex)
            {
                LogFullExceptionTree(ex);
            }
        }

        private void LogFullExceptionTree(Exception ex)
        {
            Logger.Error(ex.Message);
            Exception innerException = ex.InnerException;
            while (innerException != null)
            {
                Logger.Error(innerException.Message);
                innerException = innerException.InnerException;
            }
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Logger.Error(e.Diagnostic.Message);
        }

        private void CheckInput()
        {
            if (!File.Exists(_sourceProjectPath))
                throw new Exception($"Source project file {_sourceProjectPath} not found!");

            if (!File.Exists(_mapFilePath))
                throw new Exception($"Mapping file {_mapFilePath} not found!");
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            foreach (string dir in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(destFolder, dir.Substring(sourceFolder.Length + 1)));
            }

            foreach (string file_name in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
            {
                File.Copy(file_name, Path.Combine(destFolder, file_name.Substring(sourceFolder.Length + 1)), true);
            }
        }

        private void LoadTypeMap()
        {
            var paths = File.ReadAllLines(_mapFilePath);
            if (paths.Any())
            {
                string typeMapJson = paths.First();
                _listTypeMap = JsonConvert.DeserializeObject<List<CompareTypeResult>>(typeMapJson);
                if (paths.Count() > 1)
                {
                    string methodMapJson = paths[1];
                    _listMethodMap = JsonConvert.DeserializeObject<List<CompareMethodResult>>(methodMapJson);
                }
            }
        }

        private void ReplaceRenamedTypeReferences(string destProjPath)
        {
            Logger.Debug($"Replace renamed type usage...");
            foreach (var comparation in _listTypeMap.Where(c => c.IsChanged()))
            {
                try
                {
                    using (var workspace = MSBuildWorkspace.Create())
                    {
                        workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                        var projectDestination = workspace.OpenProjectAsync(destProjPath).Result;
                        Solution solution = projectDestination.Solution;
                        var compilation = projectDestination.GetCompilationAsync().Result;
                        var classTypeOriginal = compilation.GetTypeByMetadataName($"{comparation.OriginalNamespace}.{comparation.OriginalType}");
                        if (classTypeOriginal != null)
                        {
                            solution = Renamer.RenameSymbolAsync(solution, classTypeOriginal, comparation.NewType, solution.Options).Result;
                            var result = workspace.TryApplyChanges(solution);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogFullExceptionTree(ex);
                }
            }

            Logger.Debug($"Adjusting using directives to renamed types...");
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                var projectDestination = workspace.OpenProjectAsync(destProjPath).Result;
                Solution solution = projectDestination.Solution;
                foreach (var doc in projectDestination.Documents.Where(d => d.SourceCodeKind == SourceCodeKind.Regular && d.SupportsSyntaxTree && d.FilePath.StartsWith(Path.GetDirectoryName(d.Project.FilePath))))
                {
                    Logger.Debug($"Processing source file: {doc.FilePath}");
                    var syntaxTree = doc.GetSyntaxTreeAsync().Result;
                    var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
                    foreach (var comparation in _listTypeMap.Where(c => c.IsChanged()))
                    {
                        bool bRootChanged = false;
                        for (int i = 0; i < root.Usings.Count; i++)
                        {
                            var usng = root.Usings[i];
                            if (usng.Name.ToString() == comparation.OriginalNamespace)
                            {
                                var newusng = usng.WithName(SyntaxFactory.ParseName(comparation.NewNamespace));
                                root = root.ReplaceNode(usng, newusng);
                                bRootChanged = true;
                                break;
                            }
                        }

                        if (!bRootChanged)
                            solution = solution.WithDocumentSyntaxRoot(doc.Id, root);
                    }
                }
                var result = workspace.TryApplyChanges(solution);
            }
        }

        private void ReplaceRenamedStaticMethodReferences(string destProjPath)
        {
            if (_listMethodMap != null)
            {
                using (var workspace = MSBuildWorkspace.Create())
                {
                    workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                    var projectDestination = workspace.OpenProjectAsync(destProjPath).Result;
                    Solution solution = projectDestination.Solution;
                    Logger.Debug($"Replace moved static method usage");
                    foreach (var doc in projectDestination.Documents.Where(d => d.SourceCodeKind == SourceCodeKind.Regular && d.SupportsSyntaxTree && d.FilePath.StartsWith(Path.GetDirectoryName(d.Project.FilePath))))
                    {
                        Logger.Debug($"Processing source file: {doc.FilePath}...");
                        var syntaxTree = doc.GetSyntaxTreeAsync().Result;
                        var root = syntaxTree.GetRoot();
                        var rewriter = new StaticMethodInvocationRewriter(_listMethodMap);
                        var newnode = rewriter.Visit(root);
                        solution = solution.WithDocumentSyntaxRoot(doc.Id, newnode);
                    }

                    var result = workspace.TryApplyChanges(solution);
                }
            }
        }

        private void ReplaceNugetPackages(string destProjPath, string repositoryApiEndpoint)
        {
            string rootPath = GetAbsolutePathFromRelativePath(Path.GetDirectoryName(destProjPath));
            string packageInstallPath = GetAbsolutePathFromRelativePath($"{rootPath}\\packages");

            foreach (var comparation in _listMethodMap.Where(c => c.IsChanged()))
            {
                try
                {
                    if (comparation.OriginalAssemblyName != comparation.NewAssemblyName)
                    {
                        string packageName = comparation.NewAssemblyName;
                        IPackage packageDownloaded = DownloadNugetPackageV2(packageName, repositoryApiEndpoint, packageInstallPath);
                        if (packageDownloaded != null)
                        {
                            UpdatePackagesConfigFile($"{rootPath}\\packages.config", packageDownloaded, comparation.OriginalAssemblyName);
                            RemoveReferenceFromProject(destProjPath, $"{comparation.OriginalAssemblyName}");
                            AddReferenceToProject(destProjPath, packageInstallPath, packageDownloaded);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogFullExceptionTree(ex);
                }
            }
        }

        private IPackage DownloadNugetPackageV2(string packageName, string repositoryApiEndpointUrl, string packageInstallPath)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryApiEndpointUrl);
            List<IPackage> packages = repo.FindPackagesById(packageName).ToList();

            var nugetFound = packages.FirstOrDefault();
            if (nugetFound != null)
            {
                Logger.Debug($"Downloading {nugetFound.GetFullName()} package...");

                PackageManager packageManager = new PackageManager(repo, packageInstallPath);

                packageManager.InstallPackage(packageName, nugetFound.Version);

                Logger.Debug($"{nugetFound.Id} nuget package downloaded");
            }

            return nugetFound;
        }

        private void UpdatePackagesConfigFile(string configPath, IPackage package, string originalPackageName)
        {
            Logger.Debug($"Adding {package.Id} to 'packages.config'");

            var packageReferenceFile = new PackageReferenceFile(configPath);

            // Get the target framework of the current project to add --> targetframework="net452" attribute in the package.config file
            var currentTargetFw = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
            var targetFrameworkAttribute = ((TargetFrameworkAttribute[])currentTargetFw).FirstOrDefault();
            packageReferenceFile.AddEntry(package.Id, SemanticVersion.Parse(package.Version.ToFullString()), false, new FrameworkName(targetFrameworkAttribute.FrameworkName));

            Logger.Debug($"Removing {originalPackageName} from 'packages.config'");
            packageReferenceFile.DeleteEntry(originalPackageName, null);

            Logger.Debug($"'packages.config' updated");
        }

        private void RemoveReferenceFromProject(string projectFilePath, string referenceName)
        {
            var projectFileXmlDoc = new XmlDocument();
            projectFileXmlDoc.Load(projectFilePath);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(projectFileXmlDoc.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
            var referenceNodes = projectFileXmlDoc.SelectNodes("//x:Project/x:ItemGroup/x:Reference", nsmgr);
            foreach (XmlNode node in referenceNodes)
            {
                if (node.Attributes["Include"].Value.Contains(referenceName))
                    node.ParentNode.RemoveChild(node);
            }
            projectFileXmlDoc.Save(projectFilePath);
        }

        private void AddReferenceToProject(string projectPath, string packageInstallPath, IPackage packageDownloaded)
        {
            string path = $"{packageInstallPath}\\{packageDownloaded.Id}.{packageDownloaded.Version}";
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            string assemblyToAdd = files.FirstOrDefault(f => Path.GetFileName(f) == $"{packageDownloaded.Id}.dll");
            if (!string.IsNullOrEmpty(assemblyToAdd))
            {
                using (var workspace = MSBuildWorkspace.Create())
                {
                    workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
                    Project project = workspace.OpenProjectAsync(projectPath).Result;
                    MetadataReference reference = MetadataReference.CreateFromFile(assemblyToAdd);
                    if (reference != null)
                    {
                        var projModified = project.AddMetadataReference(reference);
                        var res = workspace.TryApplyChanges(projModified.Solution);
                    }
                }
                Logger.Debug($"{packageDownloaded.Id}.dll successfully added as reference to {Path.GetFileNameWithoutExtension(projectPath)} project");
            }
            else
            {
                Logger.Error($"{packageDownloaded.Id}.dll was not found!");
            }
        }

        //private async Task AddNugetPackage(string packageName, string packageSrcUrl, string projectPath)
        //{
        //	NuGetLogger log = new NuGetLogger(Logger);

        //	PackageSource packageSource = new PackageSource(packageSrcUrl);
        //	List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
        //	providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
        //	//  providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
        //	//	SourceRepository sourceRepository = Repository.Factory.GetCoreV2(packageSource);
        //	SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
        //	PackageMetadataResource packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
        //	SourceCacheContext cacheContext = new SourceCacheContext();
        //	cacheContext.IgnoreFailedSources = true;
        //	IEnumerable<IPackageSearchMetadata> searchMetadata = await packageMetadataResource.GetMetadataAsync(packageName, true, true, cacheContext, log, CancellationToken.None);
        //	Logger.Debug("Nuget package search finished");

        //	var nugetFound = searchMetadata.FirstOrDefault();
        //	if (nugetFound != null)
        //	{
        //		ISourceRepositoryProvider sourceRepositoryProvider = Repository.CreateProvider(providers.Select(p => p.Value));
        //		string rootPath = GetAbsolutePathFromRelativePath(Path.GetDirectoryName(projectPath));
        //		string packagesPath = GetAbsolutePathFromRelativePath($"{rootPath}\\..\\packages");
        //		ISettings settings = Settings.LoadDefaultSettings(rootPath, null, new MachineWideSettings());
        //		NuGetProject projectNuget = new FolderNuGetProject(rootPath);
        //		NuGetPackageManager packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, packagesPath)
        //		{
        //			PackagesFolderNuGetProject = (FolderNuGetProject)projectNuget
        //		};
        //		bool allowPrereleaseVersions = true;
        //		bool allowUnlisted = false;
        //		ResolutionContext resolutionContext = new ResolutionContext(
        //			DependencyBehavior.Lowest, allowPrereleaseVersions, allowUnlisted, VersionConstraints.None);
        //		INuGetProjectContext projectContext = new ProjectContext(log);
        //		IEnumerable<SourceRepository> sourceRepositories = new List<SourceRepository>() { sourceRepository };  // See part 2
        //		PackageDownloadContext downloadContext = new PackageDownloadContext(resolutionContext.SourceCacheContext);
        //		downloadContext.ParentId = projectContext.OperationId;
        //		downloadContext.ClientPolicyContext = ClientPolicyContext.GetClientPolicy(settings, log);

        //		Logger.Debug($"Starting to install {nugetFound.Identity} nuget package");

        //		//Task install = packageManager.InstallPackageAsync(packageManager.PackagesFolderNuGetProject,
        //		//	nugetFound.Identity, resolutionContext, projectContext, downloadContext, sourceRepositories,
        //		//	Array.Empty<SourceRepository>(),  // This is a list of secondary source respositories, probably empty
        //		//	CancellationToken.None);

        //		//install.Wait(50000);
        //		Logger.Debug($"{nugetFound.Title} nuget package installed");
        //	}
        //	else
        //	{
        //		Logger.Debug($"Nuget {packageName} not found");
        //	}
        //}

        //private async void InstallPackageIntoProject(INuGetProjectContext projectContext, NuGetPackageManager packageManager, IPackageSearchMetadata nugetFound, 
        //	ResolutionContext resolutionContext, IEnumerable<SourceRepository> sourceRepositories, ISettings settings, NuGetLogger log)
        //{
        //	ISolutionManager SolutionManager = null;

        //	ActivityCorrelationId.StartNew();

        //	// Step-1 : Call PreviewInstallPackageAsync to get all the nuGetProjectActions
        //	var nuGetProjectActions = await packageManager.PreviewInstallPackageAsync(packageManager.PackagesFolderNuGetProject, nugetFound.Identity, resolutionContext,
        //		projectContext, sourceRepositories, Array.Empty<SourceRepository>(), CancellationToken.None);

        //	NuGetPackageManager.SetDirectInstall(nugetFound.Identity, projectContext);

        //	var projects = new List<NuGetProject>() { packageManager.PackagesFolderNuGetProject };

        //	// find out build integrated projects so that we can arrange them in reverse dependency order
        //	var buildIntegratedProjectsToUpdate = projects.OfType<BuildIntegratedNuGetProject>().ToList();

        //	// order won't matter for other type of projects so just add rest of the projects in result
        //	var sortedProjectsToUpdate = projects.Except(buildIntegratedProjectsToUpdate).ToList();

        //	Dictionary<string, bool> _buildIntegratedProjectsUpdateDict = null;
        //	if (buildIntegratedProjectsToUpdate.Count > 0)
        //	{
        //		var logger = new ProjectContextLogger(projectContext);
        //		var referenceContext = new DependencyGraphCacheContext(logger, settings);
        //		_buildIntegratedProjectsUpdateDict = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        //		var projectUniqueNamesForBuildIntToUpdate
        //			= buildIntegratedProjectsToUpdate.ToDictionary((project) => project.MSBuildProjectPath);

        //		// get all build integrated projects of the solution which will be used to map project references
        //		// of the target projects
        //		var allBuildIntegratedProjects =
        //			(await SolutionManager.GetNuGetProjectsAsync()).OfType<BuildIntegratedNuGetProject>().ToList();

        //		var dgFile = await DependencyGraphRestoreUtility.GetSolutionRestoreSpec(SolutionManager, referenceContext);
        //		DependencyGraphSpec _buildIntegratedProjectsCache = dgFile;
        //		var allSortedProjects = DependencyGraphSpec.SortPackagesByDependencyOrder(dgFile.Projects);

        //		var msbuildProjectPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //		foreach (var projectUniqueName in allSortedProjects.Select(e => e.RestoreMetadata.ProjectUniqueName))
        //		{
        //			BuildIntegratedNuGetProject project;
        //			if (projectUniqueNamesForBuildIntToUpdate.TryGetValue(projectUniqueName, out project))
        //			{
        //				sortedProjectsToUpdate.Add(project);
        //			}
        //		}

        //		// cache these projects which will be used to avoid duplicate restore as part of parent projects
        //		_buildIntegratedProjectsUpdateDict.AddRange(
        //			buildIntegratedProjectsToUpdate.Select(child => new KeyValuePair<string, bool>(child.MSBuildProjectPath, false)));
        //	}

        //	FolderNuGetProject PackagesFolderNuGetProject = packageManager.PackagesFolderNuGetProject;//new FolderNuGetProject(packagesFolderPath, resolver);
        //	SourceRepository PackagesFolderSourceRepository = packageManager.PackagesFolderSourceRepository;//SourceRepositoryProvider.CreateRepository(new PackageSource(packagesFolderPath), feedType);

        //	// execute all nuget project actions
        //	foreach (var project in sortedProjectsToUpdate)
        //	{
        //		var nugetActions = nuGetProjectActions.Where(action => action.Project.Equals(project));
        //		var downCont = new PackageDownloadContext(resolutionContext.SourceCacheContext)
        //		{
        //			ParentId = projectContext.OperationId,
        //			ClientPolicyContext = ClientPolicyContext.GetClientPolicy(settings, log)
        //		};

        //		if (project == null)
        //		{
        //			throw new ArgumentNullException(nameof(project));
        //		}

        //		if (nuGetProjectActions == null)
        //		{
        //			throw new ArgumentNullException(nameof(nuGetProjectActions));
        //		}

        //		if (projectContext == null)
        //		{
        //			throw new ArgumentNullException(nameof(projectContext));
        //		}

        //		var stopWatch = Stopwatch.StartNew();

        //		ExceptionDispatchInfo exceptionInfo = null;

        //		// DNU: Find the closure before executing the actions
        //		var buildIntegratedProject = project as BuildIntegratedNuGetProject;
        //		if (buildIntegratedProject != null)
        //		{
        //			await packageManager.ExecuteBuildIntegratedProjectActionsAsync(buildIntegratedProject,
        //				nuGetProjectActions,
        //				projectContext,
        //				CancellationToken.None);
        //		}
        //		else
        //		{
        //			// Set the original packages config if it exists
        //			var msbuildProject = project as MSBuildNuGetProject;
        //			if (msbuildProject != null)
        //			{
        //				projectContext.OriginalPackagesConfig =
        //					msbuildProject.PackagesConfigNuGetProject?.GetPackagesConfig();
        //			}

        //			var executedNuGetProjectActions = new Stack<NuGetProjectAction>();
        //			var packageWithDirectoriesToBeDeleted = new HashSet<PackageIdentity>(PackageIdentity.Comparer);
        //			var ideExecutionContext = projectContext.ExecutionContext as IDEExecutionContext;
        //			if (ideExecutionContext != null)
        //			{
        //				await ideExecutionContext.SaveExpandedNodeStates(SolutionManager);
        //			}

        //			var logger = new ProjectContextLogger(projectContext);
        //			Dictionary<PackageIdentity, PackagePreFetcherResult> downloadTasks = null;
        //			CancellationTokenSource downloadTokenSource = null;

        //			// batch events argument object
        //			PackageProjectEventArgs packageProjectEventArgs = null;

        //			try
        //			{
        //				// PreProcess projects
        //				await project.PreProcessAsync(projectContext, CancellationToken.None);

        //				var actionsList = nuGetProjectActions.ToList();

        //				var hasInstalls = actionsList.Any(action =>
        //					action.NuGetProjectActionType == NuGetProjectActionType.Install);

        //				if (hasInstalls)
        //				{
        //					// Make this independently cancelable.
        //					downloadTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);

        //					// Download all packages up front in parallel
        //					downloadTasks = await PackagePreFetcher.GetPackagesAsync(
        //						actionsList,
        //						PackagesFolderNuGetProject,
        //						downCont,
        //						SettingsUtility.GetGlobalPackagesFolder(settings),
        //						logger,
        //						downloadTokenSource.Token);

        //					// Log download information
        //					PackagePreFetcher.LogFetchMessages(
        //						downloadTasks.Values,
        //						PackagesFolderNuGetProject.Root,
        //						logger);
        //				}

        //				// raise Nuget batch start event
        //				var batchId = Guid.NewGuid().ToString();
        //				string name;
        //				project.TryGetMetadata(NuGetProjectMetadataKeys.Name, out name);
        //				packageProjectEventArgs = new PackageProjectEventArgs(batchId, name);
        //				/*
        //				BatchStart?.Invoke(this, packageProjectEventArgs);
        //				PackageProjectEventsProvider.Instance.NotifyBatchStart(packageProjectEventArgs);
        //				*/
        //				try
        //				{
        //					if (msbuildProject != null)
        //					{
        //						//start batch processing for msbuild
        //						await msbuildProject.ProjectSystem.BeginProcessingAsync();
        //					}

        //					foreach (var nuGetProjectAction in actionsList)
        //					{
        //						/*
        //						if (nuGetProjectAction.NuGetProjectActionType == NuGetProjectActionType.Uninstall)
        //						{
        //							executedNuGetProjectActions.Push(nuGetProjectAction);

        //							await ExecuteUninstallAsync(nuGetProject,
        //								nuGetProjectAction.PackageIdentity,
        //								packageWithDirectoriesToBeDeleted,
        //								nuGetProjectContext, token);

        //							nuGetProjectContext.Log(
        //								ProjectManagement.MessageLevel.Info,
        //								Strings.SuccessfullyUninstalled,
        //								nuGetProjectAction.PackageIdentity,
        //								nuGetProject.GetMetadata<string>(NuGetProjectMetadataKeys.Name));
        //						}
        //						*/
        //					}
        //				}
        //				finally
        //				{
        //					if (msbuildProject != null)
        //					{
        //						// end batch for msbuild and let it save everything.
        //						// always calls it before PostProcessAsync or binding redirects
        //						await msbuildProject.ProjectSystem.EndProcessingAsync();
        //					}
        //				}

        //				try
        //				{
        //					if (msbuildProject != null)
        //					{
        //						//start batch processing for msbuild
        //						await msbuildProject.ProjectSystem.BeginProcessingAsync();
        //					}

        //					foreach (var nuGetProjectAction in actionsList)
        //					{
        //						if (nuGetProjectAction.NuGetProjectActionType == NuGetProjectActionType.Install)
        //						{
        //							executedNuGetProjectActions.Push(nuGetProjectAction);

        //							// Retrieve the downloaded package
        //							// This will wait on the package if it is still downloading
        //							var preFetchResult = downloadTasks[nuGetProjectAction.PackageIdentity];
        //							using (var downloadPackageResult = await preFetchResult.GetResultAsync())
        //							{
        //								// use the version exactly as specified in the nuspec file
        //								var packageIdentity = await downloadPackageResult.PackageReader.GetIdentityAsync(CancellationToken.None);

        //								try
        //								{
        //									await ExecuteInstallAsync(
        //									project,
        //									packageIdentity,
        //									downloadPackageResult,
        //									packageWithDirectoriesToBeDeleted,
        //									projectContext,
        //									CancellationToken.None,
        //									packageManager);
        //								}
        //								catch (Exception ex)
        //								{
        //									LogFullExceptionTree(ex);
        //									throw ex;
        //								}
        //							}

        //							var identityString = string.Format(CultureInfo.InvariantCulture, "{0} {1}",
        //								nuGetProjectAction.PackageIdentity.Id,
        //								nuGetProjectAction.PackageIdentity.Version.ToNormalizedString());

        //							preFetchResult.EmitTelemetryEvent(projectContext.OperationId);

        //							projectContext.Log(
        //								MessageLevel.Info,
        //								/*Strings.SuccessfullyInstalled*/"{0} successfully installed {1}",
        //								identityString,
        //								project.GetMetadata<string>(NuGetProjectMetadataKeys.Name));
        //						}
        //					}
        //				}
        //				finally
        //				{
        //					if (msbuildProject != null)
        //					{
        //						// end batch for msbuild and let it save everything.
        //						// always calls it before PostProcessAsync or binding redirects
        //						await msbuildProject.ProjectSystem.EndProcessingAsync();
        //					}
        //				}

        //				// Post process
        //				await project.PostProcessAsync(projectContext, CancellationToken.None);

        //				// Open readme file
        //				await OpenReadmeFile(project, projectContext, CancellationToken.None, settings, PackagesFolderNuGetProject);
        //			}
        //			catch (SignatureException ex)
        //			{
        //				var errors = ex.Results.SelectMany(r => r.GetErrorIssues());
        //				var warnings = ex.Results.SelectMany(r => r.GetWarningIssues());
        //				SignatureException unwrappedException = null;

        //				if (errors.Count() == 1)
        //				{
        //					// In case of one error, throw it as the exception
        //					var error = errors.First();
        //					unwrappedException = new SignatureException(error.Code, error.Message, ex.PackageIdentity);
        //				}
        //				else
        //				{
        //					// In case of multiple errors, wrap them in a general NU3000 error
        //					var errorMessage = string.Format(CultureInfo.CurrentCulture,
        //						null,//Strings.SignatureVerificationMultiple,
        //						$"{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(e => e.FormatWithCode()))}");

        //					unwrappedException = new SignatureException(NuGetLogCode.NU3000, errorMessage, ex.PackageIdentity);
        //				}

        //				foreach (var warning in warnings)
        //				{
        //					//projectContext.Log(warning);
        //					logger.LogWarning(warning.Message);
        //				}

        //				exceptionInfo = ExceptionDispatchInfo.Capture(unwrappedException);
        //			}
        //			catch (Exception ex)
        //			{
        //				exceptionInfo = ExceptionDispatchInfo.Capture(ex);
        //			}
        //			finally
        //			{
        //				if (downloadTasks != null)
        //				{
        //					// Wait for all downloads to cancel and dispose
        //					downloadTokenSource.Cancel();

        //					foreach (var result in downloadTasks.Values)
        //					{
        //						await result.EnsureResultAsync();
        //						result.Dispose();
        //					}

        //					downloadTokenSource.Dispose();
        //				}

        //				if (msbuildProject != null)
        //				{
        //					// raise nuget batch end event
        //					if (packageProjectEventArgs != null)
        //					{
        //						/*
        //						BatchEnd?.Invoke(this, packageProjectEventArgs);
        //						PackageProjectEventsProvider.Instance.NotifyBatchEnd(packageProjectEventArgs);
        //						*/
        //					}
        //				}
        //			}

        //			if (exceptionInfo != null)
        //			{
        //				//await packageManager.RollbackAsync(project, executedNuGetProjectActions, packageWithDirectoriesToBeDeleted, projectContext, CancellationToken.None);
        //			}

        //			if (ideExecutionContext != null)
        //			{
        //				await ideExecutionContext.CollapseAllNodes(SolutionManager);
        //			}

        //			// Delete the package directories as the last step, so that, if an uninstall had to be rolled back, we can just use the package file on the directory
        //			// Also, always perform deletion of package directories, even in a rollback, so that there are no stale package directories
        //			/*
        //			foreach (var packageWithDirectoryToBeDeleted in packageWithDirectoriesToBeDeleted)
        //			{
        //				var packageFolderPath = PackagesFolderNuGetProject.GetInstalledPath(packageWithDirectoryToBeDeleted);
        //				try
        //				{
        //					await DeletePackageAsync(packageWithDirectoryToBeDeleted, nuGetProjectContext, token);
        //				}
        //				finally
        //				{
        //					if (DeleteOnRestartManager != null)
        //					{
        //						if (Directory.Exists(packageFolderPath))
        //						{
        //							DeleteOnRestartManager.MarkPackageDirectoryForDeletion(
        //								packageWithDirectoryToBeDeleted,
        //								packageFolderPath,
        //								nuGetProjectContext);

        //							// Raise the event to notify listners to update the UI etc.
        //							DeleteOnRestartManager.CheckAndRaisePackageDirectoriesMarkedForDeletion();
        //						}
        //					}
        //				}
        //			}
        //			*/
        //			// Save project
        //			await project.SaveAsync(CancellationToken.None);

        //			// Clear direct install
        //			NuGetPackageManager.SetDirectInstall(null, projectContext);
        //		}


        //		// calculate total time taken to execute all nuget actions
        //		stopWatch.Stop();
        //		projectContext.Log(
        //			MessageLevel.Info, /*Strings.NugetActionsTotalTime*/"Ellapsed time hacked log string....",
        //			DatetimeUtility.ToReadableTimeFormat(stopWatch.Elapsed));

        //		// emit resolve actions telemetry event
        //		var actionTelemetryEvent = new ActionTelemetryStepEvent(
        //			projectContext.OperationId.ToString(),
        //			TelemetryConstants.ExecuteActionStepName, stopWatch.Elapsed.TotalSeconds);

        //		TelemetryActivity.EmitTelemetryEvent(actionTelemetryEvent);

        //		if (exceptionInfo != null)
        //		{
        //			exceptionInfo.Throw();
        //		}
        //	}

        //	// clear cache which could temper with other updates
        //	_buildIntegratedProjectsUpdateDict?.Clear();
        //	//_buildIntegratedProjectsCache = null;
        //	//_restoreProviderCache = null;

        //	NuGetPackageManager.ClearDirectInstall(projectContext);
        //}

        //private async Task ExecuteInstallAsync(
        //	NuGetProject nuGetProject,
        //	PackageIdentity packageIdentity,
        //	DownloadResourceResult resourceResult,
        //	HashSet<PackageIdentity> packageWithDirectoriesToBeDeleted,
        //	INuGetProjectContext nuGetProjectContext,
        //	CancellationToken token,
        //	NuGetPackageManager packageManager)
        //{
        //	// TODO: EnsurePackageCompatibility check should be performed in preview. Can easily avoid a lot of rollback
        //	await packageManager.InstallationCompatibility.EnsurePackageCompatibilityAsync(nuGetProject, packageIdentity, resourceResult, token);

        //	packageWithDirectoriesToBeDeleted.Remove(packageIdentity);

        //	await nuGetProject.InstallPackageAsync(packageIdentity, resourceResult, nuGetProjectContext, token);
        //}

        //private Task OpenReadmeFile(NuGetProject nuGetProject, INuGetProjectContext nuGetProjectContext, CancellationToken token, ISettings settings, FolderNuGetProject PackagesFolderNuGetProject)
        //{
        //	var executionContext = nuGetProjectContext.ExecutionContext;
        //	if (executionContext != null
        //		&& executionContext.DirectInstall != null)
        //	{
        //		//packagesPath is different for project.json vs Packages.config scenarios. So check if the project is a build-integrated project
        //		var buildIntegratedProject = nuGetProject as BuildIntegratedNuGetProject;
        //		var readmeFilePath = string.Empty;

        //		if (buildIntegratedProject != null)
        //		{
        //			var pathContext = NuGetPathContext.Create(settings);
        //			var pathResolver = new FallbackPackagePathResolver(pathContext);
        //			var identity = nuGetProjectContext.ExecutionContext.DirectInstall;
        //			var packageFolderPath = pathResolver.GetPackageDirectory(identity.Id, identity.Version);

        //			if (!string.IsNullOrEmpty(packageFolderPath))
        //			{
        //				readmeFilePath = Path.Combine(packageFolderPath, Constants.ReadmeFileName);
        //			}
        //		}
        //		else
        //		{
        //			var packagePath = PackagesFolderNuGetProject.GetInstalledPackageFilePath(executionContext.DirectInstall);

        //			if (!string.IsNullOrEmpty(packagePath))
        //			{
        //				readmeFilePath = Path.Combine(Path.GetDirectoryName(packagePath), Constants.ReadmeFileName);
        //			}
        //		}

        //		if (!token.IsCancellationRequested &&
        //			!string.IsNullOrEmpty(readmeFilePath) &&
        //			File.Exists(readmeFilePath))
        //		{
        //			return executionContext.OpenFile(readmeFilePath);
        //		}
        //	}

        //	return Task.FromResult(false);
        //}

        private string GetAbsolutePathFromRelativePath(string relativePath)
        {
            string absolutePath = relativePath;
            if (!(!string.IsNullOrEmpty(relativePath) && (relativePath.StartsWith("\\") || relativePath.IndexOf(':') == 1)))
            {
                string assemblyFolder = Assembly.GetExecutingAssembly().Location;
                FileInfo fiAssemblyFolder = new FileInfo(assemblyFolder);
                absolutePath = Path.Combine(fiAssemblyFolder.Directory.FullName, relativePath);
            }
            return absolutePath;
        }
    }
}
