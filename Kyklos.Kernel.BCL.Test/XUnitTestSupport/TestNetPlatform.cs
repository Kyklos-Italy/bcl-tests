using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace XUnitTestSupport
{
    public class TestNetPlatform
    {
        public string BinFolder { get; }
        public string ResourceFolder { get; }

        private static object lockObj = new object();
        private static bool isInitialized = false;


        public TestNetPlatform(NetPlatformType frameworkType)
        {
            BinFolder = (frameworkType == NetPlatformType.NETCORE) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            ResourceFolder = Path.GetDirectoryName(BinFolder) + "/Resources";
            InitTestEnvironment(frameworkType);
        }

        private static void InitTestEnvironment(NetPlatformType frameworkType)
        {
            if (frameworkType == NetPlatformType.NETCORE)
            {
                if (!isInitialized)
                {
                    lock (lockObj)
                    {
                        if (!isInitialized)
                        {
                            JsonConvert.DefaultSettings =
                                () =>
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.None,
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    };
                            isInitialized = true;
                        }
                    }
                }
            }
        }
    }
}
