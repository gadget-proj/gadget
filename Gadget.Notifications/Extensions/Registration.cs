using System.Threading.Channels;
using Gadget.Notifications.BackgroundServices;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using WebhooksService = Gadget.Notifications.Services.WebhooksService;

namespace Gadget.Notifications.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddEmailNotifications(this IServiceCollection services)
        {
            services.AddSingleton(_ => Channel.CreateUnbounded<EmailMessage>());
            services.AddHostedService<EmailService>();
            return services;
        }

        public static IServiceCollection AddWebhooksNotifications(this IServiceCollection services)
        {
            services.AddHttpClient<IWebhooksService, WebhooksService>();
            services.AddHostedService<BackgroundServices.WebhooksService>();
            services.AddSingleton(_ => Channel.CreateUnbounded<DiscordMessage>());
            return services;
        }
    }
}