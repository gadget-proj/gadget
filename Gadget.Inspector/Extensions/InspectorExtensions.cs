using System;
using System.Diagnostics;
using System.Threading.Channels;
using Gadget.Inspector.Metrics;
using Gadget.Inspector.Transport;
using Gadget.Messaging.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Inspector.Extensions
{
    public static class InspectorExtensions
    {
        public static IServiceCollection AddInspector(this IServiceCollection services, IConfiguration configuration)
        {
            var controlPlaneBaseAddress =
                configuration.GetConnectionString("ControlPlane") ?? "https://localhost:5001/";
            var controlPlaneAddress = $"{controlPlaneBaseAddress}/gadget";
            services.AddScoped(_ =>
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithAutomaticReconnect()
                    .WithUrl(controlPlaneAddress)
                    .Build();
                hubConnection.StartAsync().GetAwaiter().GetResult();
                return hubConnection;
            });
            services.AddScoped(_ => new PerformanceCounter("Processor", "% Processor Time", "_Total"));
            services.AddScoped<InspectorResources>();
            services.AddScoped<IControlPlane, ControlPlane>();
            services.AddScoped(_ => Channel.CreateUnbounded<ServiceStatusChanged>());
            services.AddScoped(_ => new Uri(controlPlaneAddress));
            services.AddHostedService<Services.Inspector>();
            return services;
        }
    }
}