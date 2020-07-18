  CREATE Procedure USP_RPT_AccessLog_MenuId(@rpt_start datetime, @rpt_end datetime)
  AS
  SET @rpt_start = ISNULL(@rpt_start, '2000-01-01') 
  SET @rpt_end = ISNULL(@rpt_end, '2999-12-31')
  SELECT 
	  ISNULL(Pub_Menu.Title_en, rpt.MenuId) as MenuName_en,
	  ISNULL(Pub_Menu.Title_cn, rpt.MenuId) as MenuName_cn,
	  rpt.ViewCount
  FROM 
  (
	SELECT MenuId, Count(1) AS ViewCount 
		FROM Pub_WebAccessLog
		WHERE 
			CreatedTime between @rpt_start and @rpt_end
		GROUP BY MenuId 
  ) rpt 
  LEFT JOIN Pub_Menu ON Pub_Menu.MenuId = rpt.MenuId
  ORDER BY rpt.ViewCount DESC