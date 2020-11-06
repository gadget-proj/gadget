using System.ServiceProcess;

namespace Gadget.Inspector
{
    public class WindowsServiceStatusChanged
    {
        public string ServiceName { get; set; }
        public ServiceControllerStatus Status { get; set; }
    }
}