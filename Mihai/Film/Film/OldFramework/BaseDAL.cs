using Kyklos.Kernel.Core.Asserts;
using Kyklos.Kernel.Data.Async;
using System;
using System.Collections.Generic;
using System.Text;

namespace Film.OldFramework
{
    public abstract  class BaseDAL
    {
        public IAsyncDao Dao { get; set; }
        
    }
}
