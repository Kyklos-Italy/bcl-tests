using Common.Logging;
using ProjectAdj;
using ProjectAdjConsoleApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAdjConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog Logger = LogManager.GetLogger("ProjectAdjConsoleApp");
            
            try
            {
                Logger.Debug("Starting app");
                if (args.Count() < 3)
                    Console.WriteLine("Wrong number of arguments!");
                else
                {
                    ProjectMapper pb = new ProjectMapper(args[0], args[1], args[2]);
                    pb.MapProject(Settings.Default.NugetPackageRepositoryEndpointUrl);
                    Console.WriteLine($"Completed");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
