using System;
using System.Threading.Channels;
using Gadget.Inspector.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gadget.Inspector
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddScoped(_ => Channel.CreateUnbounded<string>());
                    services.AddHostedService<ServicesWatcher>();
                    services.AddTransient(_ => new Uri("https://localhost:44347/gadget"));
                    services.AddHostedService<Inspector>();
                    services.AddHostedService<Producer>();
                });
    }
}