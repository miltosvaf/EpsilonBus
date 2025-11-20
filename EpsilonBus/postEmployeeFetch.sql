CREATE PROCEDURE [dbo].[postEmployeeFetch] @CompanyID int, @BranchID int, @LanguageID int=1
AS
BEGIN
	SET NOCOUNT ON;

	SELECT e.ID AS EmployeeID, e.EmployeeCode, e.EmployeeName, e.Email
		, e.CompanyID, c.CompanyName
		, e.BranchID, b.BranchName
		, e.IsActive
		, e.UserName
		, e.EntraIDCode
		, isnull(u.[Role],'Employee') AS UserRole
	FROM Employees e
	JOIN Companies c ON e.CompanyID=c.ID
	JOIN Branches b ON e.BranchID=b.ID
	LEFT JOIN [Users] u ON e.ID=u.EmployeeID
	WHERE (isnull(@CompanyID,0)=0 OR c.ID=@CompanyID)
	AND (isnull(@BranchID,0)=0 OR b.ID=@BranchID);

END;
