using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;

namespace Gadget.Notifications.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailMessage(EmailMessage message, CancellationToken cancellationToken);
    }
}