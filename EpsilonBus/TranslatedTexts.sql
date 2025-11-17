CREATE TABLE [dbo].[TranslatedTexts]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TableName] VARCHAR(MAX) NOT NULL, 
    [PrimaryKeyFieldName] VARCHAR(MAX) NOT NULL, 
    [PrimaryKeyValue] INT NOT NULL, 
    [LanguageID] INT NOT NULL, 
    [TranslatedText] NVARCHAR(MAX) NOT NULL
)
