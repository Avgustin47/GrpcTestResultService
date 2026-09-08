using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcTestResultService.Entities
{
    public class Patient
    {
        public int Id {  get; set; }
       public Guid IdClient { get; set; }
       public required string  Name { get; set; } 

       public DateTime RegisterDate { get; set; }
       public  List<TestResult> TestResults { get; set; } = [];
    }
}
