namespace Kromi.Infrastructure.SettingsModels
{
    public class MailSmtpSettings
    {
        public string Server { get; set; } = null!;
        public int Port { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public bool IsFake { get; set; }
        public string Password { get; set; } = null!;
    }
}
