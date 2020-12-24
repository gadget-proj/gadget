using System;
using System.Threading.Tasks;
using Gadget.Inspector.Consumers;
using Gadget.Messaging.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<GetAgentHealthConsumer>();
                        x.AddConsumer<StopServiceConsumer>();
                        x.AddConsumer<StartServiceConsumer>();
                        x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
                    });
                    services.AddMassTransitHostedService();
                    services.AddLogging(options => options.AddConsole());
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                });
        }
    }
}