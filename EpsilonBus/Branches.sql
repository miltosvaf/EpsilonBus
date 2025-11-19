CREATE TABLE [dbo].[Branches]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [CompanyID] INT NULL, 
    [BranchCode] VARCHAR(50) NOT NULL, 
    [BranchName] NVARCHAR(255) NOT NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL, 
    [OperatesMonday] INT NULL, 
    [OperatesTuesday] INT NULL, 
    [OperatesWednesday] INT NULL, 
    [OperatesThursday] INT NULL, 
    [OperatesFriday] INT NULL, 
    [OperatesSaturday] INT NULL, 
    [OperatesSunday] INT NULL ,
	CONSTRAINT FK_Branches_Companies FOREIGN KEY (CompanyID) REFERENCES [dbo].[Companies](ID)
)
