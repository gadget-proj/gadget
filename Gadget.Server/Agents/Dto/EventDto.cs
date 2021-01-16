using System;

namespace Gadget.Server.Agents.Dto
{
    public class EventDto
    {
        public string Agent { get; set; }
        public string Service { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
