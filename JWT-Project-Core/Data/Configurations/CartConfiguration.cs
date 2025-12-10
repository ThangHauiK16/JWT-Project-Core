using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Project_Core.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.CartId);

            builder.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.Username)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.Username)
                   .IsUnique();
        }
    }
}
