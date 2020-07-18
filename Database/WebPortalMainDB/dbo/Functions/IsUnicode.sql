CREATE FUNCTION [dbo].[IsUnicode](@text nvarchar(max)) 
RETURNS bit
AS
BEGIN
IF(len(CAST(@text AS varchar(max)))=len(@text))
return 0
return 1
END
