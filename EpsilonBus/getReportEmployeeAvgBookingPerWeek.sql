CREATE PROCEDURE [dbo].[getReportEmployeeAvgBookingPerWeek] @StartDate DATE, @EndDate DATE, @BranchID int
AS
BEGIN
	SET NOCOUNT ON;
    SET DATEFIRST 1; -- Set Monday as the first day of the week

    WITH AllBookingDates AS (
        -- recurring bookings expanded to individual dates
        SELECT 
            B.EmployeeID,
            D.DateValue
        FROM Bookings B
        INNER JOIN (
            -- generate dates between @StartDate and @EndDate
            SELECT DATEADD(DAY, v.number, @StartDate) AS DateValue
            FROM master..spt_values v 
            WHERE v.type = 'P' AND DATEADD(DAY, v.number, @StartDate) <= @EndDate
        ) D ON D.DateValue BETWEEN B.StartDate AND ISNULL(B.EndDate, B.StartDate)
        WHERE B.IsDefault = 1
        AND (
            (DATEPART(WEEKDAY, D.DateValue) = 1 AND B.UseMonday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 2 AND B.UseTuesday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 3 AND B.UseWednesday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 4 AND B.UseThursday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 5 AND B.UseFriday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 6 AND B.UseSaturday = 1)
            OR (DATEPART(WEEKDAY, D.DateValue) = 7 AND B.UseSunday = 1)
        )
        UNION
        -- single date bookings
        SELECT EmployeeID,
            StartDate AS DateValue
        FROM Bookings
        WHERE (IsDefault IS NULL OR IsDefault <> 1)
        AND StartDate BETWEEN @StartDate AND @EndDate
    ),
    DistinctBookingDays AS (
        -- distinct days with at least one booking per employee
        SELECT DISTINCT EmployeeID, DateValue
        FROM AllBookingDates
    ),
    BookingDaysPerWeek AS (
        -- count booking days per employee per ISO week and year (to handle cross-years)
        SELECT EmployeeID,
            DATEPART(YEAR, DateValue) AS BookingYear,
            DATEPART(ISO_WEEK, DateValue) AS BookingWeek,
            COUNT(*) AS DaysBookedInWeek
        FROM DistinctBookingDays
        GROUP BY EmployeeID, DATEPART(YEAR, DateValue), DATEPART(ISO_WEEK, DateValue)
    )
    SELECT e.ID AS EmployeeID, e.EmployeeCode, e.EmployeeName
	    , cast(avg(cast(isnull(bwpw.DaysBookedInWeek,0) AS FLOAT)) as decimal (16,1)) AS AvgDaysPerWeek
    FROM Employees e
    LEFT JOIN BookingDaysPerWeek bwpw ON bwpw.EmployeeID=e.ID
    WHERE @BranchID is null OR e.BranchID=@BranchID
    GROUP BY e.ID, e.EmployeeCode, e.EmployeeName
    ORDER BY e.ID;

END
