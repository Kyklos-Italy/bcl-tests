using System.Collections.Generic;
using Kyklos.Kernel.Core.KLinq;
using Xunit;

namespace Kyklos.Kernel.Core.Test.KLinq
{
    public class KLinqBaseTestMethods
    {
        protected void IsNullOrEmptyListWithNullListShouldReturnTrueCore()
        {
            IEnumerable<string> list = null;
            bool actual = list.IsNullOrEmptyList();
            Assert.True(actual);
        }
    }
}
