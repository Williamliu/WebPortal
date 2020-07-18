
CREATE View [dbo].[VW_Admin_User_Branch_Full]
AS
SELECT        a.Id, b.BranchId
FROM            dbo.Admin_User AS a INNER JOIN
                         dbo.Admin_User_Branch AS b ON a.Id = b.UserId AND a.Active = 1 AND a.Deleted = 0 INNER JOIN
                         dbo.GBranch AS c ON b.BranchId = c.Id AND c.Deleted = 0
