CREATE PROCEDURE [dbo].[postSingleCancelation] @EmployeeID int, @SpecificDate date
	, @LanguageID int=1
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

BEGIN TRY
        -- 1. Try to delete a specific date booking (IsDefault=0)
        DELETE FROM Bookings
        WHERE EmployeeID = @EmployeeID
          AND IsDefault = 0
          AND StartDate = @SpecificDate;

        -- 2. Check for a matching recurring booking (IsDefault=1)
        DECLARE @RecID int, @RecStart date, @RecEnd date,
                @RecOutboundStopID int, @RecInboundStopID int,
                @RecUseMonday int, @RecUseTuesday int, @RecUseWednesday int, @RecUseThursday int, @RecUseFriday int, @RecUseSaturday int, @RecUseSunday int;

        SELECT TOP 1
            @RecID = ID,
            @RecStart = StartDate,
            @RecEnd = EndDate,
            @RecOutboundStopID = OutboundStopID,
            @RecInboundStopID = InboundStopID,
            @RecUseMonday = UseMonday,
            @RecUseTuesday = UseTuesday,
            @RecUseWednesday = UseWednesday,
            @RecUseThursday = UseThursday,
            @RecUseFriday = UseFriday,
            @RecUseSaturday = UseSaturday,
            @RecUseSunday = UseSunday
        FROM Bookings
        WHERE EmployeeID = @EmployeeID
          AND IsDefault = 1
          AND StartDate <= @SpecificDate
          AND (EndDate IS NULL OR EndDate >= @SpecificDate)
          AND (
                (DATEPART(WEEKDAY, @SpecificDate) = 1 AND ISNULL(UseMonday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 2 AND ISNULL(UseTuesday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 3 AND ISNULL(UseWednesday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 4 AND ISNULL(UseThursday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 5 AND ISNULL(UseFriday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 6 AND ISNULL(UseSaturday,0) = 1) OR
                (DATEPART(WEEKDAY, @SpecificDate) = 7 AND ISNULL(UseSunday,0) = 1)
              );

        IF @RecID IS NOT NULL
        BEGIN
            -- a) Update the existing recurring booking to end the day before @SpecificDate
            UPDATE Bookings
            SET EndDate = DATEADD(DAY, -1, @SpecificDate)
            WHERE ID = @RecID;

            -- b) Insert a new recurring booking starting the day after @SpecificDate
            INSERT INTO Bookings (
                EmployeeID, OutboundStopID, InboundStopID, IsDefault, StartDate, EndDate, SubmittedDate,
                UseMonday, UseTuesday, UseWednesday, UseThursday, UseFriday, UseSaturday, UseSunday
            )
            VALUES (
                @EmployeeID, @RecOutboundStopID, @RecInboundStopID, 1,
                DATEADD(DAY, 1, @SpecificDate), @RecEnd, GETDATE(),
                @RecUseMonday, @RecUseTuesday, @RecUseWednesday, @RecUseThursday, @RecUseFriday, @RecUseSaturday, @RecUseSunday
            );
        END

        SET @IsSuccess = 1;
        SET @ErrorCode = NULL;
        SET @ErrorMsg = NULL;
    END TRY
    BEGIN CATCH
        SET @IsSuccess = 0;
        SET @ErrorCode = 'CANCEL-SINGLE-ERROR';
        SET @ErrorMsg = 'CANCEL-SINGLE-ERROR';
    END CATCH
END