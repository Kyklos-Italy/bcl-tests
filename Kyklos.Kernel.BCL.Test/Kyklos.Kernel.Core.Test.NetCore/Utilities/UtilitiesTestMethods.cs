using Kyklos.Kernel.Core.Test.Utilities;
using Xunit;
using static XUnitTestSupport.TestNetPlatform;

namespace Kyklos.Kernel.Core.Test.NetCore.Utilities
{
    public class UtilitiesTestMethods : UtilitiesBaseTestMethods
    {
        public UtilitiesTestMethods() : base(NetPlatformType.NETCORE) { }

        [Fact(DisplayName = "CurrentExePath")]
        public void CurrentExePath()
        {
            CurrentExePathCore();
        }
    }
}
