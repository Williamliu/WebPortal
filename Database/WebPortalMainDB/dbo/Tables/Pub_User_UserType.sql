CREATE TABLE [dbo].[Pub_User_UserType] (
    [UserId]     INT NOT NULL,
    [UserTypeId] INT NOT NULL,
    CONSTRAINT [PK_Pub_User_UserType] PRIMARY KEY CLUSTERED ([UserId] ASC, [UserTypeId] ASC)
);

