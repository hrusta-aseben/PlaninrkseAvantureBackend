using MimeKit;
using MailKit.Net.Smtp;
namespace PlaninarskeAvantureBackend.Helper.Servisi
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("planinarske.avanture@gmail.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                client.Send(message);
                client.Disconnect(true);
            }

            return Task.CompletedTask;

        }
    }
}
