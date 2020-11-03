using Gadget.Messaging.ServiceMessages;
using System;
using System.Collections.Generic;

namespace Gadget.Messaging.RegistrationMessages
{
    public class RegisterMachineReport
    {
        public string Machine { get; set; }
        public Guid AgentId { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}