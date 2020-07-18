CREATE TABLE [dbo].[Pub_User_Language] (
    [UserId]     INT NOT NULL,
    [LanguageId] INT NOT NULL,
    CONSTRAINT [PK_Pub_User_Language] PRIMARY KEY CLUSTERED ([UserId] ASC, [LanguageId] ASC)
);

