using System;

namespace Gadget.Messaging.Commands
{
    public class StopService
    {
        public string Agent { get; set; }
        public string ServiceName { get; set; }
    }
}