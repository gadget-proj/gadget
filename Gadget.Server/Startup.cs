using System.Collections.Generic;
using System.Threading.Channels;
using Gadget.Server.Agents;
using Gadget.Server.Agents.Consumers;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Extensions;
using Gadget.Server.Hubs;
using Gadget.Server.Notifications.Services;
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
            services.AddGadget(Configuration);
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