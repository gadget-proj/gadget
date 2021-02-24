using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Options;
using Gadget.Notifications.Services.Interfaces;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Notifications.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _client;
        private readonly SmtpEmailOptions _settings;

        public SmtpEmailService(SmtpClient client, SmtpEmailOptions settings)
        {
            _client = client;
            _settings = settings;
            // TO DO register smtp client ins startup like this:
            //client = new SmtpClient(settings.Host, settings.Port);
            //client.Credentials = new NetworkCredential(settings.Login, settings.Password);
        }

        public async Task SendEmailMessage(EmailMessage message, CancellationToken cancellationToken)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_settings.Sender);
            mail.To.Add(message.Receiver);
            mail.IsBodyHtml = false;
            mail.Body = message.Body;
            mail.Subject = "Monitor Usług powiadomieni";

            await _client.SendMailAsync(mail);
        }
    }
}
