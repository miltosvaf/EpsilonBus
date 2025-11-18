CREATE PROCEDURE [dbo].[getReportSingleDatePerDirectionStopDetailedList] @SpecificDate DATE, @BranchID INT, @LanguageID int
AS
BEGIN
	SET NOCOUNT ON;
    SET DATEFIRST 1;
    DECLARE @OutboundStr varchar(255) = 'Outbound';
    DECLARE @InboundStr varchar(255) = 'Inbound';

    WITH BranchEmployees AS (
	    -- Employee rows in Branch
        SELECT e.ID AS EmployeeID
        FROM Employees e
	    JOIN Branches br ON br.ID = e.BranchID
        WHERE br.ID=@BranchID
    ),
    ExpandedRecurringBookings AS (
	    -- Recurring Bookings on Operating dates 
        SELECT b.EmployeeID
		    , b.OutboundStopID
		    , b.InboundStopID
        FROM Bookings b
        JOIN BranchEmployees be ON b.EmployeeID=be.EmployeeID
        WHERE b.IsDefault=1
	    AND @SpecificDate between b.StartDate and isnull(b.EndDate,'2049-12-31')
        AND (
            (DATEPART(WEEKDAY,@SpecificDate)=1 AND b.UseMonday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=2 AND b.UseTuesday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=3 AND b.UseWednesday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=4 AND b.UseThursday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=5 AND b.UseFriday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=6 AND b.UseSaturday=1)
            OR (DATEPART(WEEKDAY,@SpecificDate)=7 AND b.UseSunday=1)
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
        WHERE (b.IsDefault is null OR b.IsDefault<>1)
        AND b.StartDate=@SpecificDate
    ),
    AllBookings AS (
        SELECT EmployeeID, OutboundStopID AS StopID, @OutboundStr AS Direction FROM ExpandedRecurringBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT EmployeeID, InboundStopID AS StopID, @InboundStr AS Direction FROM ExpandedRecurringBookings WHERE InboundStopID is not null
        UNION ALL
        SELECT EmployeeID, OutboundStopID AS StopID, @OutboundStr AS Direction FROM SingleDateBookings WHERE OutboundStopID is not null
        UNION ALL
        SELECT EmployeeID, InboundStopID AS StopID, @InboundStr AS Direction FROM SingleDateBookings WHERE InboundStopID is not null
    ),
    DistinctStops AS (
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
        SELECT @SpecificDate AS CalendarDate, s.StopID, v.Direction
        FROM DistinctStops s
        CROSS JOIN (VALUES (@OutboundStr), (@InboundStr)) v(Direction)
    )
    SELECT ods.Direction
        , ods.StopID
	    , s.StopCode
	    , isnull(tt.TranslatedText,s.StopName) AS StopName
        , ab.EmployeeID
	    , e.EmployeeName
    FROM OperatingDatesWithStops ODS
    LEFT JOIN AllBookings ab ON ods.CalendarDate=@SpecificDate AND ab.StopID=ods.StopID AND ab.Direction=ods.Direction
    LEFT JOIN Stops s ON s.ID=ods.StopID
    LEFT JOIN TranslatedTexts tt ON tt.TableName='[dbo].[Stops]' AND tt.PrimaryKeyFieldName='StopName' AND tt.PrimaryKeyValue=s.ID AND tt.LanguageID=@LanguageID
    LEFT JOIN Employees e ON ab.EmployeeID=e.ID
    ORDER BY ods.CalendarDate ASC
	    , case ods.Direction when @OutboundStr then 1 else 2 end
        , case when ods.Direction=@OutboundStr then isnull(s.OrderNum,9999) else -isnull(S.OrderNum,0) end
    OPTION (MAXRECURSION 0);

END
