using Kyklos.Kernel.Ftp.Test.Ftps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.NetFramework.Ftps
{
    public class FtpsTestMethods : FtpsBaseTestMethods
    {
        public FtpsTestMethods() : base(FrameworkType.NETFRAMEWORK) { }

        //[Fact(DisplayName = "Pippo")]
        //public void Pippo()
        //{
        //    PippoCore();
        //}
    }
}
