CREATE VIEW [dbo].[VW_Pub_PrivateMenu_SecondMenu]
AS
SELECT b.* 
	FROM Pub_Menu a
	INNER JOIN Pub_Menu b ON b.ParentId = a.Id
	WHERE  
		a.Deleted=0 AND a.Active=1 AND 
		b.Deleted=0 AND b.Active=1 AND
		a.Position=2
