using System;
using System.Threading.Tasks;
using CliFx;
using Gadget.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Cli
{
    public class Program
    {
        private static IServiceProvider GetServiceProvider()
        {
            var cconfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
            var auth = cconfiguration.GetConnectionString("Auth");
            var ctl = cconfiguration.GetConnectionString("Ctl");

            var services = new ServiceCollection();
            services.AddHttpClient<GetAgentsCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<GetGroupsCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<GetAgentServicesCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<LoginCommand>(client => { client.BaseAddress = new Uri(auth); });
            services.AddHttpClient<CreateUserCommand>(client => { client.BaseAddress = new Uri(auth); });
            services.AddHttpClient<StopServiceCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<AddToGroupCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<CreateNewGroupCommand>(client => { client.BaseAddress = new Uri(ctl); });
            services.AddHttpClient<StopGroupCommand>(client => { client.BaseAddress = new Uri(ctl); });
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