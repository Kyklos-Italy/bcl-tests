using Kyklos.Kernel.Ftp.Test.Ftps;
using XUnitTestSupport;

namespace Kyklos.Kernel.Ftp.Test.NetCore.Ftps
{
    public class FtpsTestMethods : FtpsBaseTestMethods
    {
        public FtpsTestMethods() : base(NetPlatformType.NETCORE) { }
    }
}
