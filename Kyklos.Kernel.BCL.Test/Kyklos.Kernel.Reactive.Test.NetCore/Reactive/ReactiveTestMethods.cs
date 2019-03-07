using Kyklos.Kernel.Reactive.Test.Reactive;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Kyklos.Kernel.Reactive.Test.Support.Framework;

namespace Kyklos.Kernel.Reactive.Test.NetCore.Reactive
{
    public class ReactiveTestMethods : ReactiveBaseTestMethods
    {
        public ReactiveTestMethods() : base(FrameworkType.NETCORE) { }

        [Fact(DisplayName = "Pippo")]
        public void Pippo()
        {
            PippoCore();
        }
    }
}
