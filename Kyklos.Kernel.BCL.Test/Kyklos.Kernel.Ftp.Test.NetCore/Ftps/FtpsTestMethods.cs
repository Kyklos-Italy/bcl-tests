using Kyklos.Kernel.Ftp.Test.Ftps;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Ftp.Test.NetCore.Ftps
{
    public class FtpsTestMethods : FtpsBaseTestMethods
    {
        public FtpsTestMethods() : base(NetPlatformType.NETCORE) { }
    }
}
