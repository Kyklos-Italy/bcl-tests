using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Kyklos.Kernel.Http;

namespace Kyklos.Kernel.Http.Test.Http
{
    public class HttpBaseTestMethods
    {
        protected void PippoCore()
        {
            CookieWrapper[] cookies =
            {
                new CookieWrapper("pippo", "1"),
                new CookieWrapper("ciccio", "2"),
            };
            CookiesWrapperData cookiesWrapperData = new CookiesWrapperData("http://minerva.kyklos.it:9998", cookies);
            Assert.True(true);
        }
    }
}
