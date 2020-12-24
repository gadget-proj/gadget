using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service = Gadget.Messaging.Service;

namespace Gadget.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetExecutingAssembly());
                x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
            });
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
                ;
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