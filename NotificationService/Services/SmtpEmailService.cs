using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace NotificationService.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string smtpHost;
        private readonly int smtpPort;
        private readonly string smtpUsername;
        private readonly string smptPassword;
        private readonly bool useSsl;

        public SmtpEmailService(IConfiguration configuration)
        {
            var smtpSection = configuration.GetSection("Smtp");
            smtpHost = smtpSection["Host"] ?? "smtp.yourprovider.com";
            smtpPort = int.Parse(smtpSection["Port"] ?? "587");
            smtpUsername = smtpSection["Username"] ?? "";
            smptPassword = smtpSection["Password"] ?? "";
            useSsl = bool.Parse(smtpSection["UseSsl"] ?? "false");
        }

        public async Task SendEmailAsync(string to, string subject, string body,  string? attachmentName = null)
        {
            using var client = new SmtpClient();
            if (useSsl)
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
            else
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(smtpUsername))
                await client.AuthenticateAsync(smtpUsername, smptPassword);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("МедНорст", "noreply@mednorst.ru"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = builder.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
