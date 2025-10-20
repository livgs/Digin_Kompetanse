using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Digin_Kompetanse.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailOptions _opt;
        public MailKitEmailSender(IOptions<EmailOptions> opt) => _opt = opt.Value;

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_opt.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            // Velg sikkerhet basert på port
            SecureSocketOptions security =
                _opt.Port == 465 ? SecureSocketOptions.SslOnConnect :
                _opt.EnableStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

            await client.ConnectAsync(_opt.Host, _opt.Port, security);

            if (!string.IsNullOrWhiteSpace(_opt.User))
                await client.AuthenticateAsync(_opt.User, _opt.Pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}