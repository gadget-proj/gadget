using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Gadget.Inspector
{
    public class Inspector : BackgroundService
    {
        private IPublishEndpoint _publishEndpoint;

        public Inspector(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _publishEndpoint.Publish<IRegisterNewAgent>(new
            {
                Agent = Environment.MachineName,
                Services = ServiceController.GetServices().Select(s => new Service
                {
                    Name = s.ServiceName,
                    Status = s.Status.ToString()
                })
            }, stoppingToken);
        }
    }
}