  CREATE VIEW [dbo].[VW_Class_Enroll]
  AS
  SELECT 
	Class.Id as ClassId, 
	Course.BranchId,
	Course.SiteId,
	Class.CourseId, 
	CONCAT(Class.Title_en, ' [', Class.StartDate, '~', Class.EndDate, ']-', GSite.City ) as Title_en, 
	CONCAT(Class.Title_cn, ' [', Class.StartDate, '~', Class.EndDate, ']-', GSite.City ) as Title_cn,
	'' as Detail_en,
	'' as Detail_cn,
	Class.StartDate,
	Class.EndDate,
	Status as ClassStatus,
	Class.Active,
	Class.Deleted
  FROM Class
  INNER JOIN Course ON Class.CourseId = Course.Id
  INNER JOIN GSite  ON GSite.Id = Course.SiteId 
  WHERE 
  Class.Deleted = 0 AND 
  Class.Active = 1 AND  
  Class.Status IN (2,3,4)
