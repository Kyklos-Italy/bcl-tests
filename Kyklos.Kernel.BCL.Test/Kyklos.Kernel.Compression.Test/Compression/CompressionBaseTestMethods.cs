using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Kyklos.Kernel.Compression.Zip;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Kyklos.Kernel.Compression.Test.Support;
using Kyklos.Kernel.IO.Async;
using Kyklos.Kernel.Compression.Test.Support.Mock;
using static Kyklos.Kernel.Compression.Test.Support.Framework;
using Kyklos.Kernel.Core.Support;

namespace Kyklos.Kernel.Compression.Test.Compression
{
    public class CompressionBaseTestMethods
    {
        private MockData MockData;

        public CompressionBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
        }

        protected void CreateZipFileFromFileListCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = MockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                MockData.ResourceFolder + "\\dummy1.txt",
                MockData.ResourceFolder + "\\dummy2.txt",
                MockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(MockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(MockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileList(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.True(fileContents.Count == files.Count);
        }

        protected void CreateZipFileFromFileListWithFileNameOnlyCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = MockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                MockData.ResourceFolder + "\\dummy1.txt",
                MockData.ResourceFolder + "\\dummy2.txt",
                MockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(MockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(MockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.Equal(fileContents.Count, files.Count);
        }

        protected async Task ZipStringCore()
        {
            string dummyFileName = "dummy.txt";
            Utils.CreateDummyFile(MockData.ResourceFolder, dummyFileName, cleanFolder: true);
            string originalString = await KFile.ReadAllTextAsync(MockData.ResourceFolder + "\\" + dummyFileName);
            string zippedString = Zipper.ZipString(originalString);
            Assert.Equal(Zipper.UnZipString(zippedString), originalString);
        }

        protected async Task ZipByteArrayCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(MockData.ResourceFolder, dummyFileName, cleanFolder: true);
            byte[] expectedBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
            byte[] zippedBytes = Zipper.ZipByteArray(expectedBytes, "pippo");
            byte[] actualUnzippedBytes = await Zipper.UnZipSingleFileContentToByteArrayAsync(zippedBytes).ConfigureAwait(false);
            Assert.Equal(expectedBytes, actualUnzippedBytes);
        }

        protected async Task ZipSingleFileContentCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(MockData.ResourceFolder, dummyFileName, cleanFolder: true);
            byte[] originalBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
            Stream streamOfOriginalBytes = new MemoryStream(originalBytes);
            Stream streamOfZippedBytes = Zipper.ZipSingleFileContent(originalBytes, "pippo");
            Assert.False(true);
        }

        protected void UnZipStringCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(MockData.ResourceFolder, dummyFileName, cleanFolder: true);
            string zippedString = Zipper.ZipString(dummyFilePath);
            string unZippedString = Zipper.UnZipString(zippedString);
            Assert.Equal(Zipper.ZipString(unZippedString), zippedString);
        }

        protected async Task CreateZipFromFileContentListCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = MockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                MockData.ResourceFolder + "\\dummy1.txt",
                MockData.ResourceFolder + "\\dummy2.txt",
                MockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(MockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(MockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            byte[] bytesOfZip = await KFile.ReadAllBytesAsync(dummyFileZipPath);
            IList<Core.Support.Pair<string, byte[]>> actualUnzippedFiles = UnZipper.GetFileContentsFromZipFile(bytesOfZip);
            byte[] bytesOfZipFile = Zipper.CreateZipFromFileContentList(actualUnzippedFiles);
            IList<Core.Support.Pair<string, byte[]>> expectedUnzippedFiles = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.True(expectedUnzippedFiles.Count == actualUnzippedFiles.Count);
        }

        protected void WriteAllBytesAsyncCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = MockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                MockData.ResourceFolder + "\\dummy1.txt",
                MockData.ResourceFolder + "\\dummy2.txt",
                MockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(MockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(MockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            Directory.CreateDirectory(MockData.ResourceFolder + "\\dummy");
            byte[] bytesOfZippedFolder = Zipper.CreateZipContentFromFolder(MockData.ResourceFolder, MockData.ResourceFolder + "\\dummy", "*.txt", false);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(bytesOfZippedFolder);
            Assert.False(true);
        }

        protected async Task UnZipSingleFileContentToStreamCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = MockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                MockData.ResourceFolder + "\\dummy1.txt"
            };
            Utils.CleanDirectory(MockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(MockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            byte[] fileContent = await KFile.ReadAllBytesAsync(dummyFileZipPath);
            using (MemoryStream ms = new MemoryStream())
            {
                Zipper.UnZipSingleFileContentToStream(fileContent, ms);
                byte[] bytesOfStream = ms.ToArray();
                await KFile.WriteAllBytesAsync(MockData.ResourceFolder + "\\dummy.zip", bytesOfStream).ConfigureAwait(false);
            }
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(MockData.ResourceFolder + "\\dummy.zip");
            Assert.False(true);
        }
    }
}
