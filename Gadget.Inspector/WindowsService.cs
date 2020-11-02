using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    internal class WindowsService : IWindowsService
    {
        private readonly ServiceController _serviceController;
        private ServiceControllerStatus _lastKnownStatus;
        public EventHandler<WindowsServiceStatusChanged> StatusChanged;

        public ServiceControllerStatus Status
        {
            get
            {
                _serviceController.Refresh();
                var currentStatus = _serviceController.Status;
                if (_lastKnownStatus != currentStatus)
                {
                    //Status has changed, notify the server
                }

                _lastKnownStatus = currentStatus;
                return currentStatus;
            }
        }

        public WindowsService(ServiceController serviceController)
        {
            _serviceController = serviceController;
            StartWatcher();
        }

        private void StartWatcher()
        {
            //Possibly stealing thread from thread pool and never returning it
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    _serviceController.Refresh();
                    var currentStatus = _serviceController.Status;
                    if (currentStatus != _lastKnownStatus)
                    {
                        Console.WriteLine($"{_serviceController.DisplayName} has changed, new status {currentStatus}");
                        StatusChanged.Invoke(this, new WindowsServiceStatusChanged
                        {
                            ServiceName = _serviceController.ServiceName,
                            Status = currentStatus
                        });
                    }

                    _lastKnownStatus = currentStatus;
                    await Task.Delay(1000);
                }
                // ReSharper disable once FunctionNeverReturns
            });
        }

        public void Start() => _serviceController.Start();
        public void Stop() => _serviceController.Stop();
    }
}