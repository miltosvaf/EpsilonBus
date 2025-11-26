CREATE PROCEDURE [dbo].[postBookingMass] @EmployeeID int, @Option int
	, @StopID int
	, @Direction int
	, @StartDate date, @EndDate date
	, @ApplyMonday int, @ApplyTuesday int, @ApplyWednesday int, @ApplyThursday int, @ApplyFriday int, @ApplySaturday int, @ApplySunday int
	, @LanguageID int=1
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    -- Validation: EmployeeID
    IF @EmployeeID IS NULL OR NOT EXISTS (SELECT 1 FROM Employees WHERE ID = @EmployeeID)
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDEMPLOYEE';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDEMPLOYEE';
        RETURN;
    END;

    -- Validation: Option
    IF @Option IS NULL OR @Option NOT IN (1, 2)
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDOPTION';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDOPTION';
        RETURN;
    END;

    -- Validation: StopID
    IF @StopID IS NULL OR NOT EXISTS (SELECT 1 FROM Stops WHERE ID = @StopID)
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDSTOP';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDSTOP';
        RETURN;
    END;

    -- Validation: Direction
    IF @Direction IS NULL OR @Direction NOT IN (1, 2, 3)
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDDIRECTION';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDDIRECTION';
        RETURN;
    END;

    -- Validation: StartDate
    IF @StartDate IS NULL
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDSTARTDATE';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDSTARTDATE';
        RETURN;
    END;

    -- Validation: EndDate
    IF @EndDate IS NOT NULL AND @EndDate < @StartDate
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-INVALIDENDDATE';
        SET @ErrorMsg = 'BOOK-MASS-INVALIDENDDATE';
        RETURN;
    END;

    -- Validation: At least one day for Option=2
    IF @Option = 2 AND
       ISNULL(@ApplyMonday,0) + ISNULL(@ApplyTuesday,0) + ISNULL(@ApplyWednesday,0) +
       ISNULL(@ApplyThursday,0) + ISNULL(@ApplyFriday,0) + ISNULL(@ApplySaturday,0) + ISNULL(@ApplySunday,0) = 0
    BEGIN
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-NODAYS';
        SET @ErrorMsg = 'BOOK-MASS-NODAYS';
        RETURN;
    END;

    -- Overlap check and update
    BEGIN TRY
        -- 1. Update all overlapping bookings
        UPDATE Bookings
        SET EndDate = DATEADD(DAY, -1, @StartDate)
        WHERE EmployeeID = @EmployeeID
          AND IsDefault = 1
          AND StartDate <= @StartDate
          AND (EndDate IS NULL OR EndDate >= @StartDate);

        -- 2. Delete bookings where EndDate < StartDate
        DELETE FROM Bookings
        WHERE EmployeeID = @EmployeeID
          AND IsDefault = 1
          AND EndDate IS NOT NULL
          AND EndDate < StartDate;

        -- 3. Insert new booking
        INSERT INTO Bookings (
            EmployeeID,
            OutboundStopID,
            InboundStopID,
            IsDefault,
            StartDate,
            EndDate,
            SubmittedDate,
            UseMonday,
            UseTuesday,
            UseWednesday,
            UseThursday,
            UseFriday,
            UseSaturday,
            UseSunday
        )
        VALUES (
            @EmployeeID,
            CASE WHEN @Direction IN (1, 3) THEN @StopID ELSE NULL END,
            CASE WHEN @Direction IN (2, 3) THEN @StopID ELSE NULL END,
            1,
            @StartDate,
            @EndDate,
            GETDATE(),
            @ApplyMonday,
            @ApplyTuesday,
            @ApplyWednesday,
            @ApplyThursday,
            @ApplyFriday,
            @ApplySaturday,
            @ApplySunday
        );

        SET @IsSuccess = 1;
        SET @ErrorCode = NULL;
        SET @ErrorMsg = NULL;
    END TRY
    BEGIN CATCH
        SET @IsSuccess = 0;
        SET @ErrorCode = 'BOOK-MASS-ERROR';
        SET @ErrorMsg = 'BOOK-MASS-ERROR';
    END CATCH;	
END
