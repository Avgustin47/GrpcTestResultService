using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTestResultService.Entities;
using GrpcTestResultService.Model;
using GrpcTestResultService.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserMqDto;


namespace GrpcTestResultService.Services
{
    public class TestResultService(TestResultDbContext context, IPublishEndpoint publish) : TestResultProtoService.TestResultProtoServiceBase
    {

        readonly TestResultDbContext db = context;
        readonly IPublishEndpoint _publisher = publish;
        public override Task<testresult> GetTestResult(GetTestResultRequest request, ServerCallContext context)
        {
            var result = db.TestResults.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "not found"));
            var reply = new testresult()
            {
                Id = result.Id,
                Date = Timestamp.FromDateTime(result.Date),
                Description = result.Description,
                TestType = result.TestTypeId
            };
            return Task.FromResult(reply);
        }
        [Authorize]
        public async override Task<TestResultResponce> ListTestResult(Empty request, ServerCallContext context)
        {
            var responce = new TestResultResponce();
            var testresult = await db.TestResults.AsNoTracking().Select(x => new testresult
            {
                Id = x.Id,
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(x.Date,DateTimeKind.Utc)),
                Description = x.Description,
                TestType = x.TestTypeId

            }).ToListAsync(context.CancellationToken);
                
            responce.Testresult.AddRange(testresult);
            if (responce.Testresult.Count == 0) throw new RpcException(new Status(StatusCode.NotFound, "no data"));
            return responce;
        }
        [Authorize (Roles ="admin")]
        public async override Task<testresult> CreateTestResult(CreateTestResultRequest request, ServerCallContext context)
        {
            var result = new TestResult
            {
                
                Date = DateTime.UtcNow,
                Description = request.Description,
                TestTypeId = request.TestType,
                PatientId = request.PatientId

            };
            await db.TestResults.AddAsync(result,context.CancellationToken);
           await db.SaveChangesAsync();

           await _publisher.Publish(new TestDto
            {
                Id = result.Id,
                IdClient = result.PatientId,
                Description = result.Description,
                Name = db.TestTypes.FirstOrDefault(x => x.Id == result.TestTypeId)!.Name,
                PatientName = db.Patients.FirstOrDefault(x => x.Id == result.PatientId)!.Name,
                TestDate = result.Date,
                Price = db.TestTypes.FirstOrDefault(x => x.Id == result.TestTypeId)!.Price

            });

            var responce = new testresult()
            {
                Id = result.Id,
                Date = Timestamp.FromDateTime(result.Date),
                Description = result.Description,
                TestType = result.TestTypeId,
                IdPatient = result.PatientId

            };

            return await Task.FromResult(responce);
        }

        public override Task<testresult> UpdateTestResult(UpdateTestResultRequest request, ServerCallContext context)
        {
            var result = db.TestResults.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Ошибка изменения данных"));
            result.Date = request.Date.ToDateTime();
            result.TestTypeId = request.TestType;
            result.Description = request.Description;
            db.SaveChanges();
            var reply = new testresult
            {
                Id = request.Id,
                Date = Timestamp.FromDateTime(result.Date),
                Description = result.Description,
                TestType = result.TestTypeId
            };
            return Task.FromResult(reply)!;

        }

        public override Task<Empty> DeleteTestResult(DeleteTestResultRequest request, ServerCallContext context)
        {
            var result = db.TestResults.FirstOrDefault(x => x.Id == request.Id)
             ?? throw new RpcException(new Status(StatusCode.NotFound, "не найдено"));

            db.TestResults.Remove(result);
            db.SaveChanges();
            return Task.FromResult(new Empty());
        }
    }
}
