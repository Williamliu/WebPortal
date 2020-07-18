CREATE TABLE [dbo].[GSetting] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [ItemName]    VARCHAR (32)  NULL,
    [ItemTitle]   NVARCHAR (64) NULL,
    [ItemValue]   VARCHAR (64)  NULL,
    [Active]      BIT           CONSTRAINT [DF_GSetting_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT           NULL,
    [CreatedTime] BIGINT        NULL,
    [LastUpdated] BIGINT        CONSTRAINT [DF_GSetting_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GSetting] PRIMARY KEY CLUSTERED ([Id] ASC)
);

