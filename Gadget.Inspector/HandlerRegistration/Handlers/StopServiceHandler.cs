using Gadget.Messaging.Commands;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class StopServiceHandler : IHandler<StopService>
    {
        public void Handle(StopService message)
        {
            throw new System.NotImplementedException();
        }

        //public void StopService()
        //{
        //    System.Console.WriteLine("StopServiceHandler works");
        //}
    }
}
