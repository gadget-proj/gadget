using System;
using System.Collections.Generic;
using Gadget.Notifications.Domain.Enums;

namespace Gadget.Notifications.Dto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Agent { get; set; }
        public string Service { get; set; }
        public IEnumerable<NotifierDto> Notifiers { get; set; }
    }

    public class NotifierDto
    {
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string Receiver { get; set; }
    }
}