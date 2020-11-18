using System.Collections.Generic;

namespace Gadget.Messaging.Commands
{
    public class RegisterNewAgent : IGadgetMessage
    {
        public string Agent { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}