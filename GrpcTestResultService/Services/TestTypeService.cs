using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTestResultService.Entities;
using GrpcTestResultService.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;



namespace GrpcTestResultService.Services
{
    public class TestTypeService(TestResultDbContext context, ILogger<TestTypeService> logger) : TetstTypeProtoService.TetstTypeProtoServiceBase
    {

        private readonly TestResultDbContext db = context;
        private readonly ILogger<TestTypeService> logger = logger;

        public override Task<testtype> GetTypeTest(GetTestTypeRequest request, ServerCallContext context)
        {
            logger.LogInformation("gRPC GetTypeTest вызван с id : {Id}", request.Id);
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id);
            if (result == null)
            {
                logger.LogWarning("Невалидные данные для ID :{Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, "Не найдено"));
            }
            else
            {
                logger.LogInformation("Данные успешно дайнены");
                var reply = new testtype()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Descriprion = result.Description,
                    Price = result.Price
                };
                return Task.FromResult(reply);
            }

        }
        public async override Task<TestTypeResponce> ListTypeTest(Empty request, ServerCallContext context)
        {
            var responce = new TestTypeResponce();
            var typelist = db.TestTypes.Select(x => new testtype
            {
                Id = x.Id,
                Descriprion = x.Description,
                Name = x.Name,
                Price = x.Price
            }).Take(1).Skip(1).ToList();
            responce.Testtype.AddRange(typelist);


            return responce;
        }
        public override async Task<testtype> CreateTestType(CreateTestTypeRequest request, ServerCallContext context)
        {

            var result = new TestType
            {

                Name = request.Name,
                Description = request.Descriprion,
                Price = request.Price
            };

            await db.TestTypes.AddAsync(result);

            await db.SaveChangesAsync(context.CancellationToken);


            var reply = new testtype()
            {
                Id = result.Id,
                Descriprion = result.Description,
                Price = result.Price,
                Name = result.Name
            };
            return reply;
        }
        public async override Task<testtype> UpdateTestType(UpdateTestTypeRequest request, ServerCallContext context)
        {
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id) 
                ?? throw new RpcException(new Status(StatusCode.NotFound, "запись не найдена"));
            result.Name = request.Name;
            result.Description = request.Descriprion;
            result.Price = request.Price;
            await db.SaveChangesAsync();
            var reply = new testtype()
            {
                Id = request.Id,
                Name = result.Name,
                Descriprion = result.Description,
                Price = result.Price
            };
            return reply;
        }
        public override async Task<Empty> DeleteTesttype(DeleteTestTypeRequest request, ServerCallContext context)
        {
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Ошибка удаления данных"));
            db.TestTypes.Remove(result);
            await db.SaveChangesAsync();
            return new Empty();
        }
    }
}
