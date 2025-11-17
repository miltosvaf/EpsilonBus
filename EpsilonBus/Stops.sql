CREATE TABLE [dbo].[Stops]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BranchID] INT NOT NULL, 
    [StopCode] VARCHAR(50) NOT NULL, 
    [StopName] NVARCHAR(50) NOT NULL, 
    [OrderNum] INT NULL, 
    [IsActive] INT NOT NULL DEFAULT 1, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL
)
