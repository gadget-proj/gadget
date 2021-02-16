using System;

namespace Gadget.Server.Dto
{
    public class EventDto
    {
        public string Agent { get; set; }
        public string Service { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
