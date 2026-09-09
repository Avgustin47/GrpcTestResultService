namespace UserMqDto
{
    public record UserDto
    {
      public Guid Id { get; set; }
      public required string Name { get; set; } = "имя не указано";
    }   
    public record PdfReportReadyEvent
    {
        public Guid ClientId { get; set; }
        public int ReportId {  get; set; }
        public required string ReportName { get; set;  }
        public required string StoragePath { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
    public record TestDto
    {
        
        public int Id { get; set; }
        public Guid IdClient { get; set; }
        public required string PatientName { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TestDate {  get; set; }
        public required string Name { get; set; }
        public double Price { get; set; }
    }

}
