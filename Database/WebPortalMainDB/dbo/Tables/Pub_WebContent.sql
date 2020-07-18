CREATE TABLE [dbo].[Pub_WebContent] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [PubMenuId]   INT            NULL,
    [Place]       VARCHAR (16)   NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Detail_en]   NVARCHAR (MAX) NULL,
    [Detail_cn]   NVARCHAR (MAX) NULL,
    [Sort]        INT            NULL,
    [Active]      BIT            NULL,
    [Deleted]     BIT            NULL,
    [CreatedTime] BIGINT         NULL,
    [LastUpdated] BIGINT         NULL,
    CONSTRAINT [PK_Pub_WebContent] PRIMARY KEY CLUSTERED ([Id] ASC)
);

