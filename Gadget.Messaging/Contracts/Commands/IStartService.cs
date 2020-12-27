namespace Gadget.Messaging.Contracts.Commands
{
    public interface IStartService
    {
         string Agent { get;  }
         string ServiceName { get;  }
    }
}