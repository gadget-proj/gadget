namespace Gadget.Messaging.Contracts.Events
{
    public interface IServiceStatusChanged
    {
        string Agent { get; }
        string Name { get; }
        string Status { get; }
    }
}