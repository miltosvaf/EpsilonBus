CREATE PROCEDURE [dbo].[getBusinessDaysInRange] @StartDate date, @EndDate date, @BranchID int
AS
BEGIN
	SET NOCOUNT ON;
	SET DATEFIRST 1;
    WITH DateRange AS (
        SELECT @StartDate AS CalendarDate
        UNION ALL
        SELECT DATEADD(DAY, 1, CalendarDate)
        FROM DateRange
        WHERE DATEADD(DAY, 1, CalendarDate) <= @EndDate
    ),
    BranchOpers AS (
        SELECT ID, OperatesMonday, OperatesTuesday, OperatesWednesday, OperatesThursday, OperatesFriday, OperatesSaturday, OperatesSunday
        FROM Branches
        WHERE ID=@BranchID
    )
    SELECT DR.CalendarDate AS BusinessDay
    FROM DateRange DR
    CROSS JOIN BranchOpers BO
    LEFT JOIN NonWorkingDays NWD ON NWD.BranchID = @BranchID AND NWD.CalendarDate = DR.CalendarDate
    WHERE 
        NWD.CalendarDate IS NULL -- exclude non-working days
        AND (
            (DATEPART(WEEKDAY, DR.CalendarDate) = 1 AND BO.OperatesMonday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 2 AND BO.OperatesTuesday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 3 AND BO.OperatesWednesday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 4 AND BO.OperatesThursday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 5 AND BO.OperatesFriday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 6 AND BO.OperatesSaturday = 1)
            OR (DATEPART(WEEKDAY, DR.CalendarDate) = 7 AND BO.OperatesSunday = 1)
        )
    ORDER BY DR.CalendarDate
    OPTION (MAXRECURSION 0);

END
