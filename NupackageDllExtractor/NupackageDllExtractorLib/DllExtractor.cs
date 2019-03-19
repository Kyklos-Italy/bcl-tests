using Common.Logging;
using NupackageDllExtractorLib.FileUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NupackageDllExtractorLib
{
	public class DllExtractor
	{
		string _sourceFolder;
		string _destinationFolder;
		bool _bcleanDestination = false;
		string _customFolderFilter;
		bool _blastVersionFilter = true;
		string _versionFilter;

		ILog Logger = LogManager.GetLogger(typeof(DllExtractor));

		public DllExtractor(string sourceFolder, string destinationFolder, bool bcleanDestination, string customFolderFilter, string versionFilter)
		{
			_sourceFolder = sourceFolder;
			_destinationFolder = destinationFolder;
			_bcleanDestination = bcleanDestination;
			_customFolderFilter = customFolderFilter;
			_versionFilter = versionFilter;
			if(!string.IsNullOrEmpty(_versionFilter))
				_blastVersionFilter = false;
		}

		public string Extract()
		{
			string errorMessage = CheckApplyConditionsAreMet();
			if (string.IsNullOrEmpty(errorMessage))
			{
				IList<string> nupkgFiles = GetAllNupkgFilesInSourceFolder();
				if (nupkgFiles.Count == 0)
				{
					errorMessage = "No nupkg files found in Source folder and its subdirectories";
					Logger.Error(errorMessage);
				}
				else
				{
					nupkgFiles = ApplyFiltersToNupkgFiles2(nupkgFiles, _customFolderFilter);
					if (nupkgFiles.Count == 0)
					{
						errorMessage = $"No nupkg files found in Source folder and its subdirectories for folder filter: {_customFolderFilter}";
						Logger.Error(errorMessage);
					}
					else
					{
						HandleDestinationDirectory();
						IList<string> duplicatedNupkgFiles = GetDuplicatesInNupkgFiles(nupkgFiles);
						if (duplicatedNupkgFiles.Count > 0)
						{
							foreach (string duplicatedNupkgFile in duplicatedNupkgFiles)
							{
								Logger.Debug($"found duplicate NupkgFile: {duplicatedNupkgFile}");
							}
							errorMessage = $"Some nuget packages files are duplicated, please check log";
						}
						else
						{
							errorMessage = ExtractNupkgFiles(nupkgFiles);
						}
					}
				}
			}
			return errorMessage;
		}

		private string CheckApplyConditionsAreMet()
		{
			if (string.IsNullOrEmpty(_sourceFolder))
			{
				string msg = "Please fill source folder";
				Logger.Error(msg);
				return msg;
			}
			else
			{
				if (!Directory.Exists(_sourceFolder))
				{
					string msg = "Source folder not found";
					Logger.Error(msg);
					return msg;
				}
				else
				{
					if (string.IsNullOrEmpty(_destinationFolder))
					{
						string msg = "Please fill destination folder";
						Logger.Error(msg);
						return msg;
					}
					else
					{
						if (string.IsNullOrEmpty(_customFolderFilter))
						{
							string msg = "Please fill custom Source Folder filter";
							Logger.Error(msg);
							return msg;
						}
						else
						{
							if (!_blastVersionFilter && string.IsNullOrEmpty(_versionFilter))
							{
								string msg = "Please fill version textbox";
								Logger.Error(msg);
								return msg;
							}
						}
					}
				}
				return "";
			}
		}

		private IList<string> GetAllNupkgFilesInSourceFolder()
		{
			IList<string> nupkgFiles = FileSystemUtils.GetFilesInFolderByExtension(_sourceFolder, "*.nupkg", SearchOption.AllDirectories);
			return nupkgFiles;
		}

		private IList<string> ApplyFiltersToNupkgFiles2(IList<string> nupkgFiles, string filterFolder)
		{
			nupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, filterFolder);
			return nupkgFiles;
		}

		private void HandleDestinationDirectory()
		{
			if (!Directory.Exists(_destinationFolder))
			{
				Directory.CreateDirectory(_destinationFolder);
			}
			if (_bcleanDestination)
			{
				FileSystemUtils.CleanDirectory(_destinationFolder);
			}
		}

		private IList<string> GetDuplicatesInNupkgFiles(IList<string> nupkgFiles)
		{
			IList<string> duplicatenupkgFiles = new List<string>();
			List<string> duplicateNupkgFileNames = new List<string>();
			List<IGrouping<string, string>> duplicatedNupkgFiles = nupkgFiles.GroupBy(x => Path.GetFileName(x))
				.Where(x => x.Count() > 1).ToList();
			foreach (IGrouping<string, string> duplicateNupkgFile in duplicatedNupkgFiles)
			{
				duplicateNupkgFileNames.AddRange(duplicatedNupkgFiles.Select(x => x.Key).ToList());
			}
			foreach (string nupkgFile in nupkgFiles)
			{
				foreach (string duplicateNupkgFileName in duplicateNupkgFileNames)
				{
					if (nupkgFile.Contains(duplicateNupkgFileName))
					{
						duplicatenupkgFiles.Add(nupkgFile);
						break;
					}
				}
			}
			return duplicatenupkgFiles;
		}

		private string ExtractNupkgFiles(IList<string> nupkgFiles)
		{
			string msg = string.Empty;
			if (_blastVersionFilter)
			{
				nupkgFiles = FileSystemUtils.ExtractNukpgFilesByLastVersionOfEachPackage(nupkgFiles, _destinationFolder);
				msg = "Extraction completed successfully";
				Logger.Debug(msg);
			}
			else
			{
				IList<string> filteredNupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, _versionFilter);
				if (filteredNupkgFiles.Count() == 0)
				{
					msg = "No packages found";
					Logger.Debug(msg);
				}
				else
				{
					FileSystemUtils.ExtractNukpgFilesByVersion(filteredNupkgFiles, _destinationFolder, _versionFilter);
					msg = "Extraction completed successfully";
					Logger.Debug(msg);
				}
			}
			return msg;
		}
	}
}
