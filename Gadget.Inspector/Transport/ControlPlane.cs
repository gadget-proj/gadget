using Gadget.Messaging.Commands;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Gadget.Inspector.Transport
{
    public interface IControlPlane
    {
        Task Invoke(string method, object payload);
        void RegisterHandler<T>(string method, Action<T> handler) where T : IGadgetMessage;
    }

    public class ControlPlane : IControlPlane
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<ControlPlane> _logger;

        public ControlPlane(HubConnection hubConnection, ILogger<ControlPlane> logger)
        {
            _hubConnection = hubConnection;
            //_hubConnection.On<GetAgentHealth>("GetAgentHealth", @event => Console.WriteLine($"wesz³o 1111111112222{@event.CpuPercentUsage}"));
            _logger = logger;
        }

        public async Task Invoke(string method, object payload)
        {
            _logger.LogInformation($"Trying to invoke {method} with payload {payload}");
            try
            {
                await _hubConnection.InvokeAsync(method, payload);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
        }


        public void RegisterHandler<T>(string method, Action<T> handler) where T : IGadgetMessage
        {
            _logger.LogInformation($"Registering handler for method {method}, T : {typeof(T)}");
            Console.WriteLine($"Registering handler for method {method}, T : {typeof(T)}");
            try
            {
                _hubConnection.On(method, handler);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
        }
    }
}