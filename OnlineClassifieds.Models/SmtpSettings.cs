namespace OnlineClassifieds.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public bool Ssl { get; set; }
    }
}
