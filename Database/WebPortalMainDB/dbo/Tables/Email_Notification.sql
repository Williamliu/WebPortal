CREATE TABLE [dbo].[Email_Notification] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Category]    NVARCHAR (64)  NULL,
    [RefId]       INT            NULL,
    [Subject]     NVARCHAR (128) NULL,
    [Detail]      NVARCHAR (MAX) NULL,
    [CreatedBy]   INT            CONSTRAINT [DF_Email_Notification_CreatedBy] DEFAULT ((0)) NULL,
    [Sort]        INT            CONSTRAINT [DF_Email_Notification_Sort] DEFAULT ((0)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Email_Notification_Deleted] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Email_Notification_Active] DEFAULT ((1)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Email_Notification_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Email_Notification_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Email_Notification] PRIMARY KEY CLUSTERED ([Id] ASC)
);

