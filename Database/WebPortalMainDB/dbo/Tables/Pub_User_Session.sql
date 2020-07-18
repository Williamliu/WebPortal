CREATE TABLE [dbo].[Pub_User_Session] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [PubUserId]   INT              NULL,
    [Session]     UNIQUEIDENTIFIER NULL,
    [Deleted]     BIT              NULL,
    [CreatedTime] BIGINT           NULL,
    [LastUpdated] BIGINT           NULL,
    CONSTRAINT [PK_Pub_User_Session] PRIMARY KEY CLUSTERED ([Id] ASC)
);

