using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Gadget.Inspector.Services
{
    public class Producer : BackgroundService
    {
        private readonly Channel<string> _events;

        public Producer(Channel<string> events)
        {
            _events = events;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _events.Writer.WriteAsync(DateTime.UtcNow.ToLongDateString(), stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    }
}