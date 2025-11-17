CREATE TABLE [dbo].[Bookings]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EmployeeID] INT NOT NULL, 
    [OutboundStopID] INT NULL, 
    [InboundStopID] INT NULL, 
    [IsDefault] INT NOT NULL DEFAULT 1,
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NULL, 
    [SubmittedDate] DATE NOT NULL, 
    [UseMonday] INT NULL, 
    [UseTuesday] INT NULL, 
    [UseWednesday] INT NULL, 
    [UseThursday] INT NULL, 
    [UseFriday] INT NULL, 
    [UseSaturday] INT NULL, 
    [UseSunday] INT NULL,
	CONSTRAINT FK_Bookings_Employees FOREIGN KEY (EmployeeID) REFERENCES [dbo].[Employees](ID),
	CONSTRAINT FK_Bookings_OutboundStop FOREIGN KEY (OutboundStopID) REFERENCES [dbo].[Stops](ID),
	CONSTRAINT FK_Bookings_InboundStop FOREIGN KEY (InboundStopID) REFERENCES [dbo].[Stops](ID)
)
