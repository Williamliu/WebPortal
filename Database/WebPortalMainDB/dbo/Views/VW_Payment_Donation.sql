
CREATE VIEW [dbo].[VW_Payment_Donation]
AS
SELECT a.Id
	  ,ISNULL(s.BranchId, ISNULL(p.BranchId, 0)) as BranchId
	  ,ISNULL(a.SiteId,0) as SiteId
	  ,s.Title_en as SiteName_en
	  ,s.Title_cn as SiteName_cn
	  ,ISNULL(a.UserId,0) as UserId
	  ,p.UserName
	  ,Concat(ISNULL(a.FirstName, p.FirstName)  , ' ', ISNULL(a.LastName, p.LastName)) as FullName
	  ,ISNULL(a.Email, p.Email) as Email
      ,Notes
      ,Payer
      ,PaidDate
      ,PaidAmount
      ,Currency
      ,PaidMethod
      ,PaidInvoice
      ,PaidCard
      ,PaidStatus
      ,IsSuccess
      ,TrackNumber
  FROM Donation a
  LEFT JOIN Pub_User p ON p.Id = a.UserId
  LEFT JOIN GSite s ON s.Id = a.SiteId
  LEFT JOIN GBranch b ON  b.Id = s.BranchId
  WHERE IsSuccess = 1