﻿CREATE OR ALTER FUNCTION [DBO].[CONVERT_INT_TO_DATE2] (@YEAR INT, @MONTH INT, @DAY INT)
    RETURNS DATE
AS
BEGIN
    DECLARE @DATE DATE

    SET @DATE = CAST(CAST(@YEAR AS VARCHAR) + '-' + CAST(@MONTH AS VARCHAR) + '-' + CAST(@DAY AS VARCHAR) AS DATE)

    RETURN @DATE
END