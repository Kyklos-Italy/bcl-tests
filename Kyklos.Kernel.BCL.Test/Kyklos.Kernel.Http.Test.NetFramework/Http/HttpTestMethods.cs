using Kyklos.Kernel.Http.Test.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.Http.Test.NetFramework.Http
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
