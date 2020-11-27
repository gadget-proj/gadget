using Gadget.Messaging;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace Gadget.Inspector.Metrics.Services
{
    public class WindowsServiceCheck
    {
        private readonly IDictionary<string, ServiceControllerStatus> _statuses;
        private readonly string _machineName;
        public WindowsServiceCheck()
        {
            _statuses = ServiceController.GetServices().Select(x => new { x.ServiceName, x.Status }).ToDictionary(x => x.ServiceName, x => x.Status);
            _machineName = Environment.MachineName;
        }

        public RegisterNewAgent RegisterServices()
        {
            var report = new RegisterNewAgent
            {
                Agent = _machineName,
                Services = ServiceController.GetServices().Select(x => new Service
                {
                    Name = x.ServiceName,
                    Status = x.Status.ToString()
                })
            };
            return report;
        }

        public IEnumerable<ServiceStatusChanged> CheckServices()
        {
            var output = new List<ServiceStatusChanged>();
            foreach (var service in ServiceController.GetServices())
            {
                if (service.Status != _statuses[service.ServiceName])
                {
                    output.Add(new ServiceStatusChanged 
                    { 
                        AgentId = _machineName, 
                        Name = service.ServiceName, 
                        Status = service.Status.ToString() 
                    });
                    _statuses[service.ServiceName] = service.Status;
                }
            }
            return output;
        }
    }
}


