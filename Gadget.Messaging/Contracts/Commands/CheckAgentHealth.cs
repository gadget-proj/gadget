namespace Gadget.Messaging.Contracts.Commands
{
    public interface CheckAgentHealth
    {
        string Agent { get; set; }
        bool IsAlive { get;}
    }
}
