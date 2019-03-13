using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kyklos.Kernel.Compression;
using Kyklos.Kernel.Compression.Zip;

namespace NupackageDllExtractorLib.FileUtils
{
    public static class FileSystemUtils
    {
        public static IList<string> GetFilesInFolderByExtension(string folder, string fileExtension, SearchOption searchOption)
        {
            List<string> filesFound;
            try
            {
                filesFound = Directory.GetFiles(folder, fileExtension, searchOption).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return filesFound;
        }

        public static async Task<List<string>> ExtractNupkgDllsToOutputDirectory(string nupkgFilePath, string outputFolder)
        {
            List<string> totalextractedDllPathFiles = new List<string>();
            string[] extractedDllPathFiles = await UnZipper.ExtractFlatFilesFromZipFile(nupkgFilePath, outputFolder, "*.dll").ConfigureAwait(false);
            for (var i = 0; i < extractedDllPathFiles.Length; i++)
            {
                totalextractedDllPathFiles.Add(extractedDllPathFiles[i]);
            }
            return totalextractedDllPathFiles;
        }

        public static string GetNewNupkgFileNameFromVersion(string nupkgFilePath, string version)
        {
            string newNupkgFileName = "";
            string nupkgExtension = Path.GetExtension(nupkgFilePath);
            string packageName = GetNupkgNameFromFilePath(nupkgFilePath);
            newNupkgFileName = packageName + version + nupkgExtension;
            return newNupkgFileName;
        }

        public static void RenameDllToSemVersion(IList<string> dllPathFiles, string destinationFolder, string nupkgPathFile, string filterVersion)
        {
            string newPackagePathFile = Path.Combine(destinationFolder, FileSystemUtils.GetNewNupkgFileNameFromVersion(nupkgPathFile, filterVersion));
            for (var j = 0; j < dllPathFiles.Count; j++)
            {
                string dllFileExtension = Path.GetExtension(dllPathFiles[j]);
                newPackagePathFile = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(newPackagePathFile) + dllFileExtension);
                File.Move(dllPathFiles[j], newPackagePathFile);
            }
        }

        public static IList<string> FilterNukpgFilesByFilter(IList<string> nupkgFiles, string filter)
        {
            IList<string> FilteredNupkgFiles = nupkgFiles.Where(x => x.ToLower().Contains(filter.ToLower())).ToList();
            return FilteredNupkgFiles;            
        }

        public static IList<string> GetNukpgPackageNames(IList<string> nupkgFiles)
        {
            List<string> packageNames = new List<string>();
            for (var i = 0; i < nupkgFiles.Count; i++)
            {
                string packageName = GetNupkgNameFromFilePath(nupkgFiles[i]);
                if (!packageNames.Contains(packageName))
                {
                    packageNames.Add(packageName);
                }
            }
            return packageNames;
        }

        public static IList<string> ExtractNukpgFilesByLastVersionOfEachPackage(IList<string> nupkgFiles, string destinationFolder)
        {
            IList<string> nupkgFilesFilteredByLastVersion = new List<string>();
            IList<string> packageNames = GetNukpgPackageNames(nupkgFiles);
            foreach (string packageName in packageNames)
            {
                IList<string> nupkgFilesOfPackage = FilterNukpgFilesByFilter(nupkgFiles, packageName);
                string lastVersionOfPackage = SemVersionUtils.GetLastVersionOfNugetPackages(nupkgFilesOfPackage);
                string nupkgFilesOfLastVersion = FilterNukpgFilesByFilter(nupkgFilesOfPackage, lastVersionOfPackage).First();
                nupkgFilesFilteredByLastVersion.Add(nupkgFilesOfLastVersion);
                ExtractDllToOutputFolder(nupkgFilesOfLastVersion, lastVersionOfPackage, destinationFolder);
            }
            return nupkgFilesFilteredByLastVersion;
        }

        public static void ExtractNukpgFilesByVersion(IList<string> nupkgFiles, string destinationFolder, string filterVersion)
        {
            foreach (string nupkgFile in nupkgFiles)
            {
                ExtractDllToOutputFolder(nupkgFile, filterVersion, destinationFolder);
            }
        }

        public static string GetNupkgNameFromFilePath(string nupkgFilePath)
        {
            string packageName = "";
            string nupkgFileName = Path.GetFileNameWithoutExtension(nupkgFilePath);
            string semVersionOfCurrentNupkg = SemVersionUtils.GetValidSemVersion(nupkgFileName).ToString().Replace('+', '.');
            packageName = nupkgFileName.Replace(semVersionOfCurrentNupkg, "");
            return packageName;
        }

        private static void ExtractDllToOutputFolder(string nupkgPathFile, string filterVersion, string destinationFolder)
        {
            List<string> dllPathFiles = ExtractNupkgDllsToOutputDirectory(nupkgPathFile, destinationFolder).Result;
            RenameDllToSemVersion(dllPathFiles, destinationFolder, nupkgPathFile, filterVersion);
        }

        public static void CleanDirectory(string folderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directoryInfo.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}

