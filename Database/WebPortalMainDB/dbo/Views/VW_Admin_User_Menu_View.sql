
CREATE VIEW [dbo].[VW_Admin_User_Menu_View]
AS
SELECT DISTINCT f.Id, a.MenuId, c.MenuId AS Menu, a.RightId, b.Action
FROM Admin_Role_Right a 
INNER JOIN Admin_Right b ON (a.RightId = b.Id AND b.Active=1 AND Deleted=0 AND b.Action='view')
INNER JOIN Admin_Menu c ON (a.MenuId = c.Id AND c.Active=1 AND c.Deleted=0)
INNER JOIN Admin_Role d ON (a.RoleId = d.Id AND d.Active=1 AND d.Deleted=0)
INNER JOIN Admin_User_Role e ON (a.RoleId = e.RoleId)
INNER JOIN Admin_User f ON (e.UserId = f.Id AND f.Active=1 AND f.Deleted=0)
