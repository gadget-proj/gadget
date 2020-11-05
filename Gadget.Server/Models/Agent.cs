using Gadget.Messaging.ServiceMessages;
using System;
using System.Collections.Generic;

namespace Gadget.Server.Models
{
    public class Agent
    {
        public Guid MachineId { get; set; }

        public string  ConectId { get; set; }

        public MachineHealthDataModel MachineHealthData { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();

    }
}
