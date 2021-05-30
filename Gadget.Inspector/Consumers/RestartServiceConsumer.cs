using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;

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
                var (success, error) = await RestartRunningService(service, timeout);
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = success, Reason = error
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"Could not restart service {context.Message.Agent}{serviceNormalizedName}");
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = false, Reason = exception.Message
                });
            }
        }


        private static Task<Result> RestartRunningService(ServiceController service, TimeSpan timeout)
        {
            try
            {
                Policy
                    .Handle<Win32Exception>()
                    .Or<InvalidOperationException>()
                    .WaitAndRetry(3, _ => timeout)
                    .Execute(
                        () =>
                        {
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                            }

                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        });
                return Task.FromResult(new Result(true, ""));
            }
            catch (Exception e)
            {
                return Task.FromResult(new Result(false, e.Message));
            }
        }
    }
}