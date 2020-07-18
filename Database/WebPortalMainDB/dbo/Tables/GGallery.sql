CREATE TABLE [dbo].[GGallery] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [GalleryName] VARCHAR (64)  NULL,
    [IsSaveDB]    BIT           CONSTRAINT [DF_GGallery_IsSaveDB] DEFAULT ((1)) NULL,
    [FilePath]    VARCHAR (256) NULL,
    [MaxCount]    INT           CONSTRAINT [DF_GGallery_MaxCount] DEFAULT ((0)) NULL,
    [large_w]     INT           CONSTRAINT [DF_GGallery_large_w] DEFAULT ((0)) NULL,
    [large_h]     INT           CONSTRAINT [DF_GGallery_large_h] DEFAULT ((0)) NULL,
    [medium_w]    INT           CONSTRAINT [DF_GGallery_medium_w] DEFAULT ((0)) NULL,
    [medium_h]    INT           CONSTRAINT [DF_GGallery_medium_h] DEFAULT ((0)) NULL,
    [small_w]     INT           CONSTRAINT [DF_GGallery_small_w] DEFAULT ((0)) NULL,
    [small_h]     INT           CONSTRAINT [DF_GGallery_small_h] DEFAULT ((0)) NULL,
    [tiny_w]      INT           CONSTRAINT [DF_GGallery_tiny_w] DEFAULT ((0)) NULL,
    [tiny_h]      INT           CONSTRAINT [DF_GGallery_tiny_h] DEFAULT ((0)) NULL,
    [Active]      BIT           CONSTRAINT [DF_GGallery_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT           CONSTRAINT [DF_GGallery_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT        CONSTRAINT [DF_GGallery_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT        CONSTRAINT [DF_GGallery_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GGallery] PRIMARY KEY CLUSTERED ([Id] ASC)
);

