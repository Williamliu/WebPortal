CREATE TABLE [dbo].[Admin_User] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (64)  NULL,
    [LastName]    NVARCHAR (64)  NULL,
    [UserName]    NVARCHAR (32)  NULL,
    [Email]       VARCHAR (256)  NULL,
    [Phone]       VARCHAR (32)   NULL,
    [Password]    VARCHAR (256)  NULL,
    [BranchId]    INT            CONSTRAINT [DF_Admin_User_BranchId] DEFAULT ((0)) NULL,
    [IsAdmin]     BIT            CONSTRAINT [DF_Admin_User_IsAdmin] DEFAULT ((0)) NULL,
    [Address]     NVARCHAR (256) NULL,
    [City]        NVARCHAR (64)  NULL,
    [State]       INT            CONSTRAINT [DF_Admin_User_State] DEFAULT ((0)) NULL,
    [Country]     INT            CONSTRAINT [DF_Admin_User_Country] DEFAULT ((0)) NULL,
    [Postal]      VARCHAR (16)   NULL,
    [LoginCount]  INT            CONSTRAINT [DF_Admin_User_LoginCount] DEFAULT ((0)) NULL,
    [LoginTotal]  INT            CONSTRAINT [DF_Admin_User_LoginTotal] DEFAULT ((0)) NULL,
    [LoginTime]   BIGINT         CONSTRAINT [DF_Admin_User_LoginTime] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Admin_User_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            NULL,
    [CreatedTime] BIGINT         NULL,
    [LastUpdated] BIGINT         NULL,
    CONSTRAINT [PK_Admin_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO

CREATE TRIGGER [dbo].[AdminUser_Status_Update] 
   ON  [dbo].[Admin_User]
   AFTER UPDATE
AS 
BEGIN
	IF(UPDATE(Active))
	BEGIN
		UPDATE a SET LoginCount = 0 FROM Admin_User a INNER JOIN inserted b ON (a.Id = b.Id) WHERE b.Active = 1
	END
END
