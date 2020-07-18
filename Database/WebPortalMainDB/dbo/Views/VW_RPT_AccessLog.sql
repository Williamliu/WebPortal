
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[VW_RPT_AccessLog]
AS
SELECT 
	a.*, 
	b.Id as PubMenuId,
	ISNULL(b.Title_cn, a.MenuId) AS MenuName_cn, 
	ISNULL(b.Title_en, a.MenuId) AS MenuName_en,
	c.FirstName, c.LastName, c.Email
  FROM Pub_WebAccessLog a
  LEFT JOIN Pub_Menu b ON b.MenuId = a.MenuId 
  LEFT JOIN Pub_User c ON c.Id = a.PubUserId
