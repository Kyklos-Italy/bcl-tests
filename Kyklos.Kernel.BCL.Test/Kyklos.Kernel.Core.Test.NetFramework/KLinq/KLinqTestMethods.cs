using Kyklos.Kernel.Core.Test.KLinq;
using Xunit;

namespace Kyklos.Kernel.Core.Test.NetFramework.KLinq
{
    public class KLinqTestMethods : KLinqBaseTestMethods
    {
        [Fact(DisplayName = "IsNullOrEmptyList with null list should return true")]
        public void IsNullOrEmptyListWithNullListShouldReturnTrue()
        {
            IsNullOrEmptyListWithNullListShouldReturnTrueCore();
        }
    }
}
