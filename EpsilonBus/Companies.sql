CREATE TABLE [dbo].[Companies]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [CompanyCode] VARCHAR(50) NOT NULL, 
    [CompanyName] NVARCHAR(255) NOT NULL, 
    [IsActive] INT NOT NULL DEFAULT 1
)
