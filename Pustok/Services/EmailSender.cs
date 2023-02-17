

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Pustok.Services
{
    public class EmailSender
    {
        public class EmaiSender : IEmailSender
        {
            private readonly IConfiguration _configuration;

            public EmaiSender(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void Send(string to, string subject, string html)
            {
                Send(new[] { to }, subject, html);
            }

            public void Send(string[] allto, string subject, string html)
            {
                // create message

                var email = new MimeMessage();

                email.From.Add(MailboxAddress.Parse(_configuration["Mail:UserMail"]));
                foreach (var to in allto)

                    email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(_configuration.GetValue<string>("Mail:Host"), _configuration.GetValue<int>("Mail:Port"), SecureSocketOptions.StartTls);
                smtp.Authenticate(_configuration.GetValue<string>("Mail:UserMail"), _configuration.GetValue<string>("Mail:Password"));
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
