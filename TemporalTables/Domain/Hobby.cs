namespace TemporalTables.Domain
{
    public class Hobby
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
