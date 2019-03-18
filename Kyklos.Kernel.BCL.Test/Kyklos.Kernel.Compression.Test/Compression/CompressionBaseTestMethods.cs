using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kyklos.Kernel.Compression.Test.Support;
using Kyklos.Kernel.Compression.Test.Support.Mock;
using Kyklos.Kernel.Compression.Zip;
using Kyklos.Kernel.Core.Support;
using Kyklos.Kernel.IO.Async;
using Xunit;
using static Kyklos.Kernel.Compression.Test.Support.Framework;

namespace Kyklos.Kernel.Compression.Test.Compression
{
    public class CompressionBaseTestMethods
    {
        private MockData _mockData;

        public static string GetTempDirectory()
        {
            return Guid.NewGuid().ToStandardString();
        }

        //public static string GetCurrentMethod([CallerMemberName] string method = "")
        //{
        //    return method;
        //}

        public CompressionBaseTestMethods(FrameworkType frameworkType)
        {
            _mockData = new MockData(frameworkType);
        }

        protected void CreateZipFileFromFileListCore()
        {
            string tempDir = GetTempDirectory();
            string dummyFileZip = "dummy.zip";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
            string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
            List<string> files = 
                new List<string>
                {
                    Path.Combine(dummyFolderPath, "dummy1.txt"),
                    Path.Combine(dummyFolderPath, "dummy2.txt"),
                    Path.Combine(dummyFolderPath, "dummy3.txt")
                };
            files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileList(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.True(fileContents.Count == files.Count);
            Utils.RemoveDir(dummyFolderPath);
        }

        protected void CreateZipFileFromFileListWithFileNameOnlyCore()
        {
            string tempDir = GetTempDirectory();
            string dummyFileZip = "dummy.zip";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
            string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
            List<string> files = 
                new List<string>
                {
                    Path.Combine(dummyFolderPath, "dummy1.txt"),
                    Path.Combine(dummyFolderPath, "dummy2.txt"),
                    Path.Combine(dummyFolderPath, "dummy3.txt")
                };
            files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.Equal(fileContents.Count, files.Count);
            Utils.RemoveDir(dummyFolderPath);
        }

        protected async Task ZipStringCore()
        {
            string tempDir = GetTempDirectory();
            string dummyFileName = "dummy.txt";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
            Utils.CreateDummyFile(dummyFolderPath, dummyFileName, cleanFolder: true);
            string originalString = await KFile.ReadAllTextAsync(Path.Combine(dummyFolderPath, dummyFileName));
            string zippedString = Zipper.ZipString(originalString);
            Assert.Equal(Zipper.UnZipString(zippedString), originalString);
            Utils.RemoveDir(dummyFolderPath);
        }

        protected async Task ZipByteArrayCore()
        {
            string tempDir = GetTempDirectory();
            string dummyFileName = "dummy.txt";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
            string dummyFilePath = Utils.CreateDummyFile(dummyFolderPath, dummyFileName, cleanFolder: true);
            byte[] expectedBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
            byte[] zippedBytes = Zipper.ZipByteArray(expectedBytes, "pippo");
            byte[] actualUnzippedBytes = await Zipper.UnZipSingleFileContentToByteArrayAsync(zippedBytes).ConfigureAwait(false);
            Assert.Equal(expectedBytes, actualUnzippedBytes);
            Utils.RemoveDir(dummyFolderPath);
        }

        //protected async Task ZipSingleFileContentCore()
        //{
        //    string tempDir = GetTempDirectory();
        //    string dummyFileName = "dummy.txt";
        //    string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
        //    string dummyFilePath = Utils.CreateDummyFile(dummyFolderPath, dummyFileName, cleanFolder: false);
        //    byte[] originalBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
        //    Stream streamOfOriginalBytes = new MemoryStream(originalBytes);
        //    Stream streamOfZippedBytes = Zipper.ZipSingleFileContent(originalBytes, "pippo");
        //    Assert.False(true);
        //    Utils.RemoveDir(dummyFolderPath);
        //}

        protected void UnZipStringCore()
        {
            string tempDir = GetTempDirectory();
            string dummyFileName = "dummy.txt";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, tempDir, cleanFolder: true);
            string dummyFilePath = Utils.CreateDummyFile(dummyFolderPath, dummyFileName, cleanFolder: true);
            string zippedString = Zipper.ZipString(dummyFilePath);
            string unZippedString = Zipper.UnZipString(zippedString);
            Assert.Equal(Zipper.ZipString(unZippedString), zippedString);
            Utils.RemoveDir(dummyFolderPath);
        }

