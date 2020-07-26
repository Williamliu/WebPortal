CREATE FUNCTION dbo.ToTicks ( @DateTime DateTime )
  RETURNS bigint
AS
BEGIN
	RETURN DATEDIFF(SECOND, CAST('1970-01-01 00:00:00' AS datetime),  @DateTime)
END