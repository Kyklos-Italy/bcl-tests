using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Kyklos.Kernel.Threading;
using Kyklos.Kernel.Threading.ThreadHelper;
using Kyklos.Kernel.Threading.Threading;
using System.Threading;
using Kyklos.Kernel.Threading.Test.Support;

namespace Kyklos.Kernel.Threading.Test.Threading
{
    public class ThreadingBaseTestMethods
    {
        private MockData MockData = new MockData();

        protected void SimpleTaskRepeaterIsStartedCore()
        {
            int actualChickenNumber = 0;
            SimpleTaskRepeater chickenTask = SimpleTaskRepeater.NewTask(() =>
            {
                actualChickenNumber = IncrementChickenNumber(actualChickenNumber);
            },
            MockData.StartDelayInMillisecs, MockData.RepeatIntervalInMillisecs);
            chickenTask.Start();
            Assert.True(chickenTask.IsStarted);
        }

        protected void SimpleTaskAsyncRepeaterNewTaskCore()
        {
            int expectedChickenNumber = 40;
            int actualChickenNumber = 0;
            SimpleTaskRepeater chickenTask = SimpleTaskRepeater.NewTask(() =>
            {
                actualChickenNumber = IncrementChickenNumber(actualChickenNumber);
            },
            MockData.StartDelayInMillisecs, MockData.RepeatIntervalInMillisecs);
            chickenTask.Start();
            while (actualChickenNumber < expectedChickenNumber)
            {
                Thread.Sleep(1);
            }
            Assert.Equal(expectedChickenNumber, actualChickenNumber);
        }

        protected void SimpleTaskAsyncRepeaterNewAsyncTaskCore()
        {
            int expectedChickenNumber = 40;
            int actualChickenNumber = 0;
            SimpleTaskRepeater chickenTask = SimpleTaskRepeater.NewAsyncTask(() =>
            {
                actualChickenNumber = IncrementChickenNumber(actualChickenNumber);
            },
            MockData.RepeatIntervalInMillisecs);
            chickenTask.Start();
            while (actualChickenNumber < expectedChickenNumber)
            {
                Thread.Sleep(1);
            }
            Assert.Equal(expectedChickenNumber, actualChickenNumber);
        }

        protected void SimpleTaskAsyncRepeaterNewAsyncFromTimeInSecondsCore()
        {
            int expectedChickenNumber = 3;
            int actualChickenNumber = 0;
            SimpleTaskRepeater chickenTask = SimpleTaskRepeater.NewAsyncFromTimeInSeconds(() =>
            {
                actualChickenNumber = IncrementChickenNumber(actualChickenNumber);
            },
            MockData.StartDelayInSecs, MockData.StartDelayInSecs);
            chickenTask.Start();
            while (actualChickenNumber < expectedChickenNumber)
            {
                Thread.Sleep(1);
            }
            Assert.Equal(expectedChickenNumber, actualChickenNumber);
        }

        protected async Task PeriodicTaskFactoryCore()
        {
            int expectedChickenNumber = 2;
            int actualChickenNumber = 0;
            await PeriodicTaskFactory.Start(
            () =>
            {
                actualChickenNumber = IncrementChickenNumber(actualChickenNumber); 
            }, MockData.RepeatIntervalInMillisecs, MockData.StartDelayInMillisecs, MockData.Duration, expectedChickenNumber).ConfigureAwait(false);
            Assert.Equal(expectedChickenNumber, actualChickenNumber);
        }

        private int IncrementChickenNumber(int chickenNumber)
        {
            return chickenNumber + 1;
        }
    }
}
