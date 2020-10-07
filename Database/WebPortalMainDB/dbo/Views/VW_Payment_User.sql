

/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[VW_Payment_User]
AS
SELECT 
	  ROW_NUMBER() OVER(ORDER BY PaidDate) as Id
      ,[SiteId]
      ,[SiteName_en]
      ,[SiteName_cn]
      ,[UserId]
	  ,FullName
	  ,Email
      ,[Notes]
      ,[Payer]
      ,[PaidDate]
      ,[PaidAmount]
      ,[Currency]
      ,1 as [PaidMethod]
      ,[PaidInvoice]
      ,[PaidCard]
      ,[PaidStatus]
      ,[IsSuccess]
      ,[TrackNumber]
FROM VW_Payment_Class
UNION
SELECT 
	  ROW_NUMBER() OVER(ORDER BY PaidDate) as Id
      ,[SiteId]
      ,[SiteName_en]
      ,[SiteName_cn]
      ,[UserId]
	  ,FullName
	  ,Email
      ,[Notes]
      ,[Payer]
      ,[PaidDate]
      ,[PaidAmount]
      ,[Currency]
      ,2 as [PaidMethod]
      ,[PaidInvoice]
      ,[PaidCard]
      ,[PaidStatus]
      ,[IsSuccess]
      ,[TrackNumber]
  FROM VW_Payment_Donation