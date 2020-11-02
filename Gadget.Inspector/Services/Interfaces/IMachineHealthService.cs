using Gadget.Inspector.Models;

namespace Gadget.Inspector.Services.Interfaces
{
    internal interface IMachineHealthService
    {
        MachineHealthDataModel CheckMachineHealth();
    }
}