        protected async Task CreateZipFromFileContentListCore()
        {
            string methodName = GetTempDirectory();
            string dummyFileZip = "dummyZip.zip";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, methodName, cleanFolder: true);
            string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
            List<string> files = new List<string>
            {
                Path.Combine(dummyFolderPath, "dummy1.txt"),
                Path.Combine(dummyFolderPath, "dummy2.txt"),
                Path.Combine(dummyFolderPath, "dummy3.txt")
            };
            files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            byte[] bytesOfZip = await KFile.ReadAllBytesAsync(dummyFileZipPath);
            IList<Core.Support.Pair<string, byte[]>> actualUnzippedFiles = UnZipper.GetFileContentsFromZipFile(bytesOfZip);
            byte[] bytesOfZipFile = Zipper.CreateZipFromFileContentList(actualUnzippedFiles);
            IList<Core.Support.Pair<string, byte[]>> expectedUnzippedFiles = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.True(expectedUnzippedFiles.Count == actualUnzippedFiles.Count);
            Utils.RemoveDir(dummyFolderPath);
        }

        //protected void WriteAllBytesAsyncCore()
        //{
        //    string methodName = GetTempDirectory();
        //    string dummyFileZip = "dummyZip.zip";
        //    string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, methodName, cleanFolder: true);
        //    string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
        //    List<string> files = new List<string>
        //    {
        //        Path.Combine(dummyFolderPath, "dummy1.txt"),
        //        Path.Combine(dummyFolderPath, "dummy2.txt"),
        //        Path.Combine(dummyFolderPath, "dummy3.txt")
        //    };
        //    files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
        //    Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
        //    Directory.CreateDirectory(Path.Combine(dummyFolderPath , "dummy"));
        //    byte[] bytesOfZippedFolder = Zipper.CreateZipContentFromFolder(dummyFolderPath, Path.Combine(dummyFolderPath , "dummy"), "*.txt", false);
        //    IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(bytesOfZippedFolder);
        //    Assert.False(true);
        //    Utils.RemoveDir(dummyFolderPath);
        //}

        //protected async Task UnZipSingleFileContentToStreamCore()
        //{
        //    string methodName = GetTempDirectory();
        //    string dummyFileZip = "dummyZip.zip";
        //    string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, methodName, cleanFolder: true);
        //    string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
        //    List<string> files = new List<string>
        //    {
        //        Path.Combine(dummyFolderPath, "dummy1.txt"),
        //    };
        //    files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
        //    Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
        //    byte[] fileContent = await KFile.ReadAllBytesAsync(dummyFileZipPath);
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        Zipper.UnZipSingleFileContentToStream(fileContent, ms);
        //        byte[] bytesOfStream = ms.ToArray();
        //        await KFile.WriteAllBytesAsync(Path.Combine(dummyFolderPath, "dummy.zip"), bytesOfStream).ConfigureAwait(false);
        //    }
        //    IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(Path.Combine(dummyFolderPath, "dummy.zip"));
        //    Assert.False(true);
        //    Utils.RemoveDir(dummyFolderPath);
        //}

        protected async Task UnzipDllsFromNupkgFileCore()
        {
            string methodName = GetTempDirectory();
            string dummyFileZip = "dummyZip.zip";
            string dummyFolderPath = Utils.CreateDummyDirectory(_mockData.ResourceFolder, methodName, cleanFolder: true);
            string dummyFileZipPath = Path.Combine(dummyFolderPath, dummyFileZip);
            List<string> files = 
                new List<string>
                {
                    Path.Combine(dummyFolderPath, "dummy1.txt"),
                    Path.Combine(dummyFolderPath, "dummy2.txt"),
                    Path.Combine(dummyFolderPath, "dummy3.txt")
                };
            files.ForEach(file => Utils.CreateDummyFile(dummyFolderPath, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            string unzipDirectory = Utils.CreateDummyDirectory(dummyFolderPath, "dummy", cleanFolder: true);
            await UnZipper.ExtractFlatFilesFromZipFile(dummyFileZipPath, unzipDirectory, "*.txt");
            Assert.Equal(Directory.GetFiles(unzipDirectory).Length, files.Count);
            Utils.RemoveDir(dummyFolderPath);
        }
    }
}
