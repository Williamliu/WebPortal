CREATE TABLE [dbo].[Admin_Role] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Title_cn]    NVARCHAR (64) NULL,
    [Title_en]    NVARCHAR (64) NULL,
    [Detail_cn]   NVARCHAR (64) NULL,
    [Detail_en]   NVARCHAR (64) NULL,
    [Sort]        INT           CONSTRAINT [DF_Admin_Role_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT           CONSTRAINT [DF_Admin_Role_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT           CONSTRAINT [DF_Admin_Role_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT        CONSTRAINT [DF_Admin_Role_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT        CONSTRAINT [DF_Admin_Role_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Admin_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

