﻿CREATE OR ALTER FUNCTION [DBO].[GET_LASTDAY_OF_MONTH] (@DATE DATE)
    RETURNS INT
AS
BEGIN
    DECLARE @LAST_DAY DATE

    SET @LAST_DAY = DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @DATE) + 1, 0))

    RETURN DAY(@LAST_DAY)
END