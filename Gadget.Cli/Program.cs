using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Gadget.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Cli
{
    [Command("hello", Description = "hello command")]
    public class HelloCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            await console.Output.WriteLineAsync("coo");
        }
    }

    public class Program
    {
        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddTransient<HelloCommand>();
            services.AddTransient<GetAgentsCommand>();
            services.AddTransient<GetAgentServicesCommand>();
            return services.BuildServiceProvider();
        }

        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .SetDescription("Demo application showcasing CliFx features.")
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(GetServiceProvider().GetRequiredService)
                .Build()
                .RunAsync();
    }
}