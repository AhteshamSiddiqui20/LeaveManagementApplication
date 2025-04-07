using Microsoft.EntityFrameworkCore;
using LeaveService.Entities;

namespace LeaveService
{
    public class LeaveApplicationDbContext : DbContext
    {
        public LeaveApplicationDbContext(DbContextOptions<LeaveApplicationDbContext> options)
            : base(options)
        {
        }
        public LeaveApplicationDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Read configuration from appsettings.json
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<LeaveBalance> LeaveBalance { get; set; }
    }
}
