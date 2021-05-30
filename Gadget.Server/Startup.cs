using System.Text;
using System.Threading.Channels;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Server.Consumers;
using Gadget.Server.HealthCheck;
using Gadget.Server.Persistence;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Gadget.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                var secret = Configuration.GetValue<string>("SecurityKey");
                var key = Encoding.ASCII.GetBytes(secret);
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddLogging(cfg => cfg.AddSeq());
            services.AddDbContext<GadgetContext>(builder =>
                builder.UseSqlServer(Configuration.GetConnectionString("MsSql")));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ServiceStatusChangedConsumer>();
                x.AddConsumer<RegisterNewAgentConsumer>();
                x.AddConsumer<ActionFailedConsumer>();
                x.AddConsumer<ActionResultConsumer>();
                x.AddRequestClient<CheckAgentHealth>();
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
                            .WithOrigins("https://localhost:5005")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
            services.AddControllers();
            services.AddSingleton(Channel.CreateUnbounded<IServiceStatusChanged>());
            services.AddTransient<IAgentsService, AgentsService>();
            services.AddTransient<IGroupsService, GroupsService>();
            services.AddTransient<ISelectorService, SelectorService>();
            services.AddTransient<IActionsService, ActionsService>();
            services.AddHostedService<AgentHealthCheck>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Gadget", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<GadgetContext>())
                {
                    context?.Database.EnsureCreated();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication5 v1"));
            }

            app.UseCors("AllowAll");
            app.UseFileServer();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}