CREATE TABLE [dbo].[Pub_Menu] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ParentId]    INT            NULL,
    [MenuId]      VARCHAR (16)   NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Url]         VARCHAR (1024) NULL,
    [Position]    TINYINT        CONSTRAINT [DF_Pub_Menu_Position] DEFAULT ((0)) NULL,
    [Indent]      VARCHAR (8)    NULL,
    [MenuImage]   VARCHAR (MAX)  NULL,
    [Sort]        INT            NULL,
    [Active]      BIT            NULL,
    [Deleted]     BIT            NULL,
    [CreatedTime] BIGINT         NULL,
    [LastUpdated] BIGINT         NULL,
    CONSTRAINT [PK_Pub_Menu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

