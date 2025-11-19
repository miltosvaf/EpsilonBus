CREATE PROCEDURE [dbo].[getStops] @BranchID int, @ShowOnlyActive int, @LanguageID int=1
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Translation_TableName varchar(255) = 'dbo.Stops';
	DECLARE @Translation_PrimaryKeyName varchar(255) = 'StopName';

	SELECT s.ID as StopID, s.StopCode, isnull(tt.TranslatedText, s.StopName) AS StopName, IsActive, s.OrderNum, s.Latitude, s.Longitude
	FROM Stops s
	LEFT JOIN TranslatedTexts tt ON tt.TableName= @Translation_TableName and tt.PrimaryKeyFieldName=@Translation_PrimaryKeyName and tt.PrimaryKeyValue=s.ID AND tt.LanguageID=@LanguageID
	WHERE s.BranchID=@BranchID
	AND (isnull(@ShowOnlyActive,0)=0 OR isnull(s.IsActive,0)=1)
	ORDER BY s.OrderNum;
END
