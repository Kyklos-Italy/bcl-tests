using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyklos.Kernel.Ftp.Test.Support
{
    public static class Utils
    {
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

        public static string CreateDummyFile(string directoryPath, string fileName, bool cleanFolder)
        {
            if (cleanFolder)
            {
                CleanDirectory(directoryPath);
            }
            string actualFilePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(actualFilePath, "contenuto demo");
            actualFilePath = (File.Exists(actualFilePath)) ? actualFilePath : "";
            return actualFilePath;
        }

        public static string CreateDummyDirectory(string directoryPath, string directoryName, bool cleanFolder)
        {
            if (cleanFolder)
            {
                if (Directory.Exists(Path.Combine(directoryPath, directoryName)))
                {
                    CleanDirectory(Path.Combine(directoryPath, directoryName));
                }
            }
            string actualFolderPath = Path.Combine(directoryPath, directoryName);
            Directory.CreateDirectory(actualFolderPath);
            actualFolderPath = (Directory.Exists(actualFolderPath)) ? actualFolderPath : "";
            return actualFolderPath;
        }

        public static string CreateDummyDirectory(string directoryPath, bool cleanFolder)
        {
            if (cleanFolder)
            {
                if (Directory.Exists(directoryPath))
                {
                    CleanDirectory(directoryPath);
                }
            }
            string actualFolderPath = directoryPath;
            Directory.CreateDirectory(actualFolderPath);
            actualFolderPath = (Directory.Exists(actualFolderPath)) ? actualFolderPath : "";
            return actualFolderPath;
        }
    }
}
