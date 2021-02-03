using System;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public record Message (string Body, Uri Receiver);
}