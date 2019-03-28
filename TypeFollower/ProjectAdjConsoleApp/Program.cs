using System;
using System.Linq;
using Common.Logging;
using ProjectAdj;
using ProjectAdjConsoleApp.Properties;

namespace ProjectAdjConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            args =
                new string[]
                {
                    @"C:\development\dotnet\GitFashion\KComponents\Dev\KTemplating\KTemplating.Core\KTemplating.Core.csproj",
                    @"C:\temp\KTemplating\KTemplating.Core\",
                    @"C:\development\Git\bcl-tests\TypeFollower\ProcessingFolder\cmp-results.json"
                };

            DoWork(args);
        }

        static void DoWork(string[] args)
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
