using Microsoft.EntityFrameworkCore;
using EpsilonBus.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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
        public DbSet<EmployeeCalendarDto> EmployeeCalendarDtos { get; set; } // Keyless entity for getEmployeeCalendar SP
        public DbSet<ReportPerDateDirectionDto> ReportPerDateDirectionDtos { get; set; } // Keyless entity for getReportPerDateDirection SP
        public DbSet<PostEmployeeFetchResult> PostEmployeeFetchResults { get; set; } // Keyless entity for postEmployeeFetch SP

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
            modelBuilder.Entity<EmployeeCalendarDto>().HasNoKey();
            modelBuilder.Entity<ReportPerDateDirectionDto>().HasNoKey();
            modelBuilder.Entity<PostEmployeeFetchResult>().HasNoKey();
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

        public async Task<List<EmployeeCalendarDto>> GetEmployeeCalendarSPAsync(DateTime startDate, DateTime endDate, int employeeId, int languageId = 1)
        {
            return await this.Set<EmployeeCalendarDto>().FromSqlRaw(
                "EXEC [dbo].[getEmployeeCalendar] @StartDate = {0}, @EndDate = {1}, @EmployeeID = {2}, @LanguageID = {3}",
                startDate, endDate, employeeId, languageId).ToListAsync();
        }

        public async Task<PostBookingSingleResponse> PostBookingSingleAsync(PostBookingSingleRequest request)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errorCodeParam = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            var errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[postBookingSingle] @EmployeeID = {0}, @SpecificDate = {1}, @StopID = {2}, @Direction = {3}, @LanguageID = {4}, @IsSuccess = @IsSuccess OUTPUT, @ErrorCode = @ErrorCode OUTPUT, @ErrorMsg = @ErrorMsg OUTPUT",
                request.EmployeeID,
                request.SpecificDate,
                request.StopID,
                request.Direction,
                request.LanguageID,
                isSuccessParam,
                errorCodeParam,
                errorMsgParam
            );

            return new PostBookingSingleResponse
            {
                IsSuccess = (int)(isSuccessParam.Value ?? 0),
                ErrorCode = errorCodeParam.Value as string,
                ErrorMsg = errorMsgParam.Value as string
            };
        }

        public async Task<List<ReportPerDateDirectionDto>> GetReportPerDateDirectionSPAsync(DateTime startDate, DateTime endDate, int branchId, int direction, int languageId = 1)
        {
            return await this.Set<ReportPerDateDirectionDto>().FromSqlRaw(
                "EXEC [dbo].[getReportPerDateDirection] @StartDate = {0}, @EndDate = {1}, @BranchID = {2}, @Direction = {3}, @LanguageID = {4}",
                startDate, endDate, branchId, direction, languageId).ToListAsync();
        }

        public async Task<PostSingleCancelationResponse> PostSingleCancelationAsync(PostSingleCancelationRequest request)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errorCodeParam = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            var errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[postSingleCancelation] @EmployeeID = {0}, @SpecificDate = {1}, @LanguageID = {2}, @IsSuccess = @IsSuccess OUTPUT, @ErrorCode = @ErrorCode OUTPUT, @ErrorMsg = @ErrorMsg OUTPUT",
                request.EmployeeID,
                request.SpecificDate,
                request.LanguageID,
                isSuccessParam,
                errorCodeParam,
                errorMsgParam
            );

            return new PostSingleCancelationResponse
            {
                IsSuccess = (int)(isSuccessParam.Value ?? 0),
                ErrorCode = errorCodeParam.Value as string,
                ErrorMsg = errorMsgParam.Value as string
            };
        }

        public async Task<PostBookingCancellationMassResponse> PostBookingCancellationMassAsync(PostBookingCancellationMassRequest request)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errorCodeParam = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            var errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[postBookingCancellationMass] @EmployeeID = {0}, @StartDate = {1}, @LanguageID = {2}, @IsSuccess = @IsSuccess OUTPUT, @ErrorCode = @ErrorCode OUTPUT, @ErrorMsg = @ErrorMsg OUTPUT",
                request.EmployeeID,
                request.StartDate,
                request.LanguageID,
                isSuccessParam,
                errorCodeParam,
                errorMsgParam
            );

            return new PostBookingCancellationMassResponse
            {
                IsSuccess = (int)(isSuccessParam.Value ?? 0),
                ErrorCode = errorCodeParam.Value as string,
                ErrorMsg = errorMsgParam.Value as string
            };
        }

        public async Task<PostBookingMassResponse> PostBookingMassAsync(PostBookingMassRequest request)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errorCodeParam = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            var errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[postBookingMass] @EmployeeID = {0}, @Option = {1}, @StopID = {2}, @Direction = {3}, @StartDate = {4}, @EndDate = {5}, @ApplyMonday = {6}, @ApplyTuesday = {7}, @ApplyWednesday = {8}, @ApplyThursday = {9}, @ApplyFriday = {10}, @ApplySaturday = {11}, @ApplySunday = {12}, @LanguageID = {13}, @IsSuccess = @IsSuccess OUTPUT, @ErrorCode = @ErrorCode OUTPUT, @ErrorMsg = @ErrorMsg OUTPUT",
                request.EmployeeID,
                request.Option,
                request.StopID,
                request.Direction,
                request.StartDate,
                request.EndDate,
                request.ApplyMonday,
                request.ApplyTuesday,
                request.ApplyWednesday,
                request.ApplyThursday,
                request.ApplyFriday,
                request.ApplySaturday,
                request.ApplySunday,
                request.LanguageID,
                isSuccessParam,
                errorCodeParam,
                errorMsgParam
            );

            return new PostBookingMassResponse
            {
                IsSuccess = (int)(isSuccessParam.Value ?? 0),
                ErrorCode = errorCodeParam.Value as string,
                ErrorMsg = errorMsgParam.Value as string
            };
        }

        public async Task<List<PostEmployeeFetchResult>> PostEmployeeFetchSPAsync(int companyId, int branchId, int languageId = 1)
        {
            return await this.Set<PostEmployeeFetchResult>().FromSqlRaw(
                "EXEC [dbo].[postEmployeeFetch] @CompanyID = {0}, @BranchID = {1}, @LanguageID = {2}",
                companyId, branchId, languageId).ToListAsync();
        }

        public async Task<PostEmployeeCreateResponse> PostEmployeeCreateAsync(PostEmployeeCreateRequest request)
        {
            var isSuccessParam = new Microsoft.Data.SqlClient.SqlParameter("@IsSuccess", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
            var errorCodeParam = new Microsoft.Data.SqlClient.SqlParameter("@ErrorCode", System.Data.SqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Output };
            var errorMsgParam = new Microsoft.Data.SqlClient.SqlParameter("@ErrorMsg", System.Data.SqlDbType.NVarChar, -1) { Direction = System.Data.ParameterDirection.Output };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[postEmployeeCreate] @EmployeeID = {0}, @EmployeeCode = {1}, @EmployeeName = {2}, @Email = {3}, @CompanyID = {4}, @BranchID = {5}, @IsActive = {6}, @UserName = {7}, @EntraIDCode = {8}, @UserRole = {9}, @IsSuccess = @IsSuccess OUTPUT, @ErrorCode = @ErrorCode OUTPUT, @ErrorMsg = @ErrorMsg OUTPUT",
                request.EmployeeID,
                request.EmployeeCode,
                request.EmployeeName,
                request.Email,
                request.CompanyID,
                request.BranchID,
                request.IsActive,
                request.UserName,
                request.EntraIDCode,
                request.UserRole,
                isSuccessParam,
                errorCodeParam,
                errorMsgParam
            );

            return new PostEmployeeCreateResponse
            {
                IsSuccess = (int)(isSuccessParam.Value ?? 0),
                ErrorCode = errorCodeParam.Value as string,
                ErrorMsg = errorMsgParam.Value as string
            };
        }
    }
}