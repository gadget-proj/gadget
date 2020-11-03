using Gadget.Inspector.Models;

namespace Gadget.Inspector.Services.Interfaces
{
    public interface IMachineHealthService
    {
        MachineHealthDataModel CheckMachineHealth();
    }
}
