# bcl-tests
Xunit tests for Kyklos BCL

**This is a test markdown text**

```csharp
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;
using KBizEventStore.Amqp.Service.Settings;
using KBizEventStore.Core.Security;
using Microsoft.Extensions.Configuration;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Support;

namespace KBizEventStore.Amqp.Service
{
    internal class Program
    {
        private const string ServiceSettingsJsonFileName = "kbizeventstore.amqp.service.settings.json";

        static void Main(string[] args)
        {
            SetCurrentDicrectory(args.Contains("--console"));
            ConfigureLogging();
            StartAsService();
        }

        private static void SetCurrentDicrectory(bool isConsole)
        {
            var isService = !(Debugger.IsAttached || isConsole);

            if (isService)
            {
                var pathToExe = typeof(Program).Assembly.Location;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }
        }

        private static void ConfigureLogging()
        {
            ConfigureNLog();
            Topshelf.Logging.NLogLogWriterFactory.Use();
            MassTransit.NLogIntegration.Logging.NLogLogger.Use();
        }

        private static void ConfigureNLog()
        {
            var props =
                new global::Common.Logging.Configuration.NameValueCollection
                {
                    { "configType", "FILE" },
                    { "configFile", "./nlog.config" }
                };

            LogManager.Adapter = new global::Common.Logging.NLog.NLogLoggerFactoryAdapter(props);
        }

        private static void StartAsService()
        {
            var host =
                HostFactory
                .New
                (
                    hostConfigurator =>
                    {
                        var serviceSettings = GetServiceSettings();

                        hostConfigurator
                        .Service<ApplicationService>
                        (
                            srvConfigurator =>
                            {                                
                                srvConfigurator.ConstructUsing(s => new ApplicationService(serviceSettings));
                                srvConfigurator.WhenStarted(async service => await service.StartAsync());
                                srvConfigurator.WhenStopped(async service => await service.StopAsync());
                            }
                        );

                        ConfigureServiceHost(hostConfigurator, serviceSettings.WinServiceInfo);
                    }
                );

            host.Run();
        }

        private static void ConfigureServiceHost(HostConfigurator hostConfigurator, WinServiceInfo winServiceInfo)
        {
            ServiceStartInfo serviceStartInfo =
                new ServiceStartInfo
                (
                    winServiceInfo.ServiceName,
                    winServiceInfo.ServiceDisplayName,
                    winServiceInfo.ServiceDescription,
                    winServiceInfo.RunAsMode,
                    winServiceInfo.StartMode,
                    winServiceInfo.ServiceUserName,
                    winServiceInfo.ServiceUserPasswordClear()
                );

            hostConfigurator.SetStartAndRunMode(serviceStartInfo);
        }

        private static ServiceSettings GetServiceSettings()
        {
            var builder =
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ServiceSettingsJsonFileName, optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var settingsSection = configuration.GetSection("Settings");
            ServiceSettings serviceSettings = settingsSection.Get<ServiceSettings>();
            return serviceSettings;
        }
    }
}

```
