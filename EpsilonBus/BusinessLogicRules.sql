CREATE TABLE [dbo].[BusinessLogicRules]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(MAX) NULL, 
    CONSTRAINT [AK_BusinessLogicRules_Key] UNIQUE ([Key])
)
