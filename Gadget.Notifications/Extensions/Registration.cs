using System.Threading.Channels;
using Gadget.Notifications.BackgroundServices;
using Gadget.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddHttpClient<WebhooksService>();
            services.AddHostedService<WebhooksService>();
            services.AddSingleton(_ => Channel.CreateUnbounded<DiscordMessage>());
            return services;
        }
    }
}