CREATE TABLE [dbo].[Class_Enroll] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [ClassId]     INT          NULL,
    [UserId]      INT          NULL,
    [MemberType]  INT          NULL,
    [Grp]         VARCHAR (6)  NULL,
    [IsPaid]      BIT          NULL,
    [PaidDate]    BIGINT       NULL,
    [PaidAmount]  MONEY        NULL,
    [PaidMethod]  INT          NULL,
    [PaidInvoice] VARCHAR (32) NULL,
    [PaidCard]    VARCHAR (32) NULL,
    [Active]      BIT          NULL,
    [Deleted]     BIT          NULL,
    [CreatedTime] BIGINT       NULL,
    [LastUpdated] BIGINT       NULL,
    CONSTRAINT [PK_Class_Enroll] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE TRIGGER [dbo].[trg_class_enroll_update_paid]
   ON  dbo.Class_Enroll
   AFTER UPDATE, INSERT
AS 
BEGIN
	IF UPDATE(IsPaid)  
		UPDATE Class_Enroll
			SET PaidDate = IIF(Inserted.IsPaid=1,CAST(Datediff(SECOND, '1970-01-01',GETUTCDATE()) AS bigint), NULL)
		FROM Inserted 
		INNER JOIN Class_Enroll ON Inserted.Id = Class_Enroll.Id
END
