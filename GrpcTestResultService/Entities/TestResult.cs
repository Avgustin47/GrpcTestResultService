namespace GrpcTestResultService.Entities
{
    public class TestResult
    {
        public int Id { get; set; }
        public DateTime Date {  get; set; }
        public string Description { get; set; } = string.Empty;
        public int TestTypeId { get; set; }
        public TestType? TestType { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
