using System;
using System.IO;
using System.Reflection;

namespace Kyklos.Kernel.Compression.Test.Support
{
    public class Framework
    {
        public enum FrameworkType
        {
            NETCORE,
            NETFRAMEWORK
        }

        protected string BinFolder { get; }
        public string ResourceFolder { get; }

        public Framework(FrameworkType frameworkType)
        {
            BinFolder = (frameworkType == FrameworkType.NETCORE) ? Assembly.GetExecutingAssembly().Location : new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            ResourceFolder = Path.GetDirectoryName(BinFolder) + "/Resources";
        }
    }
}
