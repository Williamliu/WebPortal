CREATE TABLE [dbo].[GTranslation] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Filter]      VARCHAR (32)   NULL,
    [Keyword]     VARCHAR (64)   NULL,
    [Word_en]     NVARCHAR (MAX) NULL,
    [Word_cn]     NVARCHAR (MAX) NULL,
    [Active]      BIT            CONSTRAINT [DF_Web_Translation_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Web_Translation_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Web_Translation_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Web_Translation_LastUpdated] DEFAULT ((0)) NULL
);

