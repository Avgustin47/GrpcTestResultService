
using GrpcPdfService.Repository;
using GrpcPdfService.Services;
using MassTransit;
using UserMqDto;

namespace GrpcPdfService.RMQConsumer
{
    public class PdfConsumer(IPdfStorage storage, IPdfRepository repos, ILogger<PdfConsumer> logger) : IConsumer<TestDto>
    {
      
        public async Task Consume(ConsumeContext<TestDto> context)
        {
            var testdto = context.Message;
            try
            {
                byte[] pdfBytes = ExpoxtResultPdf.Expoxt(testdto);
                string fileName = $"Report_{testdto.Id}_{DateTime.UtcNow:yyyyMMdd}.pdf";
                string storageKey = $"reports/client_{testdto.IdClient}/{fileName}";

                await storage.SaveAsync(storageKey, pdfBytes, context.CancellationToken);
                var report = new Entities.Report {
                    ClientId = testdto.IdClient,
                    TestId = testdto.Id,
                    FileName = fileName,
                    StoragePath = storageKey,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Completed"
                };
                await repos.AddAsync(report, context.CancellationToken);
                logger.LogInformation("PDF сохранён для клиента {ClientId}, тест {TestId}", testdto.IdClient, testdto.Id);

                await context.Publish(new PdfReportReadyEvent
                {
                    ClientId = testdto.IdClient,
                    ReportId = report.Id,
                    ReportName = $"Отчет по услуге #{testdto.Id}",
                    StoragePath = storageKey,
                    GeneratedAt = DateTime.UtcNow,
                });
            }
            catch (Exception ex)
            {
                var report = new Entities.Report
                {
                    ClientId = testdto.IdClient,
                    TestId = testdto.Id,
                   
                    CreatedAt = DateTime.UtcNow,
                    Status = "Failed"
                };
                await repos.AddAsync(report, context.CancellationToken);
                logger.LogError(ex, "Ошибка генерации/сохранения PDF");
            }
        }
    }
}

