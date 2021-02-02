namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IStopService
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}