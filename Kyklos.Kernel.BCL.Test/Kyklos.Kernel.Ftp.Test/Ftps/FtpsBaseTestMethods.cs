using System;
using System.Diagnostics;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using XUnitTestSupport;

namespace Kyklos.Kernel.Ftp.Test.Ftps
{
    public class FtpsBaseTestMethods : IDisposable
    {
        private MockData MockData;
        private Process Process { get; }

        public FtpsBaseTestMethods(NetPlatformType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = MockData.RebexFolder + "\\RebexTinySftpServer.exe";
            Process.Start();
        }

        public void Dispose()
        {
            Process.WaitForExit();
        }
    }
}
