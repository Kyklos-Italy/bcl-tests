using Kyklos.Kernel.Ftp.Sftp;
using Kyklos.Kernel.Ftp.Test.Support.Mock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.Ftps
{
    public class FtpsBaseTestMethods : IDisposable
    {
        private MockData MockData;
        private Process Process { get; }

        public FtpsBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
            Process = new Process();
            Process.StartInfo.FileName = MockData.RebexFolder + "\\RebexTinySftpServer.exe";
            Process.Start();
        }

        public void Dispose()
        {
            //Process.Kill();
            Process.WaitForExit();
        }
    }
}
