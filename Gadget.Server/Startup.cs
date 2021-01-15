using System.Collections.Generic;
using System.Threading.Channels;
using Gadget.Server.Agents.Consumers;
using Gadget.Server.Agents.Services;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Hubs;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GadgetContext>(builder => builder.UseSqlite("Data Source=gadget.db"));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ServiceStatusChangedConsumer>();
                x.AddConsumer<RegisterNewAgentConsumer>();
                x.AddConsumer<MachineHealthConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetConnectionString("RabbitMq"),
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<GadgetContext>())
                {
                    logger.LogCritical("ensurecreated");
                    context.Database.EnsureCreated();
                }
            }

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseCors("AllowAll");
            app.UseFileServer();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GadgetHub>("/gadget");
                endpoints.MapControllers();
            });
        }
    }
}