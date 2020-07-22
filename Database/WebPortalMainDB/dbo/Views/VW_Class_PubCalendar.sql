
CREATE VIEW [dbo].[VW_Class_PubCalendar]
AS
SELECT   
d.Country,
c.BranchId, 
c.SiteId, 
a.Id AS ClassId, 
a.ClassName,
a.Color,
ROW_NUMBER() Over(partition by b.classId ORDER BY b.ClassDate, b.StartTime) as DateNo,
a.Title_en AS Class_Title_en, 
a.Title_cn AS Class_Title_cn, 
a.Detail_en AS Class_Detail_en, 
a.Detail_cn AS Class_Detail_cn, 
a.StartDate, 
a.EndDate, 
a.Status AS ClassStatus, 
b.Id AS DateId, 
b.Title_en AS Date_Title_en, 
b.Title_cn AS Date_Title_cn, 
b.Detail_en AS Date_Detail_en, 
b.Detail_cn AS Date_Detail_cn, 
b.ClassDate, 
b.StartTime, 
b.EndTime, 
b.Active AS DateStatus
FROM            
	dbo.Class AS a 
	INNER JOIN dbo.Class_Detail AS b 
		ON a.Id = b.ClassId AND a.Deleted = 0 AND b.Deleted = 0 AND a.Active=1 AND b.Active=1
	INNER JOIN
		dbo.Course AS c ON a.CourseId = c.Id AND c.Deleted = 0 AND c.Active=1
	INNER JOIN dbo.GSite d ON d.Id = c.SiteId
WHERE
	a.Status = 3 -- Status: Publish