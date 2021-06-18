CREATE TABLE [dbo].[Categoria]
(
	[Nombre_Categoria] NVARCHAR(50) NOT NULL PRIMARY KEY,
	[Padre] NVARCHAR(50) NOT NULL,
		CONSTRAINT [FK_dbo.Categoria_dbo.Categoria] FOREIGN KEY ([Padre])
		REFERENCES [dbo].Categoria([Nombre_Categoria])
)
