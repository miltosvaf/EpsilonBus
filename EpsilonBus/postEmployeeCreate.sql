CREATE PROCEDURE [dbo].[postEmployeeCreate] @EmployeeID int
	, @EmployeeCode varchar(50)
	, @EmployeeName nvarchar(255)
	, @Email nvarchar(255)
	, @CompanyID int
	, @BranchID int
	, @IsActive int
	, @UserName varchar(255)
	, @EntraIDCode varchar(255)
	, @UserRole varchar(50)
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Mock implementation: always succeed
	SET @IsSuccess=1;
	SET @ErrorCode=NULL;
	SET @ErrorMsg=NULL;	
END;
