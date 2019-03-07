using Kyklos.Kernel.Ftp.Sftp;
using Kyklos.Kernel.Ftp.Test.Support;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.Sftp
{
    public class SftpBaseTestMethods : IDisposable
    {
        private MockData MockData;
        private SftpClient SftpClient;
        private Process Process { get; }

        public SftpBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = MockData.RebexFolder + "\\RebexTinySftpServer.exe";
            Process.Start();
            SftpClient = new SftpClient();
            SftpClient.Connect(MockData.HostName, MockData.Username, MockData.Password);
        }

        public void Dispose()
        {
            SftpClient.Close();
            Process.Kill();
            Process.WaitForExit();
        }

        protected void MustMakeDirCore()
        {
            string dummyFolderName = "dummy";
            Utils.CleanDirectory(MockData.SftpDataFolder);
            SftpClient.MakeDir(dummyFolderName);
            Assert.True(Directory.Exists(MockData.SftpDataFolder + "\\" + dummyFolderName));
        }

        protected void MustDeleteFileCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            SftpClient.DeleteFile(dummyFileName);
            Assert.False(File.Exists(actualFilePath));
        }

        protected void DirectoryMustExistsCore()
        {
            string dummyFolderName = "dummy";
            string actualFolderPath = Utils.CreateDummyDirectory(MockData.SftpDataFolder, dummyFolderName, cleanFolder: true);
            Assert.True(SftpClient.DirectoryExists(dummyFolderName));
        }

        protected void MustGetDirectoryListCore()
        {
            List<string> actualDirectories = new List<string>()
            {
                "dir1",
                "dir2",
                "dir3",
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: false));
            List<SftpFile> files = (List<SftpFile>)SftpClient.GetDirectoryList("/");
            bool allDirectoryArePresent = true;
            foreach (SftpFile file in files)
            {
                if (file.Name != "." && file.Name != "..")
                {
                    if (!actualDirectories.Contains(file.Name))
                    {
                        allDirectoryArePresent = false;
                    }
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected async Task MustGetDirectoryListAsyncCore()
        {
            List<string> actualDirectories = new List<string>()
            {
                "dir1",
                "dir2",
                "dir3",
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: false));
            List<SftpFile> files = (List<SftpFile>) await SftpClient.GetDirectoryListAsync("/").ConfigureAwait(false);
            bool allDirectoryArePresent = true;
            foreach (SftpFile file in files)
            {
                if (file.Name != "." && file.Name != "..")
                {
                    if (!actualDirectories.Contains(file.Name))
                    {
                        allDirectoryArePresent = false;
                    }
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected void MustGetFileModificationTimeCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyDirectory(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            DateTime expectedModificationTime = File.GetLastWriteTime(actualFilePath);
            DateTime? actualModificationTime = SftpClient.GetFileModificationTime(Path.GetFileName(actualFilePath));
            Assert.Equal(expectedModificationTime.ToString(), ((DateTime)actualModificationTime).ToString());
        }

        protected void MustGetFileTransferSizeCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            FileInfo fileInfo = new FileInfo(actualFilePath);
            ulong actualSize = (ulong)fileInfo.Length;
            ulong? expectedSize = SftpClient.GetFileTransferSize(Path.GetFileName(actualFilePath));
            Assert.Equal(expectedSize, actualSize);
        }

        protected void MustGetShortDirectoryListCore()
        {
            List<string> actualDirectories = new List<string>()
            {
                "dir1",
                "dir2",
                "dir3",
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: false));
            IList<string> files = SftpClient.GetShortDirectoryList("/");
            bool allDirectoryArePresent = true;
            foreach (string file in files)
            {
                if (file != "." && file != "..")
                {
                    if (!actualDirectories.Contains(file))
                    {
                        allDirectoryArePresent = false;
                    }
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected async Task MustGetShortDirectoryListAsyncCore()
        {
            List<string> actualDirectories = new List<string>()
            {
                "dir1",
                "dir2",
                "dir3",
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: false));
            IList<string> files = await SftpClient.GetShortDirectoryListAsync("/").ConfigureAwait(false);
            bool allDirectoryArePresent = true;
            foreach (string file in files)
            {
                if (file != "." && file != "..")
                {
                    if (!actualDirectories.Contains(file))
                    {
                        allDirectoryArePresent = false;
                    }
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected void MustRemoveDirCore()
        {
            string dummyFolderName = "dummy";
            Utils.CreateDummyDirectory(MockData.SftpDataFolder, dummyFolderName, cleanFolder: true);
            SftpClient.RemoveDir(dummyFolderName);
            Assert.False(Directory.Exists(MockData.SftpDataFolder + "\\" + dummyFolderName));
        }

        protected void MustRenameFileCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            string expectedFileName = "renamed.txt";
            SftpClient.RenameFile(dummyFileName, expectedFileName);
            Assert.True(File.Exists(MockData.SftpDataFolder + "\\" + expectedFileName));
        }
       
        protected void MustSetCurrentDirectoryCore()
        {
            string dummyFolderName = "dummy";
            string subDummyFolderName = "subDummy";
            Utils.CreateDummyDirectory(MockData.SftpDataFolder, dummyFolderName, cleanFolder: true);
            SftpClient.SetCurrentDirectory(dummyFolderName);
            SftpClient.MakeDir(subDummyFolderName);
            Assert.True(Directory.Exists(MockData.SftpDataFolder + "\\" + dummyFolderName + "\\" + subDummyFolderName));
            Assert.True(true);
        }

        protected void MustGetFileCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            string localFilePath = MockData.RebexDataFolder + "\\" + dummyFileName;
            Utils.CleanDirectory(MockData.RebexDataFolder);
            SftpClient.GetFile(dummyFileName, localFilePath);
            Assert.True(true);
        }

        protected async Task MustGetFileAsyncCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: true);
            string localFilePath = MockData.RebexDataFolder + "\\" + dummyFileName;
            Utils.CleanDirectory(MockData.RebexDataFolder);
            await SftpClient.GetFileAsync(dummyFileName, localFilePath, null, null).ConfigureAwait(false);
            Assert.True(true);
        }

        protected void MustGetFilesCore()
        {
            List<string> dummyFileNames = new List<string>()
            {
                "dummy1.txt",
                "dummy2.txt",
                "dummy3.txt"
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.SftpDataFolder, x, cleanFolder: false));
            Utils.CleanDirectory(MockData.RebexDataFolder);
            SftpClient.GetFiles(MockData.RebexDataFolder);
            bool allFilesArePresentInLocalFolder = true;
            foreach (string filePath in Directory.GetFiles(MockData.RebexDataFolder))
            {
                if (!dummyFileNames.Contains(Path.GetFileName(filePath)))
                {
                    allFilesArePresentInLocalFolder = false;
                }   
            }
            Assert.True(allFilesArePresentInLocalFolder);
        }

        protected void MustPutFileCore()
        {
            string dummyFileName = "dummy.txt";
            string localFilePath = Utils.CreateDummyFile(MockData.RebexDataFolder, dummyFileName, cleanFolder: true);
            string remoteFilePath = MockData.SftpDataFolder + "\\" + dummyFileName;
            Utils.CleanDirectory(MockData.SftpDataFolder);
            SftpClient.PutFile(localFilePath, dummyFileName, null, "", "");
            Assert.True(File.Exists(remoteFilePath));
        }

        protected async Task MustPutFileAsyncCore()
        {
            string dummyFileName = "dummy.txt";
            string localFilePath = Utils.CreateDummyFile(MockData.RebexDataFolder, dummyFileName, cleanFolder: true);
            string remoteFilePath = MockData.SftpDataFolder + "\\" + dummyFileName;
            Utils.CleanDirectory(MockData.SftpDataFolder);
            await SftpClient.PutFileAsync(localFilePath, dummyFileName, null, "", "").ConfigureAwait(false);
            Assert.True(File.Exists(remoteFilePath));
        }

        protected void MustPutFilesCore()
        {
            List<string> dummyFileNames = new List<string>()
            {
                "dummy1.txt",
                "dummy2.txt",
                "dummy3.txt"
            };
            Utils.CleanDirectory(MockData.RebexDataFolder);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.RebexDataFolder, x, cleanFolder: false));
            Utils.CleanDirectory(MockData.SftpDataFolder);
            SftpClient.PutFiles(MockData.RebexDataFolder);
            bool allFilesArePresentInRemoteFolder = true;
            foreach (string filePath in Directory.GetFiles(MockData.SftpDataFolder))
            {
                if (!dummyFileNames.Contains(Path.GetFileName(filePath)))
                {
                    allFilesArePresentInRemoteFolder = false;
                }
            }
            Assert.True(allFilesArePresentInRemoteFolder);
        }

        protected async Task MustPutFilesAsyncCore()
        {
            List<string> dummyFileNames = new List<string>()
            {
                "dummy1.txt",
                "dummy2.txt",
                "dummy3.txt"
            };
            Utils.CleanDirectory(MockData.RebexDataFolder);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.RebexDataFolder, x, cleanFolder: false));
            Utils.CleanDirectory(MockData.SftpDataFolder);
            await SftpClient.PutFilesAsync(MockData.RebexDataFolder, "/", "*.txt", Enums.EPatternStyle.Wildcard, false, null).ConfigureAwait(false);
            bool allFilesArePresentInRemoteFolder = false;
            if (Directory.GetFiles(MockData.SftpDataFolder).Length > 0)
            {
                allFilesArePresentInRemoteFolder = true;
                foreach (string filePath in Directory.GetFiles(MockData.SftpDataFolder))
                {
                    if (!dummyFileNames.Contains(Path.GetFileName(filePath)))
                    {
                        allFilesArePresentInRemoteFolder = false;
                    }
                }
            }
            Assert.True(allFilesArePresentInRemoteFolder);
        }

        protected void FilesTransferredFromServerMustBeEqualTo3Core()
        {
            List<string> dummyFileNames = new List<string>()
            {
                "dummy1.txt",
                "dummy2.txt",
                "dummy3.txt"
            };
            Utils.CleanDirectory(MockData.SftpDataFolder);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.SftpDataFolder, x, cleanFolder: false));
            Utils.CleanDirectory(MockData.RebexDataFolder);
            SftpClient.GetFiles(MockData.RebexDataFolder);
            Assert.True(SftpClient.FilesTransferredFromServer.Count == 3);
        }

        protected void FilesTransferredToServerMustBeEqualTo3Core()
        {
            List<string> dummyFileNames = new List<string>()
            {
                "dummy1.txt",
                "dummy2.txt",
                "dummy3.txt"
            };
            Utils.CleanDirectory(MockData.RebexDataFolder);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.RebexDataFolder, x, cleanFolder: false));
            Utils.CleanDirectory(MockData.SftpDataFolder);
            SftpClient.PutFiles(MockData.RebexDataFolder);
            Assert.True(SftpClient.FilesTransferredToServer.Count == 3);
        }

        protected void FilesTransferredMustBeEqualTo2Core()
        {
            string remoteFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, remoteFileName, cleanFolder: true);
            string localFilePath = MockData.RebexDataFolder + "\\" + remoteFileName;
            Utils.CleanDirectory(MockData.RebexDataFolder);
            SftpClient.GetFile(remoteFileName, localFilePath);

            string localFileName = "dummy.txt";
            localFilePath = Utils.CreateDummyFile(MockData.RebexDataFolder, localFileName, cleanFolder: true);
            string remoteFilePath = MockData.SftpDataFolder + "\\" + localFileName;
            Utils.CleanDirectory(MockData.SftpDataFolder);
            SftpClient.PutFile(localFilePath, localFileName, null, "", "");

            Assert.True(SftpClient.FilesTransferred.Count == 2);
        }
    }
}
