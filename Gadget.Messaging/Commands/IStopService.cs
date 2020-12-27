namespace Gadget.Messaging.Commands
{
    public interface IStopService
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}