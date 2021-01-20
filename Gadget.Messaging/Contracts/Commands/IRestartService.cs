namespace Gadget.Messaging.Contracts.Commands
{
    public interface IRestartService
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}
