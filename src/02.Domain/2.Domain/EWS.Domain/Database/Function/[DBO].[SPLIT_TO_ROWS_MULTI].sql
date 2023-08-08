/**************************************************************************************************************
  1. 작성자 : 홍석원
  2. 작성일 : 2023.05.09
  3. 구현내용 : 문자열 SPLIT
  4. 비고 :
    - 예제
       DECLARE @string NVARCHAR(MAX) = '1§A,2§B,3§C'
       DECLARE @delimiter NVARCHAR(1) = ','
  
 	  SELECT SKEY, SVALUE
 	  FROM [DBO].SPLIT_STRING(@string, @delimiter)
  5. 수정내역 :  
 **************************************************************************************************************/
 
CREATE OR ALTER FUNCTION [DBO].[SPLIT_TO_ROWS_MULTI]
(
    @string NVARCHAR(MAX),
    @delimiter NVARCHAR(1)
)
RETURNS @Result TABLE (SKEY NVARCHAR(MAX), SVALUE NVARCHAR(MAX))
AS
BEGIN
    DECLARE @startPos INT, @endPos INT
    
    SET @startPos = 1
    SET @endPos = CHARINDEX(@delimiter, @string)
    
    WHILE @endPos > 0
    BEGIN
        DECLARE @value NVARCHAR(MAX)
        SET @value = SUBSTRING(@string, @startPos, @endPos - @startPos)
        
        DECLARE @key NVARCHAR(MAX), @val NVARCHAR(MAX)
        SET @key = NULL
        SET @val = NULL
        
        -- § 기호로 분리하기
        IF CHARINDEX('§', @value) > 0
        BEGIN
            SET @key = SUBSTRING(@value, 1, CHARINDEX('§', @value) - 1)
            SET @val = SUBSTRING(@value, CHARINDEX('§', @value) + 1, LEN(@value))
        END
        ELSE
        BEGIN
            SET @val = @value
        END

        INSERT INTO @Result (SKEY, SVALUE)
        VALUES (@key, @val)

        SET @startPos = @endPos + 1
        SET @endPos = CHARINDEX(@delimiter, @string, @startPos)
    END
    
    -- 마지막 값 추가
    IF @startPos <= LEN(@string)
    BEGIN
        DECLARE @value1 NVARCHAR(MAX)
        SET @value1 = SUBSTRING(@string, @startPos, LEN(@string) - @startPos + 1)
        
        DECLARE @key1 NVARCHAR(MAX), @val1 NVARCHAR(MAX)
        SET @key1 = NULL
        SET @val1 = NULL
        
        -- § 기호로 분리하기
        IF CHARINDEX('§', @value1) > 0
        BEGIN
            SET @key1 = SUBSTRING(@value1, 1, CHARINDEX('§', @value1) - 1)
            SET @val1 = SUBSTRING(@value1, CHARINDEX('§', @value1) + 1, LEN(@value1))
        END
        ELSE
        BEGIN
            SET @val1 = @value1
        END

        INSERT INTO @Result (SKEY, SVALUE)
        VALUES (@key1, @val1)
    END
    
    RETURN
END