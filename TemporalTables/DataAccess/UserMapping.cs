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

            builder.HasMany(d => d.Hobbies)
                .WithMany(h => h.Users)
                .UsingEntity(e => e.ToTable("UserHobby", u => u.IsTemporal()));
        }
    }
}
