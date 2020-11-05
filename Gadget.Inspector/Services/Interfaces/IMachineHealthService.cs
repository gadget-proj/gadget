using Gadget.Messaging.ServiceMessages;

namespace Gadget.Inspector.Services.Interfaces
{
    public interface IMachineHealthService
    {
        MachineHealthDataModel CheckMachineHealth();
    }
}
