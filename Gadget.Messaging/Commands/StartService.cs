using System;

namespace Gadget.Messaging.Commands
{
    public class StartService
    {
        public string Agent { get; set; }
        public string ServiceName { get; set; }
    }
}