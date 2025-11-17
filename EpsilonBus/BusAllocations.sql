CREATE TABLE [dbo].[BusAllocations]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BranchID] INT NOT NULL, 
    [TypeID] INT NOT NULL, 
    [AllocationNum] INT NOT NULL, 
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NULL,
	CONSTRAINT FK_BusAllocation_Branches FOREIGN KEY (BranchID) REFERENCES [dbo].[Branches](ID),
	CONSTRAINT FK_BusAllocations_BusTypes FOREIGN KEY (TypeID) REFERENCES [dbo].[BusTypes](ID)
)
