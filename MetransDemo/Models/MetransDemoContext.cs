using Microsoft.EntityFrameworkCore;

namespace MetransDemo.Models
{
    public class MetransDemoContext : DbContext
    {

        public MetransDemoContext(DbContextOptions<MetransDemoContext> options)
            : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Status> Statuses { get; set; }
    }
}
