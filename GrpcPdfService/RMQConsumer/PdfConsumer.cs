
using GrpcPdfService.Services;
using MassTransit;
using UserMqDto;

namespace GrpcPdfService.RMQConsumer
{
    public class PdfConsumer() : IConsumer<TestDto>
    {
      

        public async Task Consume(ConsumeContext<TestDto> context)
        {
            var test = new TestDto
            {
                Id = context.Message.Id,
                Description = context.Message.Description,
                IdClient = context.Message.IdClient,
                Name = context.Message.Name,
                TestDate = context.Message.TestDate,
                PatientName = context.Message.PatientName,
                Price = context.Message.Price
            };

           ExpoxtResultPdf.Expoxt(test);
            
          
        }
    }
}   
