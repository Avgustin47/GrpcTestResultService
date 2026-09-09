using MassTransit;
using NotificationService.Services;
using UserMqDto;

namespace NotificationService.MqConsumer
{
    public class ReportReadyConsumer(
        IEmailService emailService, 
        IUserProfileClient userClient, 
        ILogger<ReportReadyConsumer> logger,
        ITokenStorage storage) : IConsumer<PdfReportReadyEvent>
    {

        
        public async Task Consume(ConsumeContext<PdfReportReadyEvent> context)
            {
                var evt = context.Message;
                try
                {
                    var email = await userClient.GetEmailByClientId(evt.ClientId, context.CancellationToken);

                    if (string.IsNullOrEmpty(email))
                        throw new Exception($"Email для клиента {evt.ClientId} не найден");
                var token = Guid.NewGuid().ToString("N");
                await storage.SaveTokenAsync(token, evt.ReportId, TimeSpan.FromHours(24));
                var baseUrl = "https://mednorst.ru"; // лучше вынести в appsettings
                var downloadLink = $"{baseUrl}/api/reports/download?token={token}";

                var subject = $"Ваш отчёт #{evt.ReportId} готов";
                var body = $@"
                <h1>Здравствуйте!</h1>
                <p>Ваш отчёт <strong>{evt.ReportName}</strong> сгенерирован.</p>
                <p>Скачайте его по ссылке (действительна 24 часа):</p>
                <a href='{downloadLink}'>Скачать PDF</a>
                <p>С уважением, команда МедНорст</p>
            ";
                await emailService.SendEmailAsync(email, subject, body,  $"{evt.ReportName}.pdf");

                    logger.LogInformation("Email отправлен клиенту {ClientId}", evt.ClientId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка отправки email для отчёта {ReportId}", evt.ReportId);
                    throw; 
                }
            }
        }
    }

