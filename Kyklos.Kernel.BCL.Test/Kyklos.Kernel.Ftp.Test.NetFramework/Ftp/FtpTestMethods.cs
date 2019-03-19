using Kyklos.Kernel.Ftp.Test.Ftp;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Ftp.Test.NetFramework.Ftp
{
    public class FtpTestMethods : FtpBaseTestMethods
    {
        public FtpTestMethods() : base(NetPlatformType.NETFRAMEWORK) { }
    }
}
