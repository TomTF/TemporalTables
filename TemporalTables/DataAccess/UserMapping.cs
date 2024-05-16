using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemporalTables.Domain;

namespace QueryDemo.DataAccess
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", b => b.IsTemporal());
            builder.Property(d => d.Name).HasMaxLength(64);
            builder.Property(d => d.Birthday);
        }
    }
}
