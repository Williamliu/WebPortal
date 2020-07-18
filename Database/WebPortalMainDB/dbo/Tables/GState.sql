CREATE TABLE [dbo].[GState] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CountryId]   INT            CONSTRAINT [DF_GState_CountryId] DEFAULT ((0)) NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Sort]        INT            CONSTRAINT [DF_GState_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_GState_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_GState_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_GState_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_GState_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GState] PRIMARY KEY CLUSTERED ([Id] ASC)
);

