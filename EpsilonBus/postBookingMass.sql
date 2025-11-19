CREATE PROCEDURE [dbo].[postBookingMass] @EmployeeID int, @Option int
	, @StopID int
	, @Direction int
	, @StartDate date, @EndDate date
	, @ApplyMonday int, @ApplyTuesday int, @ApplyWednesday int, @ApplyThursday int, @ApplyFriday int, @ApplySaturday int, @ApplySunday int
	, @LanguageID int=1
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	-- Mock implementation: always succeed
	SET @IsSuccess=1;
	SET @ErrorCode=NULL;
	SET @ErrorMsg=NULL;	
END
