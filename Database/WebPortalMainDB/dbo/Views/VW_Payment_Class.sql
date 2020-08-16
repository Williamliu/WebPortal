
CREATE VIEW [dbo].[VW_Payment_Class]
AS
SELECT a.Id
	  ,c.BranchId
	  ,c.SiteId
	  ,s.Title_en as SiteName_en
	  ,s.Title_cn as SiteName_cn
      ,[UserId]
	  ,p.UserName
	  ,Concat(p.FirstName, ' ', P.LastName) as FullName
	  ,p.Email
	  ,'' as Notes
      ,[Payer]
      ,[PaidDate]
      ,[PaidAmount]
      ,a.Currency
      ,[PaidMethod]
      ,[PaidInvoice]
      ,[PaidCard]
      ,[PaidStatus]
      ,[IsSuccess]
      ,[TrackNumber]
      ,[ClassId]
	  ,b.ClassName
	  ,b.StartDate
	  ,b.EndDate
	  ,b.Title_en as ClassTitle_en
	  ,b.Title_cn as ClassTitle_cn
  FROM Class_UserPayment a
  INNER JOIN Class b ON b.Id = a.ClassId
  INNER JOIN Course c ON c.id = b.CourseId
  INNER JOIN Pub_User p ON p.Id = a.UserId
  INNER JOIN GSite s ON s.Id = c.SiteId
  WHERE IsSuccess = 1