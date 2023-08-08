/**************************************************************************************************************
  1. 작성자 : 홍석원
  2. 작성일 : 2023.05.09
  3. 구현내용 : 문자열 SPLIT
  4. 비고 :
    - 예제
       DECLARE @string NVARCHAR(MAX) = '1,2,3'
       DECLARE @delimiter NVARCHAR(1) = ','
  
 	  SELECT SVALUE
 	  FROM [DBO].SPLIT_TO_ROWS(@string, @delimiter)
  5. 수정내역 :  
 **************************************************************************************************************/
CREATE OR ALTER FUNCTION [DBO].[SPLIT_TO_ROWS]
(
    @string NVARCHAR(MAX),
    @delimiter NVARCHAR(1)
)
RETURNS @Result TABLE (SVALUE NVARCHAR(MAX))
AS
BEGIN
    DECLARE @startPos INT, @endPos INT

    SET @startPos = 1
    SET @endPos = CHARINDEX(@delimiter, @string)

    WHILE @endPos > 0
    BEGIN
        DECLARE @value NVARCHAR(MAX)
        SET @value = SUBSTRING(@string, @startPos, @endPos - @startPos)

        INSERT INTO @Result (SVALUE)
        VALUES (@value)

        SET @startPos = @endPos + 1
        SET @endPos = CHARINDEX(@delimiter, @string, @startPos)
        END

    -- 마지막 값 추가
    IF @startPos <= LEN(@string)
    BEGIN
        DECLARE @value1 NVARCHAR(MAX)
        SET @value1 = SUBSTRING(@string, @startPos, LEN(@string) - @startPos + 1)

        INSERT INTO @Result (SVALUE)
        VALUES (@value1)
    END

    RETURN
END