using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Kyklos.Kernel.Ftp;
using System.Diagnostics;
using Kyklos.Kernel.Ftp.Test.Support;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;
using Kyklos.Kernel.Ftp.Sftp;

namespace Kyklos.Kernel.Ftp.Test.Ftp
{
    public class FtpBaseTestMethods : IDisposable
    {
        private MockData MockData;
        private Process Process { get; }
        private FtpClient FtpClient;

        public FtpBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = MockData.KftpServerFolder + "\\KFtpsServer.exe";
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
