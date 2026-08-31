using GrpcTestResultService.Entities;
using GrpcTestResultService.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata;
using UserMqDto;

namespace GrpcTestResultService.RmqServices
{
 
    public sealed class MqBackServ(TestResultDbContext con) : IConsumer<UserDto>
    {
        private readonly TestResultDbContext db = con;

        public async Task Consume(ConsumeContext<UserDto> contex)
        {
           
            var newpat = new Patient
            {
                Id = db.Patients.Any() ? db.Patients.Max(x=>x.Id) + 1 : 1 ,
                IdClient = contex.Message.Id,
                Name = contex.Message.Name,
                RegisterDate = DateTime.UtcNow
            };
            db.Patients.Add(newpat);
            await db.SaveChangesAsync();
        }
    }
}
