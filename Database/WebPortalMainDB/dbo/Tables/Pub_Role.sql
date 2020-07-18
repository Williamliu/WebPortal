CREATE TABLE [dbo].[Pub_Role] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Title_en]    NVARCHAR (64) NULL,
    [Title_cn]    NVARCHAR (64) NULL,
    [Detail_en]   NVARCHAR (64) NULL,
    [Detail_cn]   NVARCHAR (64) NULL,
    [Sort]        INT           NULL,
    [Active]      BIT           NULL,
    [Deleted]     BIT           NULL,
    [CreatedTime] BIGINT        NULL,
    [LastUpdated] BIGINT        NULL,
    CONSTRAINT [PK_Pub_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

