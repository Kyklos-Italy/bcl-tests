﻿using Kyklos.Kernel.Ftp.Test.Ftp;
using Xunit;
using XUnitTestSupport;

namespace Kyklos.Kernel.Ftp.Test.NetCore.Ftp
{
    public class FtpTestMethods : FtpBaseTestMethods
    {
        public FtpTestMethods() : base(NetPlatformType.NETCORE) { }

        [Fact(DisplayName = "MustDeleteFile")]
        public void MustDeleteFile()
        {
            MustDeleteFileCore();
        }
    }
}
