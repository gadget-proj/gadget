using System.Collections.Generic;
using Gadget.Server.Domain.Entities;

namespace Gadget.Server.Agents.Dto
{
    public class AgentDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class ServiceDto
    {
        public ServiceDto(string name, string status, string logOnAs, string description,
            IEnumerable<ServiceEvent> serviceEvents)
        {
            Name = name;
            Status = status;
            LogOnAs = logOnAs;
            Description = description;
            Events = serviceEvents;
        }

        public string Name { get; }
        public string Status { get; }
        public IEnumerable<ServiceEvent> Events { get; }
        public string LogOnAs { get;  }
        public string Description { get;  }
    }
}