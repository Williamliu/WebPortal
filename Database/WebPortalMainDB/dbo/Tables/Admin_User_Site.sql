CREATE TABLE [dbo].[Admin_User_Site] (
    [UserId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    CONSTRAINT [PK_Admin_Role_Site] PRIMARY KEY CLUSTERED ([UserId] ASC, [SiteId] ASC)
);

