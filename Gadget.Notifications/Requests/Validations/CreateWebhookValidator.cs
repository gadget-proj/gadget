using FluentValidation;
using FluentValidation.Results;
using Gadget.Notifications.Domain.Enums;
using System.Net.Mail;

namespace Gadget.Notifications.Requests.Validations
{
    public class CreateWebhookValidator : AbstractValidator<CreateWebhook>
    {
        public override ValidationResult Validate(ValidationContext<CreateWebhook> context)
        {
            RuleFor(x => x.Receiver).NotEmpty().NotNull().WithMessage("Receiver is mandatory");
            RuleFor(x => x.NotifierType).Must(n => n != NotifierType.None).WithMessage("Chose correct webhook type.");
            RuleFor(x => x).Must(r => ValidateReceiver(r)).WithMessage("Receiver is not in correct format");
            return base.Validate(context);
        }

        private bool ValidateReceiver(CreateWebhook createWebhook)
        {
            switch (createWebhook.NotifierType)
            {
                case NotifierType.Email: return ValidateEmail(createWebhook.Receiver);
                case NotifierType.Discord: return ValidateDiscord(createWebhook.Receiver);
                default:return false;
            }
        }

        private bool ValidateEmail(string webhook)
        {
            try
            {
                var email = new MailAddress(webhook);
                return true;
            }
            catch (System.Exception)
            {
                return false;
                throw;
            }
        }

        private bool ValidateDiscord(string webhook)
        {
            return false;
        }
    }
}
