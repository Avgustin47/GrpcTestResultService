namespace UserMqDto
{
    public record UserDto
    {
      public Guid Id { get; set; }
      public required string Name { get; set; } = "имя не указано";
    }
    public record TestDto
    {
        
        public int Id { get; set; }
        public int IdClient { get; set; }
        public required string PatientName { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TestDate {  get; set; }
        public required string Name { get; set; }
        public double Price { get; set; }
    }

}
