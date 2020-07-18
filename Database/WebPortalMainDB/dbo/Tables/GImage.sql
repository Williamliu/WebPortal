CREATE TABLE [dbo].[GImage] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [GalleryId]   INT              CONSTRAINT [DF_GImage_GalleryId] DEFAULT ((0)) NULL,
    [RefKey]      INT              CONSTRAINT [DF_GImage_RefKey] DEFAULT ((0)) NULL,
    [Guid]        UNIQUEIDENTIFIER CONSTRAINT [DF_GImage_Guid] DEFAULT (newid()) NULL,
    [Title_en]    NVARCHAR (64)    NULL,
    [Title_cn]    NVARCHAR (64)    NULL,
    [Detail_en]   NVARCHAR (256)   NULL,
    [Detail_cn]   NVARCHAR (256)   NULL,
    [Full_Name]   NVARCHAR (256)   NULL,
    [Short_Name]  NVARCHAR (256)   NULL,
    [Ext_Name]    VARCHAR (16)     NULL,
    [Mime_Type]   VARCHAR (32)     NULL,
    [Large]       VARCHAR (MAX)    NULL,
    [Medium]      VARCHAR (MAX)    NULL,
    [Small]       VARCHAR (MAX)    NULL,
    [Tiny]        VARCHAR (MAX)    NULL,
    [Main]        BIT              CONSTRAINT [DF_GImage_Main] DEFAULT ((0)) NULL,
    [Sort]        INT              CONSTRAINT [DF_GImage_Sort] DEFAULT ((0)) NULL,
    [Active]      BIT              CONSTRAINT [DF_GImage_Status] DEFAULT ((1)) NULL,
    [Deleted]     BIT              CONSTRAINT [DF_GImage_Deleted] DEFAULT ((0)) NULL,
    [CreatedTime] BIGINT           CONSTRAINT [DF_GImage_CreatedTime] DEFAULT ((0)) NULL,
    [LastUpdated] BIGINT           CONSTRAINT [DF_GImage_LastUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_GImage] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE TRIGGER [dbo].[TR_GImage_Main]
   ON [dbo].[GImage]
   AFTER INSERT ,UPDATE
AS 
BEGIN
	IF UPDATE(Main) 
	DECLARE @main bit, @gid int, @rkey int
	SELECT @gid=GalleryId, @rkey=RefKey, @main=Main FROM inserted
	IF(@main=1)
	UPDATE a SET Main=0 FROM GImage a INNER JOIN inserted b ON a.GalleryId=b.GalleryId AND a.RefKey=b.RefKey AND a.Id<>b.Id
END

GO

CREATE TRIGGER [dbo].[tr_gimage_main_image]
   ON  [dbo].[GImage]
   AFTER INSERT,UPDATE
AS 
BEGIN
	IF(UPDATE(Main)) 
	DECLARE @main bit
	SELECT @main=Main FROM inserted
	IF(@main=1)
	UPDATE a SET Main = 0 FROM GImage a JOIN inserted b ON (a.GalleryId=b.GalleryId AND a.RefKey= b.RefKey AND a.id <> b.id) 
END
