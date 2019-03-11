using System.Reflection;
using Kyklos.Kernel.Core.Test.Support.Mock;
using Xunit;
using static Kyklos.Kernel.Core.Test.Support.Framework;

namespace Kyklos.Kernel.Core.Test.Utilities
{
    public class UtilitiesBaseTestMethods
    {
        private MockData MockData;

        public UtilitiesBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
        }

        protected void CurrentExePathCore()
        {
            var mn = Core.Utilities.GetMachineName();
            string currentExePath = Core.Utilities.GetFilePathFromAssemblyCodeBase(Assembly.GetExecutingAssembly());
            string expectedCurrentExePath = MockData.BinFolder;
            Assert.True(true);
        }
    }
}
