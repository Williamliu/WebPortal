Create VIEW VW_Pub_User_FirstMenu
AS
SELECT * FROM Pub_Menu WHERE Deleted=0 AND Active=1 AND Position = 2