using Kyklos.Kernel.Threading.Test.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.Threading.Test.NetCore.Threading
{
    public class ThreadingTestMethods : ThreadingBaseTestMethods
    {
        [Fact(DisplayName= "SimpleTaskRepeaterIsStarted")]
        public void SimpleTaskRepeaterIsStarted()
        {
            SimpleTaskRepeaterIsStartedCore();
        }

        [Fact(DisplayName = "SimpleTaskAsyncRepeaterNewTask")]
        public void SimpleTaskAsyncRepeaterNewTask()
        {
            SimpleTaskAsyncRepeaterNewTaskCore();
        }

        [Fact(DisplayName = "SimpleTaskAsyncRepeaterNewAsyncTask")]
        public void SimpleTaskAsyncRepeaterNewAsyncTask()
        {
            SimpleTaskAsyncRepeaterNewAsyncTaskCore();
        }

        [Fact(DisplayName = "SimpleTaskAsyncRepeaterNewAsyncFromTimeInSeconds")]
        public void SimpleTaskAsyncRepeaterNewAsyncFromTimeInSeconds()
        {
            SimpleTaskAsyncRepeaterNewAsyncFromTimeInSecondsCore();
        }
    }
}
