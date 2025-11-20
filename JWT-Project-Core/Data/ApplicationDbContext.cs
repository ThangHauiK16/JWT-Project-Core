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
        }


    }
}
