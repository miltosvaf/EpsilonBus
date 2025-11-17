CREATE TABLE [dbo].[NonWorkingDays]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BranchID] INT NOT NULL, 
    [CalendarDate] DATE NOT NULL,   
    CONSTRAINT FK_NonWorkingDays_Branches FOREIGN KEY (BranchID) REFERENCES [dbo].[Branches](ID)
)
