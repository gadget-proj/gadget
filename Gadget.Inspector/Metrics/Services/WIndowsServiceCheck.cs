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
        private readonly IEnumerable<ServiceController> _services;
        private readonly IDictionary<string, ServiceControllerStatus> _statuses;
        private readonly string _machineName;
        public WindowsServiceCheck()
        {
            _services = ServiceController.GetServices().ToList(); // will inject it later
            _statuses = new Dictionary<string, ServiceControllerStatus>(); // as above
            _machineName = Environment.MachineName;
        }

        public RegisterNewAgent RegisterServices()
        {
            var report = new RegisterNewAgent
            {
                Agent = _machineName,
                Services = _services.Select(x => new Service
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
            foreach (var service in _services)
            {
                var lastStatus = _statuses.First(x => x.Key == service.ServiceName).Value;
                if (service.Status != lastStatus)
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


