using System.Threading.Channels;
using Gadget.Notifications.BackgroundServices;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Options;
using Gadget.Notifications.Services;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebhooksService = Gadget.Notifications.Services.WebhooksService;

namespace Gadget.Notifications.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddEmailNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(_ => Channel.CreateUnbounded<EmailMessage>());
            services.AddHostedService<EmailService>();
            services.AddHttpClient<IEmailService, HttpEmailService>();
            services.Configure<HttpEmailOptions>(o=> configuration.GetSection("HttpEmail").Bind(o)));
            return services;
        }

        public static IServiceCollection AddWebhooksNotifications(this IServiceCollection services)
        {
            services.AddHttpClient<IWebhooksService, WebhooksService>();
            services.AddHostedService<WebhooksBackgroundService>();
            services.AddSingleton(_ => Channel.CreateUnbounded<DiscordMessage>());
            return services;
        }
    }
}