CREATE TABLE [dbo].[GBranch] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [FoundDate]   DATE           NULL,
    [FTime]       TIME (7)       NULL,
    [FDate]       DATETIME       NULL,
    [Sort]        INT            CONSTRAINT [DF_GBranch_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Branch_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Branch_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Branch_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Branch_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED ([Id] ASC)
);

