using System;
using System.Threading.Tasks;
using Gadget.Inspector.Consumers;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

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
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var id = Guid.NewGuid().ToString();
                            Console.WriteLine(id);
                            cfg.ReceiveEndpoint(id, e =>
                            {
                                // e.ConfigureConsumer<StopServiceConsumer>(context);
                                // e.ConfigureConsumer<StartServiceConsumer>(context);
                                // e.ConfigureConsumer<GetAgentHealthConsumer>(context);
                                e.Bind<IStopService>(c =>
                                {
                                    c.RoutingKey = id;
                                    c.ExchangeType = ExchangeType.Direct;
                                });
                                e.Bind<IStartService>(c =>
                                {
                                    c.RoutingKey = id;
                                    c.ExchangeType = ExchangeType.Direct;
                                });
                            });
                        });
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