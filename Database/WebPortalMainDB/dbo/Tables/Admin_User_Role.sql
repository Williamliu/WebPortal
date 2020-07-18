CREATE TABLE [dbo].[Admin_User_Role] (
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_Admin_User_Role] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC)
);

