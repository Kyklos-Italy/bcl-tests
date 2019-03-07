using Kyklos.Kernel.Ftp.Test.Ftps;
using System;
using System.Collections.Generic;
using System.Text;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.NetCore.Ftps
{
    public class FtpsTestMethods : FtpsBaseTestMethods
    {
        public FtpsTestMethods() : base(FrameworkType.NETCORE) { }

        //[Fact(DisplayName = "Pippo")]
        //public void Pippo()
        //{
        //    PippoCore();
        //}
    }
}
