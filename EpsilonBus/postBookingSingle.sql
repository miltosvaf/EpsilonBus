CREATE PROCEDURE [dbo].[postBookingSingle] @EmployeeID int, @SpecificDate date, @StopID int, @Direction int
	, @LanguageID int=1
	, @IsSuccess int OUTPUT, @ErrorCode varchar(100) OUTPUT, @ErrorMsg nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;	
	IF @EmployeeID is null OR NOT EXISTS (SELECT 1 FROM Employees WHERE ID=@EmployeeID AND isnull(IsActive,0)=1) BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-INVALIDEMPLOYEE';
		SET @ErrorMsg='BOOK-SINGLE-INVALIDEMPLOYEE';
		RETURN;
	END;

	IF @SpecificDate is null BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-INVALIDDATE';
		SET @ErrorMsg='BOOK-SINGLE-INVALIDDATE';
		RETURN;
	END;

	SET DATEFIRST 1;
	DECLARE @OperatesMonday int, @OperatesTuesday int, @OperatesWednesday int, @OperatesThursday int, @OperatesFriday int, @OperatesSaturday int, @OperatesSunday int;
	SELECT @OperatesMonday=case when isnull(OperatesMonday,0)=1 then 1 else 0 end
		, @OperatesTuesday=case when isnull(OperatesTuesday,0)=1 then 1 else 0 end
		, @OperatesWednesday=case when isnull(OperatesWednesday,0)=1 then 1 else 0 end
		, @OperatesThursday=case when isnull(OperatesThursday,0)=1 then 1 else 0 end
		, @OperatesFriday=case when isnull(OperatesFriday,0)=1 then 1 else 0 end
		, @OperatesSaturday=case when isnull(OperatesSaturday,0)=1 then 1 else 0 end
		, @OperatesSunday=case when isnull(OperatesSunday,0)=1 then 1 else 0 end
    FROM Branches b
	JOIN Employees e ON e.BranchID=b.ID
    WHERE e.ID=@EmployeeID;
	IF (DATEPART(WEEKDAY, @SpecificDate)=1 AND @OperatesMonday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=2 AND @OperatesTuesday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=3 AND @OperatesWednesday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=4 AND @OperatesThursday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=5 AND @OperatesFriday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=6 AND @OperatesSaturday=0)
        OR (DATEPART(WEEKDAY, @SpecificDate)=7 AND @OperatesSunday=0)
	BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-NONWORKINGDAY';
		SET @ErrorMsg='BOOK-SINGLE-NONWORKINGDAY';
		RETURN;
	END;
	DECLARE @Int_BlockBeforeDays int;
	SELECT @Int_BlockBeforeDays=isnull(try_cast([Value] AS int),0)
	FROM BusinessLogicRules
	WHERE [Key]='BlockBeforeDays';
	SET @Int_BlockBeforeDays=case when isnull(@Int_BlockBeforeDays,0)<0 then 0 else @Int_BlockBeforeDays end;
	IF @SpecificDate <= DATEADD(DAY,-(@Int_BlockBeforeDays + 1),CAST(getdate() AS date))
	BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-OLDDATE';
		SET @ErrorMsg='BOOK-SINGLE-OLDDATE';
		RETURN;
	END;
	IF @SpecificDate=DATEADD(DAY,-@Int_BlockBeforeDays,CAST(getdate() AS date))
	BEGIN
		DECLARE @Str_BlockAfterHourOnDay nvarchar(max);
		DECLARE @BlockAfterTime time;

		SELECT @Str_BlockAfterHourOnDay=isnull([Value],'23:59:59')
		FROM BusinessLogicRules
		WHERE [Key]='BlockAfterHourOnDay';
		SET @BlockAfterTime=isnull(TRY_CAST(@Str_BlockAfterHourOnDay AS time),'23:59:59');
		IF CONVERT(time,getdate())<@BlockAfterTime
		BEGIN
			SET @IsSuccess=0;
			SET @ErrorCode='BOOK-SINGLE-OLDTIMEONDATE';
			SET @ErrorMsg='BOOK-SINGLE-OLDTIMEONDATE';
			RETURN;
		END
	END;

	IF @StopID is null OR NOT EXISTS (SELECT 1 FROM Stops WHERE ID=@StopID AND isnull(IsActive,0)=1) BEGIN
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-INVALIDSTOP';
		SET @ErrorMsg='BOOK-SINGLE-INVALIDSTOP';
		RETURN;
	END;

	IF @Direction is null OR @Direction not in (1,2,3) BEGIN 
		SET @IsSuccess=0;
		SET @ErrorCode='BOOK-SINGLE-INVALIDDIRECTION';
		SET @ErrorMsg='BOOK-SINGLE-INVALIDDIRECTION';
		RETURN;
	END;

	DECLARE @OutboundStopID int, @InboundStopID int;
	SET @OutboundStopID=case @Direction when 1 then @StopID when 2 then @StopID when 3 then null end;
	SET @InboundStopID=case @Direction when 1 then @StopID when 2 then null when 3 then @StopID end;

	DECLARE @ExistingID int;
	SELECT @ExistingID=ID 
	FROM Bookings
	WHERE EmployeeID=@EmployeeID AND StartDate=@SpecificDate AND isnull(IsDefault,0)=0;
	IF @ExistingID is null BEGIN
		-- New case
		INSERT INTO Bookings (EmployeeID, OutboundStopID, InboundStopID, IsDefault, StartDate, SubmittedDate)
		VALUES (@EmployeeID, @OutboundStopID, @InboundStopID, 0, @SpecificDate, getdate());
	END
	ELSE BEGIN
		-- Update case
		UPDATE Bookings
		SET OutboundStopID=@OutboundStopID, InboundStopID=@InboundStopID, SubmittedDate=getdate()
		WHERE ID=@ExistingID;
	END;

	SET @IsSuccess=1;
	RETURN;
END