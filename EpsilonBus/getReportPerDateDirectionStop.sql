CREATE PROCEDURE [dbo].[getReportPerDateDirectionStop] @StartDate DATE, @EndDate DATE, @BranchID INT, @LanguageID int = 1
AS
BEGIN
	SET NOCOUNT ON;
    SET DATEFIRST 1;
    DECLARE @OutboundStr varchar(255)='Outbound';
    DECLARE @InboundStr varchar(255)='Inbound';
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
        WHERE nwd.CalendarDate is null -- exclude non-working days
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
    BranchEmployees AS (
	    -- Employee rows in Branch
        SELECT e.ID AS EmployeeID
        FROM Employees e
	    JOIN Branches br ON br.ID = e.BranchID
        WHERE br.ID=@BranchID
    ),
    ExpandedRecurringBookings AS (
	    -- Recurring Bookings on Operating dates 
        SELECT b.EmployeeID
		    , d.CalendarDate AS DateValue
		    , b.OutboundStopID
		    , b.InboundStopID
        FROM Bookings b
        JOIN OperatingDates d ON d.CalendarDate between B.StartDate AND isnull(b.EndDate, d.CalendarDate)
        JOIN BranchEmployees be ON b.EmployeeID=be.EmployeeID
        WHERE B.IsDefault=1
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
        SELECT b.EmployeeID
            , b.StartDate AS DateValue
            , b.OutboundStopID
            , b.InboundStopID
        FROM Bookings b
        JOIN BranchEmployees be ON b.EmployeeID=be.EmployeeID
        JOIN OperatingDates od ON b.StartDate=od.CalendarDate
        WHERE (b.IsDefault is null OR b.IsDefault<>1)
        AND b.StartDate between @StartDate AND @EndDate
    ),
    AllBookings AS (
        -- All bookings (recurring and single date) on operating dates separate by direction
        SELECT EmployeeID, DateValue, OutboundStopID AS StopID, @OutboundStr AS Direction FROM ExpandedRecurringBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT EmployeeID, DateValue, InboundStopID AS StopID, @InboundStr AS Direction FROM ExpandedRecurringBookings WHERE InboundStopID is not null
        UNION ALL
        SELECT EmployeeID, DateValue, OutboundStopID AS StopID, @OutboundStr AS Direction FROM SingleDateBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT EmployeeID, DateValue, InboundStopID AS StopID, @InboundStr AS Direction FROM SingleDateBookings WHERE InboundStopID is not null
    ),
    DistinctStops AS (
        -- Distinct Stops from Bookings in Branch
        SELECT distinct StopID 
	    FROM (
            SELECT b.OutboundStopID AS StopID 
		    FROM Bookings b
		    JOIN BranchEmployees be ON b.EmployeeID=be.EmployeeID
		    WHERE b.OutboundStopID is not null 
            UNION
            SELECT b.InboundStopID AS StopID 
		    FROM Bookings b
		    JOIN BranchEmployees be ON b.EmployeeID=be.EmployeeID
		    WHERE b.InboundStopID is not null 
        ) AS Stops
    ),
    OperatingDatesWithStops AS (
        -- Operating Dates cross joined with Distinct Stops and Directions
        SELECT od.CalendarDate, s.StopID, v.Direction
        FROM OperatingDates od
        CROSS JOIN DistinctStops s
        CROSS JOIN (VALUES (@OutboundStr), (@InboundStr)) v(Direction)
    )
    SELECT ods.CalendarDate
        , ods.Direction
        , ods.StopID
	    , s.StopCode
	    , isnull(tt.TranslatedText,s.StopName) AS StopName
        , count(distinct ab.EmployeeID) AS EmployeeNum
    FROM OperatingDatesWithStops ods
    LEFT JOIN AllBookings ab ON ab.DateValue=ods.CalendarDate AND ab.StopID=ods.StopID AND ab.Direction=ods.Direction
    LEFT JOIN Stops s ON s.ID=ods.StopID
    LEFT JOIN TranslatedTexts tt ON tt.TableName=@TranlateTable_Stops AND tt.PrimaryKeyFieldName=@TranlateField_StopName AND tt.PrimaryKeyValue=s.ID AND tt.LanguageID=@LanguageID
    GROUP BY ods.CalendarDate, ods.Direction, ods.StopID, s.OrderNum, s.StopCode, s.StopName, tt.TranslatedText
    ORDER BY ods.CalendarDate ASC
	    , case ods.Direction when @OutboundStr then 1 else 2 end
        , case when ods.Direction=@OutboundStr then isnull(s.OrderNum,9999) else -isnull(S.OrderNum,0) end
    OPTION (MAXRECURSION 0);

END
