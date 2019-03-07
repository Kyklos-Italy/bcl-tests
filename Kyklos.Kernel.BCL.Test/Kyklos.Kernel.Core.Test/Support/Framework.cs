using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kyklos.Kernel.Core.Test.Support
{
    public class Framework
    {
        public enum FrameworkType
        {
            NETCORE,
            NETFRAMEWORK
        }

        public string BinFolder { get; }
        public string ResourceFolder { get; }

        public Framework(FrameworkType frameworkType)
        {
            BinFolder = (frameworkType == FrameworkType.NETCORE) ? Assembly.GetExecutingAssembly().Location : new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            ResourceFolder = Path.GetDirectoryName(BinFolder) + "/Resources";
        }
    }
}
