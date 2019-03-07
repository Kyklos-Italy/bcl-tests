using Kyklos.Kernel.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Kyklos.Kernel.Core.Test.Support.Framework;

namespace Kyklos.Kernel.Core.Test.NetCore.Utilities
{
    public class UtilitiesTestMethods : UtilitiesBaseTestMethods
    {
        public UtilitiesTestMethods() : base(FrameworkType.NETCORE) { }

        [Fact(DisplayName = "CurrentExePath")]
        public void CurrentExePath()
        {
            CurrentExePathCore();
        }
    }
}
