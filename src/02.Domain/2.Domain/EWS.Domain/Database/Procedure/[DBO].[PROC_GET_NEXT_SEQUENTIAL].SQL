CREATE OR ALTER PROCEDURE [DBO].[PROC_GET_NEXT_SEQUENTIAL]
(
	@TENANT_ID  NVARCHAR(20),
	@TABLE_NAME NVARCHAR(100),
	@YEAR       INT,
	@MONTH      INT,
	@DAY        INT
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	MERGE [system].[Sequentials] AS T
	USING (
		SELECT @TENANT_ID AS TenantId
			 , @TABLE_NAME AS TableName
			 , @YEAR AS Year
			 , @MONTH AS Month
			 , @DAY AS Day
	) AS S
	ON (T.TenantId = S.TenantId 
		AND T.TableName = S.TableName 
		AND T.Year = S.Year 
		AND T.Month = S.Month 
		AND T.Day = S.Day
	)
	WHEN MATCHED THEN
	  UPDATE SET Seq = ISNULL(Seq, 0) + 1
	WHEN NOT MATCHED THEN
	  INSERT (TenantId, TableName, Year, Month, Day, Seq)
	  VALUES (S.TenantId, S.TableName, S.Year, S.Month, S.Day, 1);


	SELECT MAX(SEQ) AS Seq
	FROM [system].[Sequentials] WITH(NOLOCK)
	WHERE TenantId = @TENANT_ID
	AND TableName = @TABLE_NAME
	AND Year = @YEAR
	AND Month = @MONTH
	AND Day = @DAY

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
	SET NOCOUNT OFF;

END