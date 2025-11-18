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
        public DbSet<User> Users { get; set; } // Add Users DbSet
        public DbSet<ReportPerDateDirectionStopResult> ReportPerDateDirectionStopResults { get; set; }
        public DbSet<ReportSingleDatePerDirectionStopDetailedListResult> ReportSingleDatePerDirectionStopDetailedListResults { get; set; }
        public DbSet<BusinessDayResult> BusinessDayResults { get; set; }
        public DbSet<ReportEmployeeAvgBookingPerWeekResult> ReportEmployeeAvgBookingPerWeekResults { get; set; }
        public DbSet<StopDto> StopDtos { get; set; } // Keyless entity for getStops SP
        public DbSet<EmployeeDetailsDto> EmployeeDetailsDtos { get; set; } // Keyless entity for getEmployeeDetailsByEmployeeCode SP
        public DbSet<StopActiveDetailsDto> StopActiveDetailsDtos { get; set; } // Keyless entity for getStopsActiveDetails SP

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add any custom configuration here if needed
            modelBuilder.Entity<ReportPerDateDirectionStopResult>().HasNoKey();
            modelBuilder.Entity<ReportSingleDatePerDirectionStopDetailedListResult>().HasNoKey();
            modelBuilder.Entity<BusinessDayResult>().HasNoKey();
            modelBuilder.Entity<ReportEmployeeAvgBookingPerWeekResult>().HasNoKey();
            modelBuilder.Entity<StopDto>().HasNoKey();
            modelBuilder.Entity<EmployeeDetailsDto>().HasNoKey();
            modelBuilder.Entity<StopActiveDetailsDto>().HasNoKey();
        }

        public async Task<List<StopDto>> GetStopsSPAsync(int branchId, int showOnlyActive, int languageId = 1)
        {
            return await this.Set<StopDto>().FromSqlRaw(
                "EXEC [dbo].[getStops] @BranchID = {0}, @ShowOnlyActive = {1}, @LanguageID = {2}",
                branchId, showOnlyActive, languageId).ToListAsync();
        }

        public async Task<EmployeeDetailsDto?> GetEmployeeDetailsByCodeAsync(string employeeCode, int languageId = 1)
        {
            var result = await this.Set<EmployeeDetailsDto>()
                .FromSqlRaw(
                    "EXEC [dbo].[getEmployeeDetailsByEmployeeCode] @EmployeeCode = {0}, @LanguageID = {1}",
                    employeeCode, languageId)
                .ToListAsync(); // Materialize here

            return result.FirstOrDefault();
        }

        public async Task<List<StopActiveDetailsDto>> GetStopsActiveDetailsSPAsync(int branchId, int languageId = 1)
        {
            return await this.Set<StopActiveDetailsDto>().FromSqlRaw(
                "EXEC [dbo].[getStopsActiveDetails] @BranchID = {0}, @LanguageID = {1}",
                branchId, languageId).ToListAsync();
        }
    }
}