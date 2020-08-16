

CREATE VIEW [dbo].[VW_Class_Enroll_User]
AS
SELECT  
	Class_Enroll.Id,
	Course.SiteId,
	GSite.Title_en AS SiteName_en,
	GSite.Title_cn AS SiteName_cn,
	Class_Enroll.ClassId,
	Class.ClassName,
	Class.Title_en AS ClassTitle_en,
	Class.Title_cn AS ClassTitle_cn,
	Class.IsFree,
	Class.FeeAmount,
	c.Title_en AS Currency,
	Class.StartDate,
	Class.EndDate,
	Class_Enroll.UserId,
	CONCAT(Pub_User.FirstName, ' ', Pub_User.LastName) AS FullName,
	Pub_User.Email,
	Class_Enroll.Grp,
	m.Title_en as MemberType_en,
	m.Title_cn as MemberType_cn,
	Class_Enroll.IsPaid,
	Class_Enroll.PaidDate,
	Class_Enroll.PaidAmount,
	Class_Enroll.PaidMethod,
	Class_Enroll.PaidInvoice,
	Class_Enroll.Active,
	Class_Enroll.CreatedTime
FROM Class_Enroll 
INNER JOIN Class ON Class.Id = Class_Enroll.ClassId
INNER JOIN Course ON Course.Id = Class.CourseId
INNER JOIN GSite ON GSite.Id = Course.SiteId
INNER JOIN Pub_User ON Pub_User.Id = Class_Enroll.UserId
LEFT JOIN GCategoryItem c ON c.Id = Class.Currency
LEFT JOIN GCategoryItem m ON m.Id = Class_Enroll.MemberType
WHERE 
	Class_Enroll.Deleted = 0