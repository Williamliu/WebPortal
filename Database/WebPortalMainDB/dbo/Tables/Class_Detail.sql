CREATE TABLE [dbo].[Class_Detail] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [ClassId]     INT             CONSTRAINT [DF_Class_Detail_ClassId] DEFAULT ((0)) NULL,
    [Title_en]    NVARCHAR (64)   NULL,
    [Title_cn]    NVARCHAR (64)   NULL,
    [Detail_en]   NVARCHAR (1024) NULL,
    [Detail_cn]   NVARCHAR (1024) NULL,
    [ClassDate]   DATE            NULL,
    [StartTime]   TIME (7)        NULL,
    [EndTime]     TIME (7)        NULL,
    [Active]      BIT             CONSTRAINT [DF_Class_Detail_Status] DEFAULT ((0)) NULL,
    [Deleted]     BIT             CONSTRAINT [DF_Class_Detail_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT          CONSTRAINT [DF_Class_Detail_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT          CONSTRAINT [DF_Class_Detail_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Class_Detail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

