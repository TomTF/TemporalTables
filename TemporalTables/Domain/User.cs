namespace TemporalTables.Domain
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime Birthday { get; set; }
    }
}
