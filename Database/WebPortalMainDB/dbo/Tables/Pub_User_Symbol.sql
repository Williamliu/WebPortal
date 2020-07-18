CREATE TABLE [dbo].[Pub_User_Symbol] (
    [UserId]   INT NOT NULL,
    [SymbolId] INT NOT NULL,
    CONSTRAINT [PK_Pub_User_Symbol] PRIMARY KEY CLUSTERED ([UserId] ASC, [SymbolId] ASC)
);

