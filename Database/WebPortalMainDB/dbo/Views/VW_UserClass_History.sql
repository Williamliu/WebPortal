create VIEW [dbo].[VW_UserClass_History]
AS
SELECT 
	  Class.Id,
	  Class_Enroll.UserId,
	  GSite.Country,
	  GSite.Id as SiteId,
      Class.ClassName,
      Class.Title_en as ClassTitle_en,
      Class.Title_cn as ClassTitle_cn,
	  Class.Notes_en as ClassNotes_en,
	  Class.Notes_cn as ClassNotes_cn,
      Class.StartDate,
      Class.EndDate,
	  GSite.Title_en as SiteTitle_en,
	  GSite.Title_cn as SiteTitle_cn,
	  GSite.Email_en,
	  GSite.Email_cn,
	  GSite.Phone_en,
	  GSite.Phone_cn,
	  Concat(
			GSite.Address,IIF(ISNULL(GSite.Address,'')='','',', '), 
			GSite.City, IIF(ISNULL(GSite.City, '')='','', ', '), 
			GState.Detail_en, IIF(ISNULL(GState.Detail_en, '')='','',', '), 
			GCountry.Detail_en, IIF(ISNULL(GCountry.Detail_en,'')='','',', '),
			GSite.Postal
			) AS Address_en,
	  Concat(
			GSite.Address,IIF(ISNULL(GSite.Address,'')='','',', '), 
			GSite.City, IIF(ISNULL(GSite.City, '')='','', ', '), 
			GState.Detail_cn, IIF(ISNULL(GState.Detail_cn, '')='','',', '), 
			GCountry.Detail_cn, IIF(ISNULL(GCountry.Detail_cn,'')='','',', '),
			GSite.Postal
			) AS Address_cn,
	  ISNULL(Class.IsFree, 0) AS IsFree,
	  ISNULL(Class.FeeAmount,0) AS FeeAmount,
	  ISNULL(GCategoryItem.Detail_en, '') AS Currency
  FROM Class
  INNER JOIN Class_Enroll ON Class_Enroll.ClassId = Class.Id AND Class_Enroll.Deleted=0 AND Class_Enroll.Active=1 
  INNER JOIN Course ON Course.Id = Class.CourseId AND Course.Deleted=0 AND Course.Active=1
  INNER JOIN GSite ON GSite.Id = Course.SiteId AND GSite.Deleted=0 AND GSite.Active=1
  LEFT JOIN GState ON GState.Id = GSite.State
  LEFT JOIN GCountry ON GCountry.Id = GState.CountryId
  LEFT JOIN GCategoryItem ON GCategoryItem.Id = Class.Currency
  WHERE 
	Class.Deleted=0 AND 
	Class.Active=1 AND
	Class.Status >= 5 -- Status: Not Complete