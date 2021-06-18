CREATE TABLE [dbo].[ComercianteSigueHashtag]
(
	[ComercianteEmail] NVARCHAR(255) NOT NULL, 
    [Hashtag] NVARCHAR(100) NOT NULL, 
    [Privacidad] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_ComercianteSigueHashtag_ToComerciante] FOREIGN KEY (ComercianteEmail) REFERENCES Comerciante(Email), 
    CONSTRAINT [FK_ComercianteSigueHashtag_ToHashtag] FOREIGN KEY (Hashtag) REFERENCES Hashtag(Hashtag),
    CONSTRAINT [PK_ComercianteSigueHashtag] PRIMARY KEY NONCLUSTERED (ComercianteEmail, Hashtag)
)
