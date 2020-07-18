CREATE TABLE [dbo].[Admin_User_Branch] (
    [UserId]   INT NOT NULL,
    [BranchId] INT NOT NULL,
    CONSTRAINT [PK_Admin_Role_Branch] PRIMARY KEY CLUSTERED ([UserId] ASC, [BranchId] ASC)
);

