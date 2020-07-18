CREATE TABLE [dbo].[Course_Agreement] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Agreement]   NVARCHAR (MAX) NULL,
    [Sort]        INT            CONSTRAINT [DF_Course_Agreement_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Course_Agreement_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Course_Agreement_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Course_Agreement_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Course_Agreement_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Course_Agreement] PRIMARY KEY CLUSTERED ([Id] ASC)
);

