namespace GrpcPdfService.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int TestId { get; set; }
        public  string? FileName { get; set; }
        public string?  StoragePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
}
