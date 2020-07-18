CREATE TABLE [dbo].[Pub_WebAccessLog] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [MenuId]      VARCHAR (16)  NULL,
    [Url]         VARCHAR (256) NULL,
    [UserAgent]   VARCHAR (256) NULL,
    [PubUserId]   INT           NULL,
    [IPAddress]   VARCHAR (64)  NULL,
    [Lang]        VARCHAR (8)   NULL,
    [UserLang]    VARCHAR (64)  NULL,
    [CreatedTime] DATETIME      CONSTRAINT [DF_Pub_WebAccessLog_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Pub_WebAccessLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

