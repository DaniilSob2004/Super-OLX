using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

using OnlineClassifieds.Models;

namespace OnlineClassifieds.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSenderService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                client.EnableSsl = _smtpSettings.Ssl;
                client.Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                using (mailMessage)
                {
                    await client.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
