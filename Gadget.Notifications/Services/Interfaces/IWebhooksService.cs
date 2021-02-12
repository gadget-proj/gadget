using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;

namespace Gadget.Notifications.Services.Interfaces
{
    public interface IWebhooksService
    {
        Task SendDiscordMessage(DiscordMessage message, CancellationToken cancellationToken);
    }
}