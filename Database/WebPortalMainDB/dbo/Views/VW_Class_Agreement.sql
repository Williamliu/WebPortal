/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW VW_Class_Agreement
AS
SELECT 
	Class.Id, 
	Course_Agreement.Title_en as AgreeTitle_en,
	Course_Agreement.Title_cn as AgreeTitle_cn,
	Agreement
  FROM Class
  INNER JOIN Course 
			ON Course.Id = Class.CourseId AND 
			Class.Deleted=0 AND Class.Active=1 AND
			Course.Deleted=0 AND Course.Active=1 
 INNER JOIN Course_Agreement 
			ON Course_Agreement.Id = Course.AgreeId AND 
			Course_Agreement.Deleted=0 AND Course_Agreement.Active=1