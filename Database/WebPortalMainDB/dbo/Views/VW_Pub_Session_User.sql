
Create VIEW [dbo].[VW_Pub_Session_User]
AS
SELECT        
	a.Id, 
	a.FirstName, 
	a.LastName, 
	a.UserName, 
	a.Email, 
	a.Phone, 
	a.BranchId, 
	b.Session
FROM            
	dbo.Pub_User AS a 
	INNER JOIN dbo.Pub_User_Session AS b ON a.Id = b.PubUserId
WHERE  a.Deleted = 0 AND b.Deleted = 0 AND a.Active = 1

