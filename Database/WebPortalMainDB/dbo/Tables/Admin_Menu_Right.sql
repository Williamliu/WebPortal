CREATE TABLE [dbo].[Admin_Menu_Right] (
    [MenuId]  INT NOT NULL,
    [RightId] INT NOT NULL,
    CONSTRAINT [PK_Admin_Menu_Right] PRIMARY KEY CLUSTERED ([MenuId] ASC, [RightId] ASC)
);

