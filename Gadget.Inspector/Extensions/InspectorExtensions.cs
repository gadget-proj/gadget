using System;
using System.Threading.Channels;
using Gadget.Messaging;
using Gadget.Messaging.ServiceMessages;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Inspector.Extensions
{
    public static class InspectorExtensions
    {
        public static IServiceCollection AddInspector(this IServiceCollection services)
        {
            services.AddScoped(_ => Channel.CreateUnbounded<ServiceStatusChanged>());
            services.AddScoped(_ => Channel.CreateBounded<MachineHealthDataModel>(150)); // its depend if we want to draw charts
            services.AddTransient(_ => new Uri("https://localhost:5001/gadget"));
            services.AddHostedService<Services.Inspector>();
            return services;
        }
    }
}