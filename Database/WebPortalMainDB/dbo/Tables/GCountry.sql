CREATE TABLE [dbo].[GCountry] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Sort]        INT            CONSTRAINT [DF_GCountry_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_GCountry_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_GCountry_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_GCountry_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_GCountry_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GCountry] PRIMARY KEY CLUSTERED ([Id] ASC)
);

