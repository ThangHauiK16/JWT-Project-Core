using JWT_Project_Core.Model;
using JWT_Project_Core.Model.Human;
using Microsoft.EntityFrameworkCore;

namespace JWT_Project_Core.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Sach> Saches { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDon_Sach> HoaDon_Saches { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HoaDon_Sach>()
                .HasKey(hs => new { hs.MaHoaDon, hs.MaSach });

            modelBuilder.Entity<HoaDon_Sach>()
                .HasOne(hs => hs.HoaDon)
                .WithMany(h => h.HoaDon_Saches)
                .HasForeignKey(hs => hs.MaHoaDon);

            modelBuilder.Entity<HoaDon_Sach>()
                .HasOne(hs => hs.Sach)
                .WithMany(s => s.HoaDon_Saches)
                .HasForeignKey(hs => hs.MaSach);
                        
            modelBuilder.Entity<HoaDon>()
                  .HasOne(h => h.User)           
                  .WithMany(u => u.HoaDons)       
                  .HasForeignKey(h => h.Username) 
                  .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Cart>()
                  .HasOne(c => c.User)
                  .WithMany() 
                  .HasForeignKey(c => c.Username)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                   .HasIndex(c => c.Username)
                   .IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Sach)
                .WithMany()
                .HasForeignKey(ci => ci.MaSach)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
