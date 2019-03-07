using System;
using System.Collections.Generic;
using System.Text;

namespace Kyklos.Kernel.Threading.Test.Support
{
    internal class MockData
    {
        public int StartDelayInMillisecs { get; set; } = 50;
        public int RepeatIntervalInMillisecs { get; set; } = 10;
        public int StartDelayInSecs { get; set; } = 1;
        public int RepeatIntervalInSecs { get; set; } = 2;
        public int Duration { get; set; } = -1;
    }
}
