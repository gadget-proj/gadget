using Gadget.Messaging.Commands;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class StopServiceHandler : IHandler<StopService>
    {
        public void StopService()
        {
            System.Console.WriteLine("StopServiceHandler works");
        }
    }
}
