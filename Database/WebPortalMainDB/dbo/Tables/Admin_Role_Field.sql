CREATE TABLE [dbo].[Admin_Role_Field] (
    [RoleId] INT           NOT NULL,
    [MenuId] INT           NOT NULL,
    [Fields] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Admin_Role_Field] PRIMARY KEY CLUSTERED ([RoleId] ASC, [MenuId] ASC)
);

