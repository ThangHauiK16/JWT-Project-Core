using JWT_Project_Core.Interface;
using JWT_Project_Core.Model;
using JWT_Project_Core.Model.Base;
using JWT_Project_Core.Model.Human;
using JWT_Project_Core.Service;
using Microsoft.EntityFrameworkCore;

namespace JWT_Project_Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Book> Order_Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order_Book>()
                .HasKey(hs => new { hs.MaHoaDon, hs.MaSach });

            modelBuilder.Entity<Order_Book>()
                .HasOne(hs => hs.HoaDon)
                .WithMany(h => h.Order_Books)
                .HasForeignKey(hs => hs.MaHoaDon);

            modelBuilder.Entity<Order_Book>()
                .HasOne(hs => hs.Sach)
                .WithMany(s => s.Order_Books)
                .HasForeignKey(hs => hs.MaSach);

            modelBuilder.Entity<Order>()
                  .HasOne(h => h.User)
                  .WithMany(u => u.Orders)
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

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    UpdateAuditFields();
        //    return base.SaveChangesAsync(cancellationToken);
        //}
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity);

           
            var currentUsername = _currentUserService.GetUsername();

           
            var actor = string.IsNullOrEmpty(currentUsername) ? "System" : currentUsername;

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
               
                    entity.CreatedAt = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entity.CreatedBy))
                    {
                        entity.CreatedBy = actor;
                    }
                }

                if (entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Modified)
                {
                   
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.UpdatedBy = actor;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        private void UpdateAuditFields()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                var currentUser = "System"; // TODO: lấy user hiện tại nếu có

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.UpdatedBy = currentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUser;
                    entry.Property(p => p.CreatedAt).IsModified = false;
                    entry.Property(p => p.CreatedBy).IsModified = false;
                }
            }
        }
    }
}
