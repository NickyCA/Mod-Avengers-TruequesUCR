CREATE TABLE [dbo].[Calificacion]
(
	[Calificacion_Comerciante] FLOAT NOT NULL  DEFAULT 0, 
    [Email_Comerciante] NVARCHAR(255) NOT NULL, 
    PRIMARY KEY ([Calificacion_Comerciante],[Email_Comerciante]),
    CONSTRAINT [FK_dbo.Calificacion_Comerciante] FOREIGN KEY ([Email_Comerciante])
		REFERENCES [dbo].Comerciante ([Email])
)

