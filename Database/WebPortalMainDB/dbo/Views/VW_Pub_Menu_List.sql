
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[VW_Pub_Menu_List]
AS
SELECT Id as Grp, 
	Id,
	MenuId,
	Title_cn,
	Title_en,
	Detail_cn,
	Detail_en,
	Sort,
	Active,
	Deleted
	FROM Pub_Menu 
	WHERE Deleted=0 AND Active =1 AND ParentId = 0 
UNION 
SELECT s.ParentId as Grp, 
	s.Id,
	s.MenuId,
	s.Title_cn,
	s.Title_en,
	s.Detail_cn,
	s.Detail_en,
	s.Sort,
	s.Active,
	s.Deleted
  FROM Pub_Menu p
  INNER JOIN Pub_Menu s 
  ON s.ParentId = p.Id AND p.ParentId = 0 AND p.Deleted=0 AND p.Active=1 AND s.Deleted=0 AND s.Active = 1 

  
