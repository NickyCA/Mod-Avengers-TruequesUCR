CREATE PROCEDURE insertarEnHistorialDeBusqueda
	(@criterio nvarchar(255),
	@Tipo nvarchar(50),
	--@IdBusqueda int,
	@FechaYHora date,
	@EmailComerciante nvarchar(255))
AS
	BEGIN
		INSERT INTO HistorialBusquedas(/*IdBusqueda,*/ FechaYHora, EmailComerciante, Tipo, Descripcion)
		VALUES (/*@IdBusqueda,*/ @FechaYHora, @EmailComerciante, @Tipo, @criterio);
	END;

