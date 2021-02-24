namespace Gadget.Notifications.Options
{
    public class SmtpEmailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
