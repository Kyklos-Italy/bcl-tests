using Kyklos.Kernel.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kyklos.Kernel.Core.Test.Support.Framework;

namespace Kyklos.Kernel.Core.Test.NetFramework.Utilities
{
    public class UtilitiesTestMethods : UtilitiesBaseTestMethods
    {
        public UtilitiesTestMethods() : base(FrameworkType.NETFRAMEWORK) { }

        [Fact(DisplayName = "CurrentExePath")]
        public void CurrentExePath()
        {
            CurrentExePathCore();
        }
    }
}
