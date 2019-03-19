using System.Threading.Tasks;
using Kyklos.Kernel.Ftp.Test.Sftp;
using Xunit;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Ftp.Test.NetFramework.Sftp
{
    public class SftpTestMethods : SftpBaseTestMethods
    {
        public SftpTestMethods() : base(NetPlatformType.NETFRAMEWORK) { }

        [Fact(DisplayName = "MustMakeDir")]
        public void MustMakeDir()
        {
            MustMakeDirCore();
        }

        [Fact(DisplayName = "MustDeleteFile")]
        public void MustDeleteFile()
        {
            MustDeleteFileCore();
        }

        [Fact(Skip = "The method or operation is not implemented", DisplayName = "DirectoryMustExists")]
        public void DirectoryMustExists()
        {
            DirectoryMustExistsCore();
        }

        [Fact(DisplayName = "MustGetDirectoryList")]
        public void MustGetDirectoryList()
        {
            MustGetDirectoryListCore();
        }

        [Fact(DisplayName = "MustGetDirectoryListAsync")]
        public async Task MustGetDirectoryListAsync()
        {
            await MustGetDirectoryListAsyncCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "MustGetFileModificationTime")]
        public void MustGetFileModificationTime()
        {
            MustGetFileModificationTimeCore();
        }

        [Fact(DisplayName = "MustGetFileTransferSize")]
        public void MustGetFileTransferSize()
        {
            MustGetFileTransferSizeCore();
        }

        [Fact(DisplayName = "MustGetShortDirectoryList")]
        public void MustGetShortDirectoryList()
        {
            MustGetShortDirectoryListCore();
        }

        [Fact(DisplayName = "MustGetShortDirectoryListAsync")]
        public async Task MustGetShortDirectoryListAsync()
        {
            await MustGetShortDirectoryListAsyncCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "MustRemoveDir")]
        public void MustRemoveDir()
        {
            MustRemoveDirCore();
        }

        [Fact(DisplayName = "MustRenameFile")]
        public void MustRenameFile()
        {
            MustRenameFileCore();
        }

        [Fact(DisplayName = "MustSetCurrentDirectory")]
        public void MustSetCurrentDirectory()
        {
            MustSetCurrentDirectoryCore();
        }

        [Fact(Skip = "capire come fare", DisplayName = "MustGetFile")]
        public void MustGetFile()
        {
            MustGetFileCore();
        }

        [Fact(Skip = "capire come fare", DisplayName = "MustGetFileAsync")]
        public async Task MustGetFileAsync()
        {
            await MustGetFileAsyncCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "MustGetFiles")]
        public void MustGetFiles()
        {
            MustGetFilesCore();
        }

        [Fact(DisplayName = "MustPutFile")]
        public void MustPutFile()
        {
            MustPutFileCore();
        }

        [Fact(DisplayName = "MustPutFileAsync")]
        public async Task MustPutFileAsync()
        {
            await MustPutFileAsyncCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "MustPutFilesAsync")]
        public async Task MustPutFilesAsync()
        {
            await MustPutFilesAsyncCore().ConfigureAwait(false);
        }

        [Fact(DisplayName = "FilesTransferredFromServerMustBeEqualToFilesInRemote")]
        public void FilesTransferredFromServerMustBeEqualToFilesInRemote()
        {
            FilesTransferredFromServerMustBeEqualToFilesInRemoteCore();
        }

        [Fact(DisplayName = "FilesTransferredToServerMustBeEqualToLocal")]
        public void FilesTransferredToServerMustBeEqualToLocal()
        {
            FilesTransferredToServerMustBeEqualToLocalCore();
        }

        [Fact(DisplayName = "FilesTransferredMustBeEqualTo2")]
        public void FilesTransferredMustBeEqualTo2()
        {
            FilesTransferredMustBeEqualTo2Core();
        }


    }
}
