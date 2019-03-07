using Kyklos.Kernel.Ftp.Test.Ftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kyklos.Kernel.Ftp.Test.Support.Framework;

namespace Kyklos.Kernel.Ftp.Test.NetFramework.Ftp
{
    public class FtpTestMethods : FtpBaseTestMethods
    {
        public FtpTestMethods() : base(FrameworkType.NETFRAMEWORK) { }

        //[Fact(DisplayName = "Pippo")]
        //public void Pippo()
        //{
        //    PippoCore();
        //}
    }
}
