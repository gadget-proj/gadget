using Gadget.Inspector.Services;
using Gadget.Inspector.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    class Program
    {
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<Startup>();
            services.AddSingleton<Inspector>();
            services.AddTransient(_ => new Uri("https://localhost:44347/gadget"));
            services.AddTransient<IMachineHealthService, MachineHealthService>();
            services.AddTransient<IWindowsService, WindowsService>();
            services.AddLogging();

            return services;
        }
        private static async Task Main()
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<Startup>().Run();
        }
    }
}