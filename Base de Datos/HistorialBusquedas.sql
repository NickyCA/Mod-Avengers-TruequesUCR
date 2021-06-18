CREATE TABLE [dbo].[HistorialBusquedas]
(
    [IdBusqueda] INT IDENTITY (1, 1) NOT NULL,
    [FechaYHora] DATE NOT NULL,
    [EmailComerciante] NVARCHAR(255) NOT NULL,
    [Tipo] NVARCHAR(50) NULL,
    [Descripcion] NVARCHAR(255) NOT NULL,
    [CantAparicion] INT NULL,
    PRIMARY KEY ([IdBusqueda], [FechaYHora], [EmailComerciante]),
    CONSTRAINT [FK_dbo.HistorialBusquedas_dbo.Item] FOREIGN KEY ([EmailComerciante]) REFERENCES [dbo].Comerciante ([Email])
)