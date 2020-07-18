CREATE VIEW [dbo].[VW_Pub_User_Menu_Public]
AS
-- Pub Menu:  Position=2  Private Menu;  Position=1 Public Menu
SELECT 
	Pub_Menu.Id as Grp, *
	FROM Pub_Menu 
	WHERE Deleted=0 AND Active=1 AND Position=1 AND ParentId = 0 
UNION
SELECT 
	Pub_Menu.ParentId AS Grp,
	Pub_Menu.*
	FROM Pub_Menu
	INNER JOIN 
	(
		SELECT 
			Id 
			FROM Pub_Menu 
			WHERE Deleted=0 AND Active=1 AND Position=1 AND ParentId = 0 
	) pubRoot
	ON pubRoot.Id = Pub_Menu.ParentId
