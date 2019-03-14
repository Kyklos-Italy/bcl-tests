using Kyklos.Kernel.Ftp.Sftp;
using Kyklos.Kernel.Ftp.Test.Support;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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

        public static string GetCurrentMethod([CallerMemberName] string method = "")
        {
            return method;
        }

        public SftpBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = Path.Combine(MockData.RebexFolder, "RebexTinySftpServer.exe");
            Process.StartInfo.WorkingDirectory = MockData.RebexFolder;
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
            string methodName = GetCurrentMethod();
            string remoteFolder = Path.Combine(MockData.SftpDataFolder, methodName);
            if (Directory.Exists(remoteFolder))
            {
                Directory.Delete(remoteFolder);
            }
            SftpClient.MakeDir(methodName);
            Assert.True(Directory.Exists(remoteFolder));
        }

        protected void MustDeleteFileCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            SftpClient.DeleteFile(dummyFileName);
            Assert.False(File.Exists(Path.Combine(MockData.SftpDataFolder, dummyFileName)));
        }

        protected void DirectoryMustExistsCore()
        {
            string methodName = GetCurrentMethod();
            string remoteFolder = Path.Combine(MockData.SftpDataFolder, methodName);
            if (Directory.Exists(remoteFolder))
            {
                Directory.Delete(remoteFolder);
            }
            string actualFolderPath = Utils.CreateDummyDirectory(remoteFolder, cleanFolder: false);
            Assert.True(SftpClient.DirectoryExists(methodName));
        }

        protected void MustGetDirectoryListCore()
        {
            string methodName = GetCurrentMethod();
            List<string> actualDirectories = new List<string>()
            {
                methodName + "_dir1",
                methodName + "_dir2",
                methodName + "_dir3",
            };
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: true));
            List<SftpFile> files = (List<SftpFile>)SftpClient.GetDirectoryList("/");
            bool allDirectoryArePresent = true;
            foreach(string actualDirectory in actualDirectories)
            {
                bool actualDirectoryIsPresent = false;
                foreach (SftpFile file in files)
                {
                    if (file.Name.Contains(actualDirectory))
                    {
                        actualDirectoryIsPresent = true;
                        break;
                    }
                }
                if (!actualDirectoryIsPresent)
                {
                    allDirectoryArePresent = false;
                    break;
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected async Task MustGetDirectoryListAsyncCore()
        {
            string methodName = GetCurrentMethod();
            List<string> actualDirectories = new List<string>()
            {
                methodName + "_dir1",
                methodName + "_dir2",
                methodName + "_dir3",
            };
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: true));
            List<SftpFile> files = (List<SftpFile>) await SftpClient.GetDirectoryListAsync("/").ConfigureAwait(false);
            bool allDirectoryArePresent = true;
            foreach (string actualDirectory in actualDirectories)
            {
                bool actualDirectoryIsPresent = false;
                foreach (SftpFile file in files)
                {
                    if (file.Name.Contains(actualDirectory))
                    {
                        actualDirectoryIsPresent = true;
                        break;
                    }
                }
                if (!actualDirectoryIsPresent)
                {
                    allDirectoryArePresent = false;
                    break;
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected void MustGetFileModificationTimeCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            DateTime expectedModificationTime = File.GetLastWriteTime(actualFilePath);
            DateTime? actualModificationTime = SftpClient.GetFileModificationTime(Path.GetFileName(actualFilePath));
            Assert.Equal(expectedModificationTime.ToString(), ((DateTime)actualModificationTime).ToString());
        }

        protected void MustGetFileTransferSizeCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            FileInfo fileInfo = new FileInfo(actualFilePath);
            ulong actualSize = (ulong)fileInfo.Length;
            ulong? expectedSize = SftpClient.GetFileTransferSize(Path.GetFileName(actualFilePath));
            Assert.Equal(expectedSize, actualSize);
        }

        protected void MustGetShortDirectoryListCore()
        {
            string methodName = GetCurrentMethod();
            List<string> actualDirectories = new List<string>()
            {
                methodName + "_dir1",
                methodName + "_dir2",
                methodName + "_dir3",
            };
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: true));
            IList<string> files = SftpClient.GetShortDirectoryList("/");
            bool allDirectoryArePresent = true;
            foreach (string actualDirectory in actualDirectories)
            {
                bool actualDirectoryIsPresent = false;
                foreach (string file in files)
                {
                    if (file.Contains(actualDirectory))
                    {
                        actualDirectoryIsPresent = true;
                        break;
                    }
                }
                if (!actualDirectoryIsPresent)
                {
                    allDirectoryArePresent = false;
                    break;
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected async Task MustGetShortDirectoryListAsyncCore()
        {
            string methodName = GetCurrentMethod();
            List<string> actualDirectories = new List<string>()
            {
                methodName + "_dir1",
                methodName + "_dir2",
                methodName + "_dir3",
            };
            actualDirectories.ForEach(directoryName => Utils.CreateDummyDirectory(MockData.SftpDataFolder, directoryName, cleanFolder: true));
            IList<string> files = await SftpClient.GetShortDirectoryListAsync("/").ConfigureAwait(false);
            bool allDirectoryArePresent = true;
            foreach (string actualDirectory in actualDirectories)
            {
                bool actualDirectoryIsPresent = false;
                foreach (string file in files)
                {
                    if (file.Contains(actualDirectory))
                    {
                        actualDirectoryIsPresent = true;
                        break;
                    }
                }
                if (!actualDirectoryIsPresent)
                {
                    allDirectoryArePresent = false;
                    break;
                }
            }
            Assert.True(allDirectoryArePresent);
        }

        protected void MustRemoveDirCore()
        {
            string methodName = GetCurrentMethod();
            Utils.CreateDummyDirectory(MockData.SftpDataFolder, methodName, cleanFolder: true);
            SftpClient.RemoveDir(methodName);
            Assert.False(Directory.Exists(Path.Combine(MockData.SftpDataFolder, methodName)));
        }

        protected void MustRenameFileCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            string expectedFileName = methodName + "_renamed.txt";
            SftpClient.RenameFile(dummyFileName, expectedFileName);
            Assert.True(File.Exists(Path.Combine(MockData.SftpDataFolder, expectedFileName)));
        }
       
        protected void MustSetCurrentDirectoryCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFolderName = methodName + "_dummy";
            string subDummyFolderName = methodName + "_subDummy";
            Utils.CreateDummyDirectory(MockData.SftpDataFolder, dummyFolderName, cleanFolder: true);
            SftpClient.SetCurrentDirectory(dummyFolderName);
            SftpClient.MakeDir(subDummyFolderName);
            Assert.True(Directory.Exists(Path.Combine(MockData.SftpDataFolder, dummyFolderName, subDummyFolderName)));
        }

        protected void MustGetFileCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            string localFilePath = Path.Combine(MockData.RebexDataFolder, dummyFileName);
            SftpClient.GetFile(dummyFileName, localFilePath);
            Assert.True(true);
        }

        protected async Task MustGetFileAsyncCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.SftpDataFolder, dummyFileName, cleanFolder: false);
            string localFilePath = MockData.RebexDataFolder + "\\" + dummyFileName;
            await SftpClient.GetFileAsync(dummyFileName, localFilePath, null, null).ConfigureAwait(false);
            Assert.True(true);
        }

        protected void MustGetFilesCore()
        {
            string methodName = GetCurrentMethod();
            List<string> dummyFileNames = new List<string>()
            {
                methodName + "_dummy1.txt",
                methodName + "_dummy2.txt",
                methodName + "_dummy3.txt"
            };
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(MockData.SftpDataFolder, x, cleanFolder: false));
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            SftpClient.GetFiles(localFolder);
            bool allFilesArePresentInLocalFolder = true;
            IList<string> filePaths = Directory.GetFiles(localFolder);
            foreach (string dummyFileName in dummyFileNames)
            {
                bool dummyFileNameIsPresent = false;
                foreach (string filePath in filePaths)
                {
                    if (filePath.Contains(dummyFileName))
                    {
                        dummyFileNameIsPresent = true;
                        break;
                    }
                }
                if (!dummyFileNameIsPresent)
                {
                    allFilesArePresentInLocalFolder = false;
                    break;
                }
            }
            Assert.True(allFilesArePresentInLocalFolder);
        }

        protected void MustPutFileCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(MockData.RebexDataFolder, methodName, cleanFolder: true);
            string localFilePath = Utils.CreateDummyFile(localFolder, dummyFileName, cleanFolder: false);
            string remoteFilePath = Path.Combine(MockData.SftpDataFolder, dummyFileName);
            SftpClient.PutFile(localFilePath, dummyFileName, null, "", "");
            Assert.True(File.Exists(remoteFilePath));
        }

        protected async Task MustPutFileAsyncCore()
        {
            string methodName = GetCurrentMethod();
            string dummyFileName = methodName + "_dummy.txt";
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(MockData.RebexDataFolder, methodName, cleanFolder: true);
            string localFilePath = Utils.CreateDummyFile(localFolder, dummyFileName, cleanFolder: false);
            string remoteFilePath = Path.Combine(MockData.SftpDataFolder, dummyFileName);
            await SftpClient.PutFileAsync(localFilePath, dummyFileName, null, "", "").ConfigureAwait(false);
            Assert.True(File.Exists(remoteFilePath));
        }

        protected void MustPutFilesCore()
        {
            string methodName = GetCurrentMethod();
            List<string> dummyFileNames = new List<string>()
            {
                methodName + "_dummy1.txt",
                methodName + "_dummy2.txt",
                methodName + "_dummy3.txt"
            };
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(MockData.RebexDataFolder, methodName, cleanFolder: true);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(localFolder, x, cleanFolder: false));
            SftpClient.PutFiles(localFolder);
            bool allFilesArePresentInRemoteFolder = true;
            foreach (string filePath in Directory.GetFiles(localFolder))
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
            string methodName = GetCurrentMethod();
            List<string> dummyFileNames = new List<string>()
            {
                methodName + "_dummy1.txt",
                methodName + "_dummy2.txt",
                methodName + "_dummy3.txt"
            };
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(MockData.RebexDataFolder, methodName, cleanFolder: true);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(localFolder, x, cleanFolder: false));
            await SftpClient.PutFilesAsync(MockData.RebexDataFolder, "/", "*.txt", Enums.EPatternStyle.Wildcard, false, null).ConfigureAwait(false);
            bool allFilesArePresentInRemoteFolder = true;
            foreach (string filePath in Directory.GetFiles(localFolder))
            {
                if (!dummyFileNames.Contains(Path.GetFileName(filePath)))
                {
                    allFilesArePresentInRemoteFolder = false;
                }
            }
            Assert.True(allFilesArePresentInRemoteFolder);
        }

        protected void FilesTransferredFromServerMustBeEqualToFilesInRemoteCore()
        {
            string methodName = GetCurrentMethod();
            string remoteFolder = Path.Combine(MockData.SftpDataFolder, methodName);
            List<string> dummyFileNames = new List<string>()
            {
                methodName + "_dummy1.txt",
                methodName + "_dummy2.txt",
                methodName + "_dummy3.txt"
            };
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(localFolder, cleanFolder: true);
            Utils.CreateDummyDirectory(remoteFolder, cleanFolder: true);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(remoteFolder, x, cleanFolder: false));
            SftpClient.GetFiles(localFolder);
            int remoteFileCount = Directory.GetFiles(MockData.SftpDataFolder).Length;
            Assert.True(SftpClient.FilesTransferredFromServer.Count == remoteFileCount);
        }

        protected void FilesTransferredToServerMustBeEqualToLocalCore()
        {
            string methodName = GetCurrentMethod();
            string remoteFolder = Path.Combine(MockData.SftpDataFolder, methodName);
            List<string> dummyFileNames = new List<string>()
            {
                methodName + "_dummy1.txt",
                methodName + "_dummy2.txt",
                methodName + "_dummy3.txt"
            };
            string localFolder = Path.Combine(MockData.RebexDataFolder, methodName);
            Utils.CreateDummyDirectory(localFolder, cleanFolder: true);
            Utils.CreateDummyDirectory(remoteFolder, cleanFolder: true);
            dummyFileNames.ForEach(x => Utils.CreateDummyFile(localFolder, x, cleanFolder: false));
            SftpClient.PutFiles(remoteFolder);
            int localFileCount = Directory.GetFiles(remoteFolder).Length;
            Assert.True(SftpClient.FilesTransferredToServer.Count == localFileCount);
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
