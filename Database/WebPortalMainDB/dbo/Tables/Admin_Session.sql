CREATE TABLE [dbo].[Admin_Session] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [AdminId]     INT              CONSTRAINT [DF_Admin_Session_AdminId] DEFAULT ((0)) NULL,
    [Session]     UNIQUEIDENTIFIER NULL,
    [Deleted]     BIT              CONSTRAINT [DF_Admin_Session_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT           CONSTRAINT [DF_Admin_Session_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT           CONSTRAINT [DF_Admin_Session_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Admin_Session] PRIMARY KEY CLUSTERED ([Id] ASC)
);

