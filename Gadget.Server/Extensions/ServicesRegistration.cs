using Gadget.Messaging.Contracts.Commands;
using Gadget.Server.Agents;
using Gadget.Server.Agents.Consumers;
using Gadget.Server.Agents.HealthCheck;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Notifications.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Gadget.Server.Extensions
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddGadget(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GadgetContext>(builder => builder.UseSqlite("Data Source=gadget.db"));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ServiceStatusChangedConsumer>();
                x.AddConsumer<RegisterNewAgentConsumer>();
                x.AddConsumer<MachineHealthConsumer>();
                x.AddRequestClient<CheckAgentHealth>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("RabbitMq"),
                        configurator =>
                        {
                            configurator.Username("guest");
                            configurator.Password("guest");
                        });
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsBuilder =>
                    {
                        corsBuilder
                            .WithOrigins("localhost:3000")
                            .WithOrigins("http://localhost:3000")
                            .WithOrigins("localhost:5000")
                            .WithOrigins("http://localhost:5000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
            services.AddSingleton(_ => Channel.CreateUnbounded<Notification>());
            services.AddHostedService<NotificationsService>();
            services.AddSignalR();
            services.AddControllers();
            services.AddSingleton<ICollection<Agent>>(new List<Agent>());
            services.AddTransient<IAgentsService, AgentsService>();
            services.AddHostedService<AgentHealthCheck>();
            return services;
        }
    }
}