using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Project_Core.Data.Configurations
{
    public class OrderBookConfiguration : IEntityTypeConfiguration<OrderBook>
    {
        public void Configure(EntityTypeBuilder<OrderBook> builder)
        {
            builder.HasKey(ob => new { ob.MaHoaDon, ob.MaSach });

            builder.HasOne(ob => ob.HoaDon)
                   .WithMany(o => o.OrderBooks)
                   .HasForeignKey(ob => ob.MaHoaDon)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ob => ob.Sach)
                   .WithMany(b => b.OrderBooks)
                   .HasForeignKey(ob => ob.MaSach)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
