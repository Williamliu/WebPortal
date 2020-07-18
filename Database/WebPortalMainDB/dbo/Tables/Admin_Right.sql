CREATE TABLE [dbo].[Admin_Right] (
    [Id]        INT           NOT NULL,
    [Action]    VARCHAR (16)  NULL,
    [Title_en]  NVARCHAR (32) NULL,
    [Title_cn]  NVARCHAR (32) NULL,
    [Detail_en] NVARCHAR (64) NULL,
    [Detail_cn] NVARCHAR (64) NULL,
    [Sort]      INT           CONSTRAINT [DF_Admin_Right_Sort] DEFAULT ((0)) NULL,
    [Active]    BIT           CONSTRAINT [DF_Admin_Right_Status] DEFAULT ((1)) NULL,
    [Deleted]   BIT           CONSTRAINT [DF_Admin_Right_Deleted] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Admin_Right] PRIMARY KEY CLUSTERED ([Id] ASC)
);

