CREATE TABLE [dbo].[Admin_Menu] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ParentId]    INT            CONSTRAINT [DF_Admin_Menu_ParentId] DEFAULT ((0)) NULL,
    [MenuId]      VARCHAR (16)   NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Url]         VARCHAR (1024) NULL,
    [Position]    TINYINT        CONSTRAINT [DF_Admin_Menu_Position] DEFAULT ((0)) NULL,
    [MenuImage]   VARCHAR (MAX)  NULL,
    [Sort]        INT            CONSTRAINT [DF_Admin_Menu_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Admin_Menu_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Admin_Menu_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Admin_Menu_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Admin_Menu_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Admin_Menu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

