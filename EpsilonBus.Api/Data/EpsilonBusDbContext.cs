using Microsoft.EntityFrameworkCore;
using EpsilonBus.Api.Models;

namespace EpsilonBus.Api.Data
{
    public class EpsilonBusDbContext : DbContext
    {
        public EpsilonBusDbContext(DbContextOptions<EpsilonBusDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<BusType> BusTypes { get; set; }
        public DbSet<BusAllocation> BusAllocations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<NonWorkingDay> NonWorkingDays { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<TranslatedText> TranslatedTexts { get; set; }
        public DbSet<BusinessLogicRule> BusinessLogicRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add any custom configuration here if needed
        }
    }
}