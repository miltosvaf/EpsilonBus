CREATE PROCEDURE [dbo].[postSingleCancelation] @EmployeeID int, @SpecificDate date
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