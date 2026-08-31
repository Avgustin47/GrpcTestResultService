using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTestResultService.Entities;
using GrpcTestResultService.Model;



namespace GrpcTestResultService.Services
{
    public class TestTypeService(TestResultDbContext context) : TetstTypeProtoService.TetstTypeProtoServiceBase
    {

        readonly TestResultDbContext db = context;

        private class PubUser
        {
            public Guid Id { get; set; }
            public required string Name { get; set; }

        }
        public override Task<testtype> GetTypeTest(GetTestTypeRequest request, ServerCallContext context)
        {
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id);
            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Не найдено"));
            else
            {
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
            }).ToList();
            responce.Testtype.AddRange(typelist);


            return await Task.FromResult(responce);
        }
        public override async Task<testtype> CreateTestType(CreateTestTypeRequest request, ServerCallContext context)
        {

            var result = new TestType
            {
                Id = db.TestTypes.Any() ? db.TestTypes.Max(x => x.Id) + 1 : 1,
                Name = request.Name,
                Description = request.Descriprion,
                Price = request.Price
            };

            await db.TestTypes.AddAsync(result);

            await db.SaveChangesAsync();


            var reply = new testtype()
            {
                Id = result.Id,
                Descriprion = result.Description,
                Price = result.Price,
                Name = result.Name
            };
            return await Task.FromResult(reply)!;
        }
        public override Task<testtype> UpdateTestType(UpdateTestTypeRequest request, ServerCallContext context)
        {
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id);

            result!.Name = request.Name;
            result.Description = request.Descriprion;
            result.Price = request.Price;
            db.SaveChanges();
            var reply = new testtype()
            {
                Id = request.Id,
                Name = result.Name,
                Descriprion = result.Description,
                Price = result.Price
            };
            return Task.FromResult(reply)!;
        }
        public override Task<Empty> DeleteTesttype(DeleteTestTypeRequest request, ServerCallContext context)
        {
            var result = db.TestTypes.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Ошибка удаления данных"));
            db.TestTypes.Remove(result);
            db.SaveChanges();
            return Task.FromResult(new Empty())!;
        }
    }
}
