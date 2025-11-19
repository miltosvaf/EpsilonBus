CREATE PROCEDURE [dbo].[getEmployeeCalendar] @StartDate DATE, @EndDate DATE, @EmployeeID INT, @LanguageID int = 1
AS
BEGIN
    --
    -- Calendar of Employee Bookings
    -- Per Date in provided range (StartDate to EndDate) - on days of the week Branch is operating without nonworking dates
    -- Per Direction (Outbound/Inbound)
    -- Per Stop (translated via TranslatedTexts table for LanguageID provided)
    --
	SET NOCOUNT ON;
    SET DATEFIRST 1;

    DECLARE @BranchID INT;
    SELECT @BranchID=BranchID FROM Employees WHERE ID=@EmployeeID;

    SET DATEFIRST 1;
    DECLARE @TranlateTable_Stops varchar(255)='[dbo].[Stops]';
    DECLARE @TranlateField_StopName varchar(255)='StopName';

    WITH DateRange AS (
	    -- Date rows in range
        SELECT @StartDate AS CalendarDate
        UNION ALL
        SELECT DATEADD(DAY,1,CalendarDate)
        FROM DateRange
        WHERE DATEADD(DAY,1,CalendarDate)<=@EndDate
    ),
    BranchOpers AS (
	    -- Days of Week Branch Operate
        SELECT ID, OperatesMonday, OperatesTuesday, OperatesWednesday, OperatesThursday, OperatesFriday, OperatesSaturday, OperatesSunday
        FROM Branches
        WHERE ID=@BranchID
    ),
    OperatingDates AS (
	    -- Date rows Branch operates, without non working dates
        SELECT dr.CalendarDate
        FROM DateRange dr
        CROSS JOIN BranchOpers bo
        LEFT JOIN NonWorkingDays nwd ON nwd.BranchID=@BranchID AND nwd.CalendarDate=dr.CalendarDate
        WHERE nwd.CalendarDate is null
        AND (
            (DATEPART(WEEKDAY, dr.CalendarDate)=1 AND bo.OperatesMonday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=2 AND bo.OperatesTuesday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=3 AND bo.OperatesWednesday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=4 AND bo.OperatesThursday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=5 AND bo.OperatesFriday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=6 AND bo.OperatesSaturday=1)
            OR (DATEPART(WEEKDAY, dr.CalendarDate)=7 AND bo.OperatesSunday=1)
        )
    ),
    ExpandedRecurringBookings AS (
	    -- Recurring Bookings on Operating dates 
        SELECT d.CalendarDate AS DateValue
		    , b.OutboundStopID
		    , b.InboundStopID
			, 1 AS IsDefault
        FROM Bookings b
        JOIN OperatingDates d ON d.CalendarDate between B.StartDate AND isnull(b.EndDate, d.CalendarDate)
        WHERE b.EmployeeID=@EmployeeID 
		AND b.IsDefault=1
        AND (
            (DATEPART(WEEKDAY,d.CalendarDate)=1 AND b.UseMonday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=2 AND b.UseTuesday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=3 AND b.UseWednesday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=4 AND b.UseThursday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=5 AND b.UseFriday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=6 AND b.UseSaturday=1)
            OR (DATEPART(WEEKDAY,d.CalendarDate)=7 AND b.UseSunday=1)
        )
    ),
    SingleDateBookings AS (
	    -- Bookings on specific date
        SELECT b.StartDate AS DateValue
            , b.OutboundStopID
            , b.InboundStopID
			, 0 AS IsDefault
        FROM Bookings b
        JOIN OperatingDates od ON b.StartDate=od.CalendarDate
        WHERE b.EmployeeID=@EmployeeID 
		AND (b.IsDefault is null OR b.IsDefault<>1)
        AND b.StartDate between @StartDate AND @EndDate
    ),
    AllBookings AS (
        -- All bookings (recurring and single date) on operating dates separate by direction
        SELECT DateValue, OutboundStopID AS StopID, 1 AS Direction, IsDefault FROM ExpandedRecurringBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT DateValue, InboundStopID AS StopID, 2 AS Direction, IsDefault FROM ExpandedRecurringBookings WHERE InboundStopID is not null
        UNION ALL
        SELECT DateValue, OutboundStopID AS StopID, 1 AS Direction, IsDefault FROM SingleDateBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT DateValue, InboundStopID AS StopID, 2 AS Direction, IsDefault FROM SingleDateBookings WHERE InboundStopID is not null
    ),
    RankedBookings AS (
        SELECT
            ab.DateValue,
            ab.StopID,
            ab.Direction,
            ab.IsDefault,
            ROW_NUMBER() OVER (PARTITION BY ab.DateValue, ab.Direction ORDER BY ab.IsDefault) AS rn
        FROM AllBookings ab
    ),
    DateBookings AS (
        SELECT od.CalendarDate
            , abo.StopID AS OutboundStopID, s_o.StopCode AS OutboundStopCode, isnull(tt.TranslatedText, s_o.StopName) AS OutboundStopName
            , abi.StopID AS InboundStopID, s_i.StopCode AS InboundStopCode, isnull(tti.TranslatedText, s_i.StopName) AS InboundStopName
        FROM OperatingDates od
        LEFT JOIN RankedBookings abo ON od.CalendarDate=abo.DateValue AND abo.Direction=1 AND abo.rn=1
        LEFT JOIN RankedBookings abi ON od.CalendarDate=abi.DateValue AND abi.Direction=2 AND abi.rn=1
        LEFT JOIN Stops s_o ON abo.StopID=s_o.ID
        LEFT JOIN TranslatedTexts tt ON tt.TableName=@TranlateTable_Stops AND tt.PrimaryKeyFieldName=@TranlateField_StopName AND tt.PrimaryKeyValue=s_o.ID AND tt.LanguageID=@LanguageID
        LEFT JOIN Stops s_i ON abi.StopID=s_i.ID
        LEFT JOIN TranslatedTexts tti ON tti.TableName=@TranlateTable_Stops AND tti.PrimaryKeyFieldName=@TranlateField_StopName AND tti.PrimaryKeyValue=s_i.ID AND tti.LanguageID=@LanguageID
    )
	SELECT CalendarDate, OutboundStopCode, OutboundStopName, InboundStopCode, InboundStopName
	FROM DateBookings
	WHERE OutboundStopID is not null OR InboundStopID is not null
    OPTION (MAXRECURSION 0);

END