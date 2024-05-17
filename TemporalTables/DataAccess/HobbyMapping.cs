using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemporalTables.Domain;

namespace QueryDemo.DataAccess
{
    public class HobbyMapping : IEntityTypeConfiguration<Hobby>
    {
        public void Configure(EntityTypeBuilder<Hobby> builder)
        {
            builder.ToTable("Hobby", b => b.IsTemporal());
        }
    }
}
