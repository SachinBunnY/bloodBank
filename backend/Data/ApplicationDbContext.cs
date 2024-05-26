// File: Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using BloodBank.Backend.Models;

namespace BloodBank.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryRecord> InventoryRecords { get; set; }

        // DbSet for Payment entity  RAZOR PAYMENT INTEGRATION
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }


    }
}
