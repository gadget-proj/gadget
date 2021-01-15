using System;

namespace Gadget.Server.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; }
        public string Message { get; }

        public Notification(string message)
        {
            Id = Guid.NewGuid();
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}