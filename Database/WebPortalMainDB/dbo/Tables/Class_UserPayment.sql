CREATE TABLE [dbo].[Class_UserPayment] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [ClassId]      INT           NULL,
    [UserId]       INT           NULL,
    [Payer]        VARCHAR (128) NULL,
    [PaidDate]     BIGINT        NULL,
    [PaidAmount]   MONEY         NULL,
    [Currency]     VARCHAR (12)  NULL,
    [PaidMethod]   VARCHAR (64)  NULL,
    [PaidInvoice]  VARCHAR (64)  NULL,
    [PaidCard]     VARCHAR (64)  NULL,
    [PaidStatus]   VARCHAR (64)  NULL,
    [IsSuccess]    BIT           NULL,
    [TrackNumber]  VARCHAR (256) NULL,
    [TrackMessage] VARCHAR (MAX) NULL,
    [CreatedTime]  BIGINT        NULL,
    CONSTRAINT [PK_Class_Payment] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO

GO
CREATE TRIGGER [dbo].[TRG_Class_UserPayment]
ON [dbo].[Class_UserPayment]
After Insert, Update
AS
BEGIN

	IF EXISTS( 
			SELECT Class_Enroll.ClassId, Class_Enroll.UserId 
					FROM Class_Enroll 
					INNER JOIN Inserted 
						On Inserted.ClassId = Class_Enroll.ClassId AND Inserted.UserId=Class_Enroll.UserId 
						AND Class_Enroll.Deleted=0 AND Class_Enroll.Active=1
	)
	BEGIN
		UPDATE Class_Enroll
		 SET 
			PaidDate = Inserted.PaidDate, 
			PaidMethod = Inserted.PaidMethod, 
			PaidInvoice = Inserted.PaidInvoice, 
			PaidCard = Inserted.PaidCard,
			IsPaid = 1
		FROM Class_Enroll
		INNER JOIN Inserted 
							On Inserted.ClassId = Class_Enroll.ClassId AND Inserted.UserId=Class_Enroll.UserId 
							AND Inserted.IsSuccess=1
							AND Class_Enroll.Deleted=0 AND Class_Enroll.Active=1 

		UPDATE Class_Enroll
		 SET 
			PaidAmount = payment.PaidAmount,
			IsPaid = 1
		FROM Class_Enroll
		INNER JOIN Inserted 
							On Inserted.ClassId = Class_Enroll.ClassId AND Inserted.UserId=Class_Enroll.UserId 
							AND Class_Enroll.Deleted=0 AND Class_Enroll.Active=1
		CROSS APPLY (
			SELECT Sum(PaidAmount) as PaidAmount
			FROM Class_UserPayment 
			WHERE ClassId = Inserted.ClassId AND UserId=Inserted.UserId AND IsSuccess=1
		) payment
	END

	IF NOT EXISTS( 
			SELECT Class_Enroll.ClassId, Class_Enroll.UserId 
					FROM Class_Enroll 
					INNER JOIN Inserted 
						On Inserted.ClassId = Class_Enroll.ClassId AND Inserted.UserId=Class_Enroll.UserId 
						AND Class_Enroll.Deleted=0 AND Class_Enroll.Active=1
	)
	INSERT Class_Enroll(ClassId, UserId, IsPaid, PaidDate, PaidAmount, PaidMethod, PaidInvoice, PaidCard, Deleted, Active)
	SELECT ClassId, UserId, IsSuccess, PaidDate, PaidAmount, PaidMethod, PaidInvoice, PaidCard, 0, 1
	FROM Inserted 

END