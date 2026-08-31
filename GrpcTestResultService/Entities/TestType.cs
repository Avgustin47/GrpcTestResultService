namespace GrpcTestResultService.Entities
{
    public class TestType
    {
       public int Id { get; set; }
       public required string Name { get; set; }
       public required string Description { get; set; }
       public double Price { get; set; }
        public List<TestResult> TestResults { get; set; } = []; 
    }
}
