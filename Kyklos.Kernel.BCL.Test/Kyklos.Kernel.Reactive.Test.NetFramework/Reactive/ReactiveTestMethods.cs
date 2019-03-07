using Kyklos.Kernel.Reactive.Test.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kyklos.Kernel.Reactive.Test.Support.Framework;

namespace Kyklos.Kernel.Reactive.Test.NetFramework.Reactive
{
    public class ReactiveTestMethods : ReactiveBaseTestMethods
    {
        public ReactiveTestMethods() : base(FrameworkType.NETFRAMEWORK) { }

        [Fact(DisplayName = "Pippo")]
        public void Pippo()
        {
            PippoCore();
        }
    }
}
