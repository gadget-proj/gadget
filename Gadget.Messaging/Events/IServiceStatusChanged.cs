namespace Gadget.Messaging.Events
{
    public interface IServiceStatusChanged
    {
        string  Agent { get; }
        string ServiceName { get; }
        string Status { get; }
    }
}