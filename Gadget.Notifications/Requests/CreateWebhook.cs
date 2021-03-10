using Gadget.Notifications.Domain.Enums;

namespace Gadget.Notifications.Requests
{
    public record CreateWebhook(string Receiver, NotifierType NotifierType);
}