CREATE TABLE [dbo].[Class] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [CourseId]    INT             CONSTRAINT [DF_Class_CourseId] DEFAULT ((0)) NULL,
    [ClassName]   NVARCHAR (32)   NULL,
    [Color]       VARCHAR (16)    NULL,
    [Notes_en]    NVARCHAR (1024) NULL,
    [Notes_cn]    NVARCHAR (1024) NULL,
    [Title_en]    NVARCHAR (64)   NULL,
    [Title_cn]    NVARCHAR (64)   NULL,
    [Detail_en]   NVARCHAR (MAX)  NULL,
    [Detail_cn]   NVARCHAR (MAX)  NULL,
    [StartDate]   DATE            NULL,
    [EndDate]     DATE            NULL,
    [IsFree]      BIT             NULL,
    [FeeAmount]   MONEY           NULL,
    [Currency]    INT             NULL,
    [Status]      INT             NULL,
    [Active]      BIT             CONSTRAINT [DF_Class_Status] DEFAULT ((0)) NULL,
    [Deleted]     BIT             CONSTRAINT [DF_Class_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT          CONSTRAINT [DF_Class_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT          CONSTRAINT [DF_Class_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Id] ASC)
);



