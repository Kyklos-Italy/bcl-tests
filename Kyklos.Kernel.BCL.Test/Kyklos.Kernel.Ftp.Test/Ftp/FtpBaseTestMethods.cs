using System;
using System.Diagnostics;
using System.IO;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using Xunit;
using XUnitTestSupport;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Ftp.Test.Ftp
{
    public class FtpBaseTestMethods : IDisposable
    {
        private MockData MockData;
        private Process Process { get; }
        private FtpClient FtpClient;

        public FtpBaseTestMethods(NetPlatformType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = Path.Combine(MockData.KftpServerFolder, "KFtpsServer.exe");
            Process.Start();
            FtpClient = new FtpClient(MockData.HostName, MockData.Username, MockData.Password);
        }

        public void Dispose()
        {
            Process.Kill();
            Process.WaitForExit();
        }

        protected void MustDeleteFileCore()
        {
            string dummyFileName = "dummy.txt";
            string actualFilePath = Utils.CreateDummyFile(MockData.FtpDataFolder, dummyFileName, cleanFolder: true);
            FtpClient.DeleteFtpFile(dummyFileName, MockData.FtpDataFolder);
            Assert.False(File.Exists(actualFilePath));
        }
    }
}
