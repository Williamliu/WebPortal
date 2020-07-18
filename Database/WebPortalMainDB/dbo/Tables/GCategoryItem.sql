CREATE TABLE [dbo].[GCategoryItem] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]  INT            CONSTRAINT [DF_GCategoryItem_CategoryId] DEFAULT ((0)) NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Sort]        INT            CONSTRAINT [DF_GCategoryItem_Sort] DEFAULT ((0)) NULL,
    [Creator]     INT            CONSTRAINT [DF_GCategoryItem_Creator] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_GCategoryItem_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_GCategoryItem_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_GCategoryItem_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_GCategoryItem_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GCategoryItem] PRIMARY KEY CLUSTERED ([Id] ASC)
);

