CREATE TABLE [dbo].[Email_Notification_User] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Category]    VARCHAR (16) NULL,
    [EmailId]     INT          NOT NULL,
    [UserId]      INT          NOT NULL,
    [CreatedTime] BIGINT       NULL,
    CONSTRAINT [PK_Email_Notification_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

