CREATE PROCEDURE [dbo].[getEmployeeDetailsByEmployeeCode] @EmployeeCode varchar(255), @LanguageID int=1
AS
BEGIN
	SET NOCOUNT ON;

	SELECT e.ID AS EmployeeID, e.EmployeeName
		, c.ID AS CompanyID, c.CompanyCode, c.CompanyName
		, b.ID AS BranchID, b.BranchCode, b.BranchName
		, b.Latitude, b.Longitude
		, b.OperatesMonday
		, b.OperatesTuesday
		, b.OperatesWednesday
		, B.OperatesThursday
		, b.OperatesFriday
		, b.OperatesSaturday
		, b.OperatesSunday
	FROM Employees e
	JOIN Companies c ON e.CompanyID=c.ID
	JOIN Branches b ON e.BranchID=b.ID
	WHERE e.EmployeeCode=@EmployeeCode;
END
