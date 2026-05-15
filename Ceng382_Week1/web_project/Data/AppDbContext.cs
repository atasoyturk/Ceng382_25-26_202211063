using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tastemam.Models;

namespace tastemam.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Menu> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemCustomization> OrderItemCustomizations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CustomizationGroup> CustomizationGroups { get; set; }
        public DbSet<CustomizationOption> CustomizationOptions { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<CaretakerAgreement> CaretakerAgreements { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItemCustomization>()
                .HasOne(o => o.OrderItem)
                .WithMany(i => i.SelectedCustomizations)
                .HasForeignKey(o => o.OrderItemID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItemCustomization>()
                .HasOne(o => o.CustomizationOption)
                .WithMany()
                .HasForeignKey(o => o.CustomizationOptionID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}