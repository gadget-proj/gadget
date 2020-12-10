using Gadget.Inspector.Metrics;
using Gadget.Messaging.Commands;
using System.Diagnostics;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class GetAgentHealthHandler: IHandler<GetAgentHealth>
    {
        //public GetAgentHealth GetAgentHealth() 
        //{
        //    System.Console.WriteLine("get agent health handler");
        //    var logic = new InspectorResources(new PerformanceCounter("Processor", "% Processor Time", "_Total"));
        //    return logic.GetMachineHealthData();
        //}

        public void GetAgentHealth()
        {
            System.Console.WriteLine("Get health weszło inspector");
        }
    }
}
