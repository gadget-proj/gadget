using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class RestartServiceConsumer : IConsumer<IRestartService>
    {
        private readonly ILogger<RestartServiceConsumer> _logger;

        public RestartServiceConsumer(ILogger<RestartServiceConsumer> logger)
        {
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<IRestartService> context)
        {
            var serviceNormalizedName = context.Message.ServiceName.Trim().ToLower();
            _logger.LogInformation($"Trying to start {serviceNormalizedName}");
            var service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == serviceNormalizedName);
            if (service == null)
            {
                throw new ApplicationException($"Service {serviceNormalizedName} could not be found");
            }

            try
            {
                var timeout = TimeSpan.FromMilliseconds(500);
                await RestartRunningService(service, timeout);
                await context.RespondAsync<IActionResultResponse>(new
                {
                    Success = true
                });
            }
            catch
            {
                await context.RespondAsync<IActionResultResponse>(new
                {
                    Success = false
                });
                _logger.LogError($"Could not restart service {context.Message.Agent}{serviceNormalizedName}");
            }
        }


        private static Task RestartRunningService(ServiceController service, TimeSpan timeout)
        {
            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            return Task.CompletedTask;
        }
    }
}