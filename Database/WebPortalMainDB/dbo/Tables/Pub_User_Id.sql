CREATE TABLE [dbo].[Pub_User_Id] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           CONSTRAINT [DF_Pub_User_Id_UserId] DEFAULT ((0)) NULL,
    [IdType]      INT           CONSTRAINT [DF_Pub_User_Id_IdType] DEFAULT ((0)) NULL,
    [IdNumber]    VARCHAR (128) NULL,
    [Active]      BIT           CONSTRAINT [DF_Pub_User_Id_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT           CONSTRAINT [DF_Pub_User_Id_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT        CONSTRAINT [DF_Pub_User_Id_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT        CONSTRAINT [DF_Pub_User_Id_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Pub_User_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

