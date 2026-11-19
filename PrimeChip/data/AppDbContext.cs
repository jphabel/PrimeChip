using Microsoft.EntityFrameworkCore;
using PrimeChip.Models;

namespace PrimeChip.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockMovement>()
                .HasOne(movement => movement.Inventory)
                .WithMany()
                .HasForeignKey(movement => movement.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockMovement>()
                .HasOne(movement => movement.Vendor)
                .WithMany()
                .HasForeignKey(movement => movement.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}