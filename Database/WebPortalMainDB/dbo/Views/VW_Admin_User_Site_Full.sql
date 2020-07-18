
CREATE View [dbo].[VW_Admin_User_Site_Full]
AS
SELECT        a.Id, b.SiteId
FROM            dbo.Admin_User AS a INNER JOIN
                         dbo.Admin_User_Site AS b ON a.Id = b.UserId AND a.Active = 1 AND a.Deleted = 0 INNER JOIN
                         dbo.GSite AS c ON b.SiteId = c.Id AND c.Deleted = 0 INNER JOIN
                         dbo.GBranch AS d ON c.BranchId = d.Id AND d.Deleted = 0
