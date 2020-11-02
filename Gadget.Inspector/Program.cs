using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class Startup
    {
        private readonly Inspector _inspector;

        public Startup(Inspector inspector)
        {
            _inspector = inspector;
        }

        public async Task Run()
        {
            Console.WriteLine("Running");
            await _inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();
        }
    }

    internal class Program
    {
        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<Startup>();
            services.AddSingleton<Inspector>();
            services.AddTransient(_ => new Uri("https://localhost:5001/gadget"));
            services.AddSingleton<ICollection<int>>(_ => new List<int>());
            services.AddLogging();
            return services;
        }

        internal static async Task Main()
        {
            var services = ConfigureServices();
            var servicesProvider = services.BuildServiceProvider();
            var startup = servicesProvider.GetService<Startup>();
            await startup.Run();
        }
    }
}