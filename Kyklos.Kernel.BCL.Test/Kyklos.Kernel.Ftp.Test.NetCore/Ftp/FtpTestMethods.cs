using Kyklos.Kernel.Ftp.Test.Ftp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.NetCore.Ftp
{
    public class FtpTestMethods : FtpBaseTestMethods
    {
        public FtpTestMethods() : base(FrameworkType.NETCORE) { }

        [Fact(DisplayName = "MustDeleteFile")]
        public void MustDeleteFile()
        {
            MustDeleteFileCore();
        }
    }
}
