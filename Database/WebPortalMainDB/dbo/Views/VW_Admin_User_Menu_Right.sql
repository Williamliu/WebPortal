
CREATE VIEW [dbo].[VW_Admin_User_Menu_Right]
AS
SELECT DISTINCT d.UserId, c.MenuId, b.Action
FROM            dbo.Admin_Role_Right AS a INNER JOIN
                         dbo.Admin_Right AS b ON a.RightId = b.Id AND b.Active = 1 AND b.Deleted = 0 INNER JOIN
                         dbo.Admin_Menu AS c ON a.MenuId = c.Id AND c.Active = 1 AND c.Deleted = 0 INNER JOIN
                         dbo.Admin_User_Role AS d ON a.RoleId = d.RoleId INNER JOIN
                         dbo.Admin_User AS e ON d.UserId = e.Id AND e.Active = 1 AND e.Deleted = 0
