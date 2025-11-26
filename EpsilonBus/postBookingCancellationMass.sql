CREATE PROCEDURE [dbo].[postBookingCancellationMass] @EmployeeID int, @StartDate date
	, @LanguageID int=1
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Delete all non-recurring bookings (IsDefault=0) for the employee after @StartDate
        DELETE FROM Bookings
        WHERE EmployeeID = @EmployeeID
          AND isnull(IsDefault, 0) = 0
          AND StartDate >= @StartDate;

        -- 2. Update any recurring bookings (IsDefault=1) where @StartDate is between StartDate and EndDate (or EndDate is null)
        UPDATE Bookings
        SET EndDate = DATEADD(DAY, -1, @StartDate)
        WHERE EmployeeID = @EmployeeID
          AND isnull(IsDefault, 0) = 1
          AND StartDate < @StartDate
          AND (EndDate IS NULL OR EndDate >= @StartDate);

        SET @IsSuccess = 1;
        SET @ErrorCode = NULL;
        SET @ErrorMsg = NULL;
    END TRY
    BEGIN CATCH
        SET @IsSuccess = 0;
        SET @ErrorCode = 'CANCEL-MASS-ERROR';
        SET @ErrorMsg = 'CANCEL-MASS-ERROR';
    END CATCH
END
