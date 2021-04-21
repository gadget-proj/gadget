using System;
using System.Threading.Tasks;
using CliFx;
using Gadget.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Cli
{
    public class Program
    {
        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddTransient<GetAgentsCommand>();
            services.AddTransient<GetGroupsCommand>();
            services.AddTransient<GetAgentServicesCommand>();
            services.AddTransient<LoginCommand>();
            services.AddTransient<StopServiceCommand>();
            services.AddTransient<AddToGroupCommand>();
            services.AddTransient<CreateNewGroupCommand>();
            services.AddTransient<StopGroupCommand>();
            return services.BuildServiceProvider();
        }

        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .SetDescription("Gadget CLI")
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(GetServiceProvider().GetRequiredService)
                .Build()
                .RunAsync();
    }
}