﻿CREATE TABLE [dbo].[GSite] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [BranchId]    INT            CONSTRAINT [DF_GSite_BranchId] DEFAULT ((0)) NULL,
    [Title_cn]    NVARCHAR (64)  NULL,
    [Title_en]    NVARCHAR (64)  NULL,
    [Detail_cn]   NVARCHAR (256) NULL,
    [Detail_en]   NVARCHAR (256) NULL,
    [Email_en]    VARCHAR (256)  NULL,
    [Email_cn]    VARCHAR (256)  NULL,
    [Phone_en]    VARCHAR (32)   NULL,
    [Phone_cn]    VARCHAR (32)   NULL,
    [Timezone]    TINYINT        CONSTRAINT [DF_Site_Timezone] DEFAULT ((0)) NULL,
    [Address]     NVARCHAR (256) NULL,
    [City]        NVARCHAR (64)  NULL,
    [State]       INT            CONSTRAINT [DF_GSite_State] DEFAULT ((0)) NULL,
    [Country]     INT            CONSTRAINT [DF_GSite_Country] DEFAULT ((0)) NULL,
    [Postal]      VARCHAR (16)   NULL,
    [FoundDate]   DATE           NULL,
    [Sort]        INT            CONSTRAINT [DF_GSite_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT            CONSTRAINT [DF_Site_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Site_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT         CONSTRAINT [DF_Site_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT         CONSTRAINT [DF_Site_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([Id] ASC)
);
