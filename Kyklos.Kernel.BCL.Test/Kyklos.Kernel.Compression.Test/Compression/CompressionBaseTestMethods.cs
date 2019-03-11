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

        public CompressionBaseTestMethods(FrameworkType frameworkType)
        {
            _mockData = new MockData(frameworkType);
        }

        protected void CreateZipFileFromFileListCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = _mockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                _mockData.ResourceFolder + "\\dummy1.txt",
                _mockData.ResourceFolder + "\\dummy2.txt",
                _mockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(_mockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(_mockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileList(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.True(fileContents.Count == files.Count);
        }

        protected void CreateZipFileFromFileListWithFileNameOnlyCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = _mockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                _mockData.ResourceFolder + "\\dummy1.txt",
                _mockData.ResourceFolder + "\\dummy2.txt",
                _mockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(_mockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(_mockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(dummyFileZipPath);
            Assert.Equal(fileContents.Count, files.Count);
        }

        protected async Task ZipStringCore()
        {
            string dummyFileName = "dummy.txt";
            Utils.CreateDummyFile(_mockData.ResourceFolder, dummyFileName, cleanFolder: true);
            string originalString = await KFile.ReadAllTextAsync(_mockData.ResourceFolder + "\\" + dummyFileName);
            string zippedString = Zipper.ZipString(originalString);
            Assert.Equal(Zipper.UnZipString(zippedString), originalString);
        }

        protected async Task ZipByteArrayCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(_mockData.ResourceFolder, dummyFileName, cleanFolder: true);
            byte[] expectedBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
            byte[] zippedBytes = Zipper.ZipByteArray(expectedBytes, "pippo");
            byte[] actualUnzippedBytes = await Zipper.UnZipSingleFileContentToByteArrayAsync(zippedBytes).ConfigureAwait(false);
            Assert.Equal(expectedBytes, actualUnzippedBytes);
        }

        protected async Task ZipSingleFileContentCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(_mockData.ResourceFolder, dummyFileName, cleanFolder: true);
            byte[] originalBytes = await KFile.ReadAllBytesAsync(dummyFilePath);
            Stream streamOfOriginalBytes = new MemoryStream(originalBytes);
            Stream streamOfZippedBytes = Zipper.ZipSingleFileContent(originalBytes, "pippo");
            Assert.False(true);
        }

        protected void UnZipStringCore()
        {
            string dummyFileName = "dummy.txt";
            string dummyFilePath = Utils.CreateDummyFile(_mockData.ResourceFolder, dummyFileName, cleanFolder: true);
            string zippedString = Zipper.ZipString(dummyFilePath);
            string unZippedString = Zipper.UnZipString(zippedString);
            Assert.Equal(Zipper.ZipString(unZippedString), zippedString);
        }

        protected async Task CreateZipFromFileContentListCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = _mockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                _mockData.ResourceFolder + "\\dummy1.txt",
                _mockData.ResourceFolder + "\\dummy2.txt",
                _mockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(_mockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(_mockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
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
            string dummyFileZipPath = _mockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                _mockData.ResourceFolder + "\\dummy1.txt",
                _mockData.ResourceFolder + "\\dummy2.txt",
                _mockData.ResourceFolder + "\\dummy3.txt"
            };
            Utils.CleanDirectory(_mockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(_mockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            Directory.CreateDirectory(_mockData.ResourceFolder + "\\dummy");
            byte[] bytesOfZippedFolder = Zipper.CreateZipContentFromFolder(_mockData.ResourceFolder, _mockData.ResourceFolder + "\\dummy", "*.txt", false);
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(bytesOfZippedFolder);
            Assert.False(true);
        }

        protected async Task UnZipSingleFileContentToStreamCore()
        {
            string dummyFileZip = "dummyZip.zip";
            string dummyFileZipPath = _mockData.ResourceFolder + "\\" + dummyFileZip;
            List<string> files = new List<string>
            {
                _mockData.ResourceFolder + "\\dummy1.txt"
            };
            Utils.CleanDirectory(_mockData.ResourceFolder);
            files.ForEach(file => Utils.CreateDummyFile(_mockData.ResourceFolder, Path.GetFileName(file), cleanFolder: false));
            Zipper.CreateZipFileFromFileListWithFileNameOnly(dummyFileZipPath, files);
            byte[] fileContent = await KFile.ReadAllBytesAsync(dummyFileZipPath);
            using (MemoryStream ms = new MemoryStream())
            {
                Zipper.UnZipSingleFileContentToStream(fileContent, ms);
                byte[] bytesOfStream = ms.ToArray();
                await KFile.WriteAllBytesAsync(_mockData.ResourceFolder + "\\dummy.zip", bytesOfStream).ConfigureAwait(false);
            }
            IList<Pair<string, byte[]>> fileContents = UnZipper.GetFileContentsFromZipFile(_mockData.ResourceFolder + "\\dummy.zip");
            Assert.False(true);
        }

        protected async Task UnzipDllsFromNupkgFileCore()
        {
            string zipPath = @"C:\development\Git\Kyklos-Essentials\src\libs\BCL\Kyklos.Kernel.BCL\Kyklos.Kernel.Dynamic.Castle\bin\Debug\Kyklos.Kernel.Dynamic.Castle.1.0.0-beta.1.nupkg";
            string outDir = @"C:\tmp\cippa";
            await UnZipper.ExtractFlatFilesFromZipFile(zipPath, outDir, "*.dll");
        }
    }
}
