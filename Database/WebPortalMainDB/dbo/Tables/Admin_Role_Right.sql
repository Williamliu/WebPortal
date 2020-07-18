CREATE TABLE [dbo].[Admin_Role_Right] (
    [RoleId]  INT NOT NULL,
    [MenuId]  INT NOT NULL,
    [RightId] INT NOT NULL,
    CONSTRAINT [PK_Admin_Role_Right] PRIMARY KEY CLUSTERED ([RoleId] ASC, [MenuId] ASC, [RightId] ASC)
);

