using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kyklos.Kernel.Ftp.Test.Support
{
    public class Framework
    {
        public enum FrameworkType
        {
            NETCORE,
            NETFRAMEWORK
        }

        protected string BinFolder { get; }
        
        public Framework(FrameworkType frameworkType)
        {
            BinFolder = (frameworkType == FrameworkType.NETCORE) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
        }
    }
}
