CREATE TABLE [dbo].[Donation] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [SiteId]       INT             NULL,
    [UserId]       INT             NULL,
    [FirstName]    NVARCHAR (64)   NULL,
    [LastName]     NVARCHAR (64)   NULL,
    [Email]        VARCHAR (256)   NULL,
    [Notes]        NVARCHAR (1024) NULL,
    [Payer]        VARCHAR (128)   NULL,
    [PaidDate]     BIGINT          NULL,
    [PaidAmount]   MONEY           NULL,
    [Currency]     VARCHAR (12)    NULL,
    [PaidMethod]   VARCHAR (64)    NULL,
    [PaidInvoice]  VARCHAR (64)    NULL,
    [PaidCard]     VARCHAR (64)    NULL,
    [PaidStatus]   VARCHAR (64)    NULL,
    [IsSuccess]    BIT             NULL,
    [TrackNumber]  VARCHAR (256)   NULL,
    [TrackMessage] VARCHAR (MAX)   NULL,
    [CreatedTime]  BIGINT          NULL,
    CONSTRAINT [PK_Donation] PRIMARY KEY CLUSTERED ([Id] ASC)
);

