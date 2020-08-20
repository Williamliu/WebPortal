








CREATE VIEW [dbo].[VW_Class_Enroll_Member]
AS
SELECT  
	Class_Enroll.Id,
	Class_Enroll.ClassId,
	Class_Enroll.UserId,
	Pub_User.BranchId,
	Pub_User.MemberId,
	MemberType,
	FirstName,
	LastName,
	UserName,
	FirstNameLegal,
	LastNameLegal,
	DharmaName,
	DisplayName,
	AliasName,
	CertificateName,
	Gender,
	Email,
	Phone,
	Cell,
	City,
	Grp,
	IsPaid,
	PaidDate,
	PaidAmount,
	PaidMethod,
	PaidInvoice,
	Class_Enroll.Active,
	Class_Enroll.Deleted,
	Class_Enroll.CreatedTime,
	Class_Enroll.LastUpdated
	FROM Class_Enroll 
INNER JOIN Class ON Class.Id = Class_Enroll.ClassId
INNER JOIN Pub_User on Pub_User.Id = Class_Enroll.UserId
WHERE 
	Class_Enroll.Deleted = 0 
