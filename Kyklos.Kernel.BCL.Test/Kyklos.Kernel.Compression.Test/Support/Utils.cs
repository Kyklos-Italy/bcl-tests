using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Kyklos.Kernel.Compression.Test.Support
{
    public static class Utils
    {
        public static Stream GetStreamFromFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            return ms;
        }
        
        public static byte[] ReadBytesFromFile(string path)
        {
            byte[] readBytes = File.ReadAllBytes(path);
            return readBytes;
        }

        public static string ReadStringFromFile(string path)
        {
            string readText = File.ReadAllText(path);
            return readText;
        }

        public static byte[] FileToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public static bool IsZipValid(string path)
        {
            try
            {
                using (var zipFile = ZipFile.OpenRead(path))
                {
                    var entries = zipFile.Entries;
                    return true;
                }
            }
            catch (InvalidDataException ex)
            {
                return false;
            }
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

        public static string CreateDummyFile(string directoryPath, string fileName, bool cleanFolder)
        {
            if (cleanFolder)
            {
                CleanDirectory(directoryPath);
            }
            string actualFilePath = directoryPath + "\\" + fileName;
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
            string actualFolderPath = directoryPath + "\\" + directoryName;
            Directory.CreateDirectory(actualFolderPath);
            actualFolderPath = (Directory.Exists(actualFolderPath)) ? actualFolderPath : "";
            return actualFolderPath;
        }
    }
}
