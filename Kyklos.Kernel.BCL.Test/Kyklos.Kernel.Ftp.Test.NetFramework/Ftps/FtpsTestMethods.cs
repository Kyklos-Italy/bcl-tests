using Kyklos.Kernel.Ftp.Test.Ftps;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Ftp.Test.NetFramework.Ftps
{
    public class FtpsTestMethods : FtpsBaseTestMethods
    {
        public FtpsTestMethods() : base(NetPlatformType.NETFRAMEWORK) { }
    }
}
