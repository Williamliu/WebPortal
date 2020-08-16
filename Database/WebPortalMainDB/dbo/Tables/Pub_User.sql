CREATE TABLE [dbo].[Pub_User] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [MemberId]          VARCHAR (32)     NULL,
    [FirstName]         NVARCHAR (64)    NULL,
    [LastName]          NVARCHAR (64)    NULL,
    [FirstNameLegal]    NVARCHAR (64)    NULL,
    [LastNameLegal]     NVARCHAR (64)    NULL,
    [DharmaName]        NVARCHAR (64)    NULL,
    [DisplayName]       NVARCHAR (128)   NULL,
    [AliasName]         NVARCHAR (128)   NULL,
    [CertificateName]   NVARCHAR (128)   NULL,
    [UserName]          NVARCHAR (32)    NULL,
    [Password]          VARCHAR (256)    NULL,
    [BranchId]          INT              CONSTRAINT [DF_Pub_User_BranchId] DEFAULT ((0)) NULL,
    [BirthYY]           VARCHAR (4)      NULL,
    [BirthMM]           VARCHAR (2)      NULL,
    [BirthDD]           VARCHAR (2)      NULL,
    [DharmaYY]          VARCHAR (4)      NULL,
    [DharmaMM]          VARCHAR (2)      NULL,
    [DharmaDD]          VARCHAR (2)      NULL,
    [IsMember]          BIT              NULL,
    [MemberExpiry]      DATE             NULL,
    [MemberYY]          VARCHAR (4)      NULL,
    [MemberMM]          VARCHAR (2)      NULL,
    [MemberDD]          VARCHAR (2)      NULL,
    [Gender]            TINYINT          CONSTRAINT [DF_Pub_User_Gender] DEFAULT ((0)) NULL,
    [Education]         INT              CONSTRAINT [DF_Pub_User_Education] DEFAULT ((0)) NULL,
    [MotherLang]        INT              CONSTRAINT [DF_Pub_User_Language] DEFAULT ((0)) NULL,
    [Nationality]       INT              CONSTRAINT [DF_Pub_User_Nationality] DEFAULT ((0)) NULL,
    [Occupation]        NVARCHAR (256)   NULL,
    [Religion]          INT              CONSTRAINT [DF_Pub_User_Religion] DEFAULT ((0)) NULL,
    [IDType]            INT              CONSTRAINT [DF_Pub_User_IDType] DEFAULT ((0)) NULL,
    [IDNumber]          NVARCHAR (32)    NULL,
    [Memo]              NVARCHAR (256)   NULL,
    [Referor]           INT              CONSTRAINT [DF_Pub_User_Referor] DEFAULT ((0)) NULL,
    [Email]             VARCHAR (256)    NULL,
    [Phone]             VARCHAR (32)     NULL,
    [Cell]              VARCHAR (32)     NULL,
    [EmergencyRelation] NVARCHAR (32)    NULL,
    [EmergencyPerson]   NVARCHAR (128)   NULL,
    [EmergencyPhone]    VARCHAR (32)     NULL,
    [EmergencyCell]     VARCHAR (32)     NULL,
    [Address]           NVARCHAR (256)   NULL,
    [City]              NVARCHAR (64)    NULL,
    [State]             INT              CONSTRAINT [DF_Pub_User_State] DEFAULT ((0)) NULL,
    [Country]           INT              CONSTRAINT [DF_Pub_User_Country] DEFAULT ((0)) NULL,
    [Postal]            VARCHAR (16)     NULL,
    [MedicalConcern]    NVARCHAR (1024)  NULL,
    [Symbol_Other]      NVARCHAR (32)    NULL,
    [MultiLang_Other]   NVARCHAR (32)    NULL,
    [HearUs_Other]      NVARCHAR (32)    NULL,
    [LoginCount]        INT              CONSTRAINT [DF_Pub_User_LoginCount] DEFAULT ((0)) NULL,
    [LoginTotal]        INT              CONSTRAINT [DF_Pub_User_LoginTotal] DEFAULT ((0)) NULL,
    [LoginTime]         BIGINT           CONSTRAINT [DF_Pub_User_LoginTime] DEFAULT ((0)) NULL,
    [Active]            BIT              CONSTRAINT [DF_Pub_User_Status] DEFAULT ((1)) NULL,
    [Deleted]           BIT              CONSTRAINT [DF_Pub_User_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime]       BIGINT           CONSTRAINT [DF_Pub_User_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated]       BIGINT           CONSTRAINT [DF_Pub_User_LastUpdate] DEFAULT ((0)) NULL,
    [guid]              UNIQUEIDENTIFIER CONSTRAINT [DF_Pub_User_guid] DEFAULT (newid()) NULL,
    [Expiry]            BIGINT           NULL,
    CONSTRAINT [PK_Pub_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_Pub_User] UNIQUE NONCLUSTERED ([MemberId] ASC)
);






