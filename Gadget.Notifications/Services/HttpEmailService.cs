using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Options;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Notifications.Services
{
    public class HttpEmailService : IEmailService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpEmailService> _logger;
        private  HttpEmailOptions _settings;

        public HttpEmailService(HttpClient client, ILogger<HttpEmailService> logger, IOptions<HttpEmailOptions> settings)
        {
            _client = client;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task SendEmailMessage(EmailMessage message, CancellationToken cancellationToken)
        {
            
            var loginResponse = await _client.PostAsJsonAsync(_settings.LoginUrl, new { login = _settings.Login, _settings.Password });
            var token = await loginResponse.Content.ReadAsStringAsync();
            _logger.LogInformation(token);

            await _client.PostAsJsonAsync(_settings.SendUrl, new
            {
                mailTo = message.Receiver,
                sendWithTemplate= false,
                subject = "Monitor Usług powiadomieni",
                mainMessage = message.Body,
                accessToken = token,
                userName = _settings.Login
            });
        }
    }
}
