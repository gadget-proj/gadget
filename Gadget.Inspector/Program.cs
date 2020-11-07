using System;
using System.Threading.Channels;
using Gadget.Inspector.Services;
using Gadget.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddScoped(_ => Channel.CreateUnbounded<ServiceStatusChanged>());
                    services.AddTransient(_ => new Uri("https://localhost:44347/gadget"));
                    services.AddHostedService<Services.Inspector>();
                });
        }
    }
}