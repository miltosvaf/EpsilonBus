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
	
	DECLARE @IsNew int;
	IF EXISTS (SELECT 1 FROM Employees WHERE ID=@EmployeeID) SET @IsNew=0 ELSE SET @IsNew=1;
	IF isnull(@IsActive,0)=0 SET @IsActive=0 ELSE SET @IsActive=1;
	--
	-- Valid User Roles are:  Admin, HR, Employee
	--
	IF @UserRole is null OR LOWER(ltrim(rtrim(@UserRole))) not in ('admin','hr','employee')
		SET @UserRole='Employee'
	ELSE
		SET @UserRole=case LOWER(ltrim(rtrim(@UserRole))) when 'admin' then 'Admin' when 'hr' then 'HR' when 'employee' then 'Employee' end;

	--
	-- Validations
	--
	IF ltrim(rtrim(isnull(@EmployeeCode,'')))='' BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-EMPTYCODE';
		SET @ErrorMsg='EMPLOYEE-CREATE-EMPTYCODE';
		RETURN;
	END;
	IF ltrim(rtrim(isnull(@EmployeeName,'')))='' BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-EMPTYNAME';
		SET @ErrorMsg='EMPLOYEE-CREATE-EMPTYNAME';
		RETURN;
	END;
	IF ltrim(rtrim(isnull(@Email,'')))='' BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-EMPTYEMAIL';
		SET @ErrorMsg='EMPLOYEE-CREATE-EMPTYEMAIL';
		RETURN;
	END;
	IF @CompanyID is null OR NOT EXISTS (SELECT 1 FROM Companies WHERE ID=@CompanyID) BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-INVALIDCOMPANY';
		SET @ErrorMsg='EMPLOYEE-CREATE-INVALIDCOMPANY';
		RETURN;
	END;
	IF @BranchID is null OR NOT EXISTS (SELECT 1 FROM Branches WHERE ID=@BranchID) BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-INVALIDBRANCH';
		SET @ErrorMsg='EMPLOYEE-CREATE-INVALIDBRANCH';
		RETURN;
	END;

	IF @IsNew=1 BEGIN
		BEGIN TRY
			-- New employee
			INSERT INTO Employees (ID, EmployeeName, EmployeeCode, UserName, Email, CompanyID, BranchID, IsActive, EntraIDCode)
			VALUES (@EmployeeID, @EmployeeName, @EmployeeCode, @UserName, @Email, @CompanyID, @BranchID, @IsActive, @EntraIDCode);
		END TRY
		BEGIN CATCH
			SET @IsSuccess=0;
			SET @ErrorCode='EMPLOYEE-CREATE-INSERTERROR';
			SET @ErrorMsg='EMPLOYEE-CREATE-INSERTERROR';
			RETURN;
		END CATCH;
	END
	ELSE BEGIN
		BEGIN TRY
			-- Existing employee
			UPDATE Employees
			SET EmployeeName=@EmployeeName, EmployeeCode=@EmployeeCode
				, UserName=@UserName, Email=@Email
				, CompanyID=@CompanyID, BranchID=@BranchID
				, IsActive=@IsActive, EntraIDCode=@EntraIDCode
			WHERE ID=@EmployeeID;
		END TRY
		BEGIN CATCH
			SET @IsSuccess=0;
			SET @ErrorCode='EMPLOYEE-CREATE-UPDATEERROR';
			SET @ErrorMsg='EMPLOYEE-CREATE-UPDATEERROR';
			RETURN;
		END CATCH;
	END;

	--
	-- Roles:  If it's not employee, a User entry must exist (User ID = Employee ID)
	--
	BEGIN TRY
		DECLARE @UserID int;
		SELECT @UserID=ID FROM Users WHERE EmployeeID=@EmployeeID
		IF @UserRole<>'Employee' BEGIN
			IF @UserID is null BEGIN
				-- User does not exist for employee
				INSERT INTO Users (ID, UserName, PasswordHash, Email, [Role], EntraIDCode, EmployeeID)
				VALUES (@EmployeeID, @UserName, '', @Email, @UserRole, @EntraIDCode, @EmployeeID);
			END
			ELSE BEGIN
				-- Employee already has user entry
				UPDATE Users
				SET UserName=@UserName, Email=@Email, [Role]=@UserRole, EntraIDCode=@EntraIDCode
				WHERE ID=@UserID;
			END;
		END
		ELSE BEGIN
			-- An Employee Role should not have an entry in Users
			DELETE FROM Users WHERE ID=@UserID;
		END;
	END TRY
	BEGIN CATCH
		SET @IsSuccess=0;
		SET @ErrorCode='EMPLOYEE-CREATE-ROLUPDATEERROR';
		SET @ErrorMsg='EMPLOYEE-CREATE-ROLEUPDATEERROR';
		RETURN;
	END CATCH;

	SET @IsSuccess=1;
	SET @ErrorCode=NULL;
	SET @ErrorMsg=NULL;	
	RETURN;
END;

