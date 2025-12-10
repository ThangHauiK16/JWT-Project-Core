using JWT_Project_Core.Model.Human;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Project_Core.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Username);

            builder.Property(u => u.Username)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
