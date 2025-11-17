CREATE TABLE [dbo].[Employees]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [EmployeeName] NVARCHAR(255) NOT NULL, 
    [EmployeeCode] NVARCHAR(50) NULL, 
    [UserName] NVARCHAR(50) NULL, 
    [Email] VARCHAR(255) NULL,
    [CompanyID] INT NOT NULL, 
    [BranchID] INT NOT NULL, 
    [IsActive] INT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Employees_Companies FOREIGN KEY (CompanyID) REFERENCES [dbo].[Companies](ID),
	CONSTRAINT FK_Employees_Branches FOREIGN KEY (BranchID) REFERENCES [dbo].[Branches](ID)
)
