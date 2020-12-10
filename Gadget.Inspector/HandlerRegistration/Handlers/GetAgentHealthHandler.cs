using Gadget.Messaging.Commands;

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

        public void GetAgentHealth(GetAgentHealth health)
        {
            System.Console.WriteLine("Get health weszło inspector");
        }
    }
}