GO
CREATE TRIGGER [dbo].[Pub_User_AutoFill_Names]
   ON  [dbo].[Pub_User] 
   AFTER INSERT
AS 
BEGIN
    IF(UPDATE(DisplayName)) print 'Display Name'
    IF(UPDATE(CertificateName)) print 'Certificate Name'

	IF(NOT UPDATE(DisplayName))
	UPDATE a Set DisplayName = Concat(b.FirstName,' ', b.LastName)  FROM Pub_User a INNER JOIN Inserted b on (a.Id = b.Id) 
END

GO
CREATE trigger [dbo].[tr_pubuser_deleted_update]
ON [dbo].[Pub_User] After Update
AS 
BEGIN
	IF UPDATE(Deleted)
	UPDATE a SET a.Deleted = b.Deleted FROM Pub_User_Id a INNER JOIN Inserted b ON (a.UserId = b.Id) 
END

GO

CREATE TRIGGER [dbo].[tr_pubuser_name_insert] 
   ON [dbo].[Pub_User]
   AFTER INSERT
AS 
BEGIN
	DECLARE @first nvarchar(64), @last nvarchar(64), @firstleg nvarchar(64), @lastleg nvarchar(64)
	DECLARE @display nvarchar(128), @cert nvarchar(128)
	DECLARE @insertId int, @defaultRole int
	SELECT 
	@insertId = Id,
	@first = FirstName, 
	@last = LastName,
	@firstleg = FirstNameLegal,
	@lastleg = LastNameLegal,
	@display = DisplayName,
	@cert = CertificateName
	FROM inserted

	IF @cert='' OR @cert is null
	BEGIN
		IF UNICODE(@firstleg)=63 OR UNICODE(@lastleg)=63 OR UNICODE(@firstleg) > 127 OR  UNICODE(@lastleg) > 127
			SET @cert=LTRIM(RTRIM(@lastleg)) + LTRIM(RTRIM(@firstleg));
		ELSE
			SET @cert=LTRIM(RTRIM(@firstleg)) + ' ' + LTRIM(RTRIM(@lastleg));

		UPDATE a SET CertificateName = @cert FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END

	if (@firstleg='' OR @firstleg is null) AND (@lastleg='' OR @lastleg is null) AND (@cert='' OR @cert is null) 
	BEGIN
		IF UNICODE(@first)=63 OR UNICODE(@last)=63 OR UNICODE(@first) > 127 OR  UNICODE(@last) > 127
			SET @cert=LTRIM(RTRIM(@last)) + LTRIM(RTRIM(@first));
		ELSE
			SET @cert=LTRIM(RTRIM(@first)) +  ' ' + LTRIM(RTRIM(@last));

		UPDATE a SET CertificateName = @cert FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END

	if @display='' OR @display is null
	BEGIN
		print CONCAT('First : ' , UNICODE(@first));
		print CONCAT('Last : ' , UNICODE(@last));

		IF UNICODE(@first)=63 OR UNICODE(@last)=63 OR UNICODE(@first) > 127 OR  UNICODE(@last) > 127
			SET @display=LTRIM(RTRIM(@last)) + LTRIM(RTRIM(@first));
		ELSE
			SET @display=LTRIM(RTRIM(@first)) + ' ' + LTRIM(RTRIM(@last));

		UPDATE a SET DisplayName = @display FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END

	UPDATE a SET MemberId = 
				CONCAT( 
				IIF(b.BranchId is null OR b.BranchId='', 0, b.BranchId),
				'-',
				RIGHT(DATEPART(YYYY, GETDATE()), 2), 
				RIGHT(CONCAT('0', DATEPART(MM, GETDATE())),2), 
				RIGHT(CONCAT('0', DATEPART(DD, GETDATE())),2), 
				RIGHT(CONCAT('0', DATEPART(HH, GETDATE())),2),
				'-',
				RIGHT(CONCAT('000' , b.Id), 3)
				)
	FROM Pub_User a INNER JOIN inserted b ON (a.id = b.id) 

	SELECT TOP 1 @defaultRole = Id FROM Pub_Role WHERE Deleted=0 AND Active=1 ORDER BY Sort DESC
	IF(@defaultRole IS NOT NULL) INSERT Pub_User_Role(UserId, RoleId) VALUES(@insertId, @defaultRole) 
