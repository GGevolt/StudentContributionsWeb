using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace StudentContributions.Services
{
    public class EmailSenderService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient smtpClient = new SmtpClient("127.0.0.1", 25);
            MailMessage mail = new MailMessage("system@bulul.cunny", email, subject, htmlMessage);
            mail.IsBodyHtml = true;

            return smtpClient.SendMailAsync(mail);
        }
    }
}
