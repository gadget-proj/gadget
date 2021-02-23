using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
        //private readonly IConfiguration _configuration;

        public HttpEmailService(HttpClient client, ILogger<HttpEmailService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task SendEmailMessage(EmailMessage message, CancellationToken cancellationToken)
        {
            var sendEmailUrl = "https://newsletter.polskieradio.pl/api"; // new Uri(_configuration.GetValue<string>(""));

            var login = "gadget";
            var password = "";
            var loginResponse = await _client.PostAsJsonAsync(sendEmailUrl + "/login", new { login = login, password = password });

            var token = await loginResponse.Content.ReadAsStringAsync();
            _logger.LogInformation(token);

            var sendResponse = await _client.PostAsJsonAsync(sendEmailUrl + "/sendmail", new
            {
                mailTo = message.Receiver,
                sendWithTemplate= false,
                subject = "Monitor Usług powiadomienie",
                mainMessage = message.Body,
                accessToken = token,
                userName = login
            });


            _logger.LogInformation(sendResponse.StatusCode.ToString());
        }
    }
}