END

GO
CREATE TRIGGER [dbo].[tr_pubuser_name_update] 
   ON [dbo].[Pub_User]
   AFTER UPDATE
AS 
BEGIN
	DECLARE @first nvarchar(64), @last nvarchar(64), @firstleg nvarchar(64), @lastleg nvarchar(64)
	DECLARE @display nvarchar(128), @cert nvarchar(128)
	SELECT 
	@first = FirstName, 
	@last = LastName,
	@firstleg = FirstNameLegal,
	@lastleg = LastNameLegal,
	@display = DisplayName,
	@cert = CertificateName
	FROM inserted

	IF NOT UPDATE(CertificateName) AND (UPDATE(FirstNameLegal) OR UPDATE(LastNameLegal))
	BEGIN
		IF UNICODE(@firstleg)=63 OR UNICODE(@lastleg)=63 OR UNICODE(@firstleg) > 127 OR  UNICODE(@lastleg) > 127
			SET @cert=LTRIM(RTRIM(@lastleg)) + LTRIM(RTRIM(@firstleg));
		ELSE
			SET @cert=LTRIM(RTRIM(@firstleg)) + ' ' + LTRIM(RTRIM(@lastleg));

		UPDATE a SET CertificateName = @cert FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END

	if (@firstleg='' OR @firstleg is null) AND (@lastleg='' OR @lastleg is null) AND NOT UPDATE(CertificateName) AND (UPDATE(FirstName) OR UPDATE(LastName))
	BEGIN
		IF UNICODE(@first)=63 OR UNICODE(@last)=63 OR UNICODE(@first) > 127 OR  UNICODE(@last) > 127
			SET @cert=LTRIM(RTRIM(@last)) + LTRIM(RTRIM(@first));
		ELSE
			SET @cert=LTRIM(RTRIM(@first)) +  ' ' + LTRIM(RTRIM(@last));

		UPDATE a SET CertificateName = @cert FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END

	if NOT UPDATE(DisplayName) AND (UPDATE(FirstName) OR UPDATE(LastName))
	BEGIN
		print CONCAT('First : ' , UNICODE(@first));
		print CONCAT('Last : ' , UNICODE(@last));

		IF UNICODE(@first)=63 OR UNICODE(@last)=63 OR UNICODE(@first) > 127 OR  UNICODE(@last) > 127
			SET @display=LTRIM(RTRIM(@last)) + LTRIM(RTRIM(@first));
		ELSE
			SET @display=LTRIM(RTRIM(@first)) + ' ' + LTRIM(RTRIM(@last));

		UPDATE a SET DisplayName = @display FROM Pub_User a INNER JOIN inserted b ON (a.Id = b.Id)
	END
END
