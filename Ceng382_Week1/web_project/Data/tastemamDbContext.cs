using Microsoft.EntityFrameworkCore;
using tastemam.Models;

namespace tastemam.Data
{
    public class TastemamDbContext : DbContext
    {
        public TastemamDbContext(DbContextOptions<TastemamDbContext> options)
            : base(options) { }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}