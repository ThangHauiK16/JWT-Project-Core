using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Project_Core.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.MaSach);

            builder.Property(b => b.MaSach)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
