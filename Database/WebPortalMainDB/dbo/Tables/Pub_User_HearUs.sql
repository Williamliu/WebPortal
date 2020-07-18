CREATE TABLE [dbo].[Pub_User_HearUs] (
    [UserId]   INT NOT NULL,
    [HearUsId] INT NOT NULL,
    CONSTRAINT [PK_Pub_User_HearUs] PRIMARY KEY CLUSTERED ([UserId] ASC, [HearUsId] ASC)
);

