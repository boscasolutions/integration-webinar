using AlpineWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlpineWebApi.Data
{
    public class OrderShippingDatabaseContext : DbContext
    {
        public OrderShippingDatabaseContext(
            DbContextOptions<OrderShippingDatabaseContext> dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<OrderShipping> OrderShippings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderShipping>()
                .HasKey(x => x.OrderId);
        }
    }
}