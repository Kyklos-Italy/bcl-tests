using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Kyklos.Kernel.Reactive;
using Kyklos.Kernel.Reactive.Test.Support.Mock;
using static Kyklos.Kernel.Reactive.Test.Support.Framework;

namespace Kyklos.Kernel.Reactive.Test.Reactive
{
    public class ReactiveBaseTestMethods
    {
        private MockData MockData;

        public ReactiveBaseTestMethods(FrameworkType frameworkType)
        {
            MockData = new MockData(frameworkType);
        }

        protected void PippoCore()
        {

            Assert.True(true);
        }
    }
}
