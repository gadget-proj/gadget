namespace Gadget.Messaging.Contracts.Commands
{
    public interface CheckAgentHealth
    {
        string Agent {get;}
        bool IsAlive {get;}
    }
}
