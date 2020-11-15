using Gadget.Messaging.RegistrationMessages;
using Gadget.Messaging.ServiceMessages;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gadget.Inspector.Services
{
    public class WIndowsServiceWatcher : BackgroundService
    {
        private RegisterMachineReport _services;
        private readonly Guid _id;
        //private readonly IEquatable<>
        private readonly ChannelWriter<RegisterMachineReport> _registerChannel;
        public WIndowsServiceWatcher(
            ChannelWriter<RegisterMachineReport> registerChannel,
            Guid id)
            
        {
            _registerChannel = registerChannel;
            _id = id;
            _services = new RegisterMachineReport
            {
                Machine = Environment.MachineName,
                AgentId = _id,
                Services = ServiceController.GetServices().Select(x => new Service
                {
                    Name = x.ServiceName, Status = x.Status.ToString()
                })
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in _services.Services)
                {
                }
            }
            return Task.CompletedTask;
        }
    }
}
