namespace Gadget.Messaging.Contracts.Commands
{
    public interface IStopService
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}