CREATE TABLE [dbo].[Course] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Detail_en]   NVARCHAR (MAX) NULL,
    [Detail_cn]   NVARCHAR (MAX) NULL,
    [BranchId]    INT            CONSTRAINT [DF_Course_Category_BranchId] DEFAULT ((0)) NULL,
    [SiteId]      INT            CONSTRAINT [DF_Course_Category_SiteId] DEFAULT ((0)) NULL,
    [AgreeId]     INT            CONSTRAINT [DF_Course_AgreeId] DEFAULT ((0)) NULL,
    [Category]    VARCHAR (16)   NULL,
    [IsFree]      BIT            NULL,
    [FeeAmount]   MONEY          NULL,
    [Currency]    INT            NULL,
    [Sort]        INT            CONSTRAINT [DF_Course_Category_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Course_Category_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Course_Category_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Course_Category_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Course_Category_LastUpdated] DEFAULT ((0)) NULL
);

