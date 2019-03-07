using Kyklos.Kernel.Http.Test.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kyklos.Kernel.Http.Test.NetCore.Http
{
    public class HttpTestMethods : HttpBaseTestMethods
    {
        [Fact(DisplayName = "Pippo")]
        public void Pippo()
        {
            PippoCore();
        }
        
    }
}
