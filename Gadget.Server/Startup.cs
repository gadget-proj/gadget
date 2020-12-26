using System.Collections.Generic;
using Gadget.Server.Agents.Consumers;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
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
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddSignalR();
            services.AddControllers();
            services.AddSingleton<ICollection<Agent>>(new List<Agent>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseCors("AllowAll");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapHub<GadgetHub>("/gadget");
                endpoints.MapControllers();
            });
        }
    }
}