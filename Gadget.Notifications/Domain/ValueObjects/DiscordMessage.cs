using System;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public record DiscordMessage (string Body, Uri Receiver);
}