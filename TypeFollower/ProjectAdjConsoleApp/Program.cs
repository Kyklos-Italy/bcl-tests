using System;
using System.Linq;
using System.Threading.Tasks;
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
                    //@"C:\development\dotnet\GitFashion\KComponents\Dev\KTemplating\KTemplating.Core\KTemplating.Core.csproj",
                    @"C:\development\dotnet\GitFashion\KComponents\Dev\KTemplating\KTemplating.sln",
                    // @"C:\temp\KTemplating\KTemplating.Core\",
                    @"C:\development\Git\bcl-tests\TypeFollower\ProcessingFolder\cmp-results.json"
                };

            DoWork(args).Wait();
        }


        private static async Task DoWork(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog Logger = LogManager.GetLogger("ProjectAdjConsoleApp");

            try
            {
                Logger.Debug("Starting app");
                if (args.Count() < 2)
                    Console.WriteLine("Wrong number of arguments!");
                else
                {
                    using (SolutionAdj solAdj = new SolutionAdj(args[0], args[1], Settings.Default.NugetPackageRepositoryEndpointUrl))
                    {
                        await solAdj.Adjust().ConfigureAwait(false);
                    }
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

        private static async Task DoWorkOld(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog Logger = LogManager.GetLogger("ProjectAdjConsoleApp");
            
            try
            {
                Logger.Debug("Starting app");
                if (args.Count() < 2)
                    Console.WriteLine("Wrong number of arguments!");
                else
                {
                    using (ProjectMapper pb = new ProjectMapper(args[0], args[1])) //, args[2]);
                    {
                        await pb.MapProjectAsync(Settings.Default.NugetPackageRepositoryEndpointUrl).ConfigureAwait(false);
                    }
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
