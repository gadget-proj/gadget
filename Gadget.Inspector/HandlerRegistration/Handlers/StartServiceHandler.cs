using Gadget.Messaging.Commands;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class StartServiceHandler : IHandler<StartService>
    {
        public void Handle(StartService message)
        {
            System.Console.WriteLine(message.ServiceName);

        }
    }
}
