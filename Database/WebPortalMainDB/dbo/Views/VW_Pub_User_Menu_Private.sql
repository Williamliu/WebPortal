
CREATE VIEW [dbo].[VW_Pub_User_Menu_Private]
AS
-- Pub Menu:  Position=2  Private Menu;  Position=1 Public Menu
SELECT DISTINCT
	e.Id AS UserId, b.Id AS Grp, b.*
	FROM Pub_Role_Menu a
	INNER JOIN Pub_Menu b 
		ON (b.Deleted=0 AND b.Active=1 AND a.MenuId=b.Id AND 
			b.Position=2 AND b.ParentId=0)
	INNER JOIN Pub_Role c ON (c.Deleted=0 AND c.Active=1 AND a.RoleId=c.Id) 
	INNER JOIN Pub_User_Role d ON (a.RoleId=d.RoleId)
	INNER JOIN Pub_User e ON (e.Deleted=0 AND e.Active=1 AND d.UserId=e.Id)
UNION
-- Private Menu - Sub Menus
SELECT privateRoot.UserId, Pub_Menu.ParentId AS Grp, Pub_Menu.* 
	FROM Pub_Menu
	INNER JOIN 
	(
		SELECT DISTINCT 
			e.Id AS UserId, b.Id
		FROM Pub_Role_Menu a
		INNER JOIN Pub_Menu b 
			ON (b.Deleted=0 AND b.Active=1 AND a.MenuId=b.Id AND 
				b.Position=2 AND b.ParentId=0)
		INNER JOIN Pub_Role c ON (c.Deleted=0 AND c.Active=1 AND a.RoleId=c.Id) 
		INNER JOIN Pub_User_Role d ON (a.RoleId=d.RoleId)
		INNER JOIN Pub_User e ON (e.Deleted=0 AND e.Active=1 AND d.UserId=e.Id)
	) privateRoot
	ON Pub_Menu.ParentId = privateRoot.Id
