using Gadget.Messaging;
using Gadget.Messaging.ServiceMessages;
using System;
using System.Collections.Generic;

namespace Gadget.Server.Models
{
    public class Agent
    {
        public Agent(Guid machineId, string connectionId, IEnumerable<Service> services)
        {
            MachineId = machineId;
            ConnectionId = connectionId;
            Services = services;
        }

        public Guid MachineId { get; private set; }

        public string ConnectionId { get; private set; }

        public MachineHealthDataModel MachineHealthData { get; set; }

        public IEnumerable<Service> Services { get; set; } = new List<Service>();
    }
}
