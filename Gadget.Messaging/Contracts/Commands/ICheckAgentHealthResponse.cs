namespace Gadget.Messaging.Contracts.Commands
{
    public interface ICheckAgentHealthResponse
    {
        bool IsAlive { get; }
    }
}
