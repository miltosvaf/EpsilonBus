CREATE TABLE [dbo].[Users]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Username] VARCHAR(255) NOT NULL, 
    [PasswordHash] NVARCHAR(MAX) NOT NULL
)
