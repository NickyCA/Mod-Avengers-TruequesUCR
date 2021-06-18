CREATE TRIGGER [insertBusqueda]
ON [dbo].[HistorialBusquedas]
INSTEAD OF INSERT
AS
BEGIN
	DECLARE 
		@FechaYHora as DATE,
		@EmailComerciante as NVARCHAR(255),
		@Tipo as NVARCHAR(255),
		@Descripcion as NVARCHAR(50)

	SELECT @FechaYHora = FechaYHora,
		@EmailComerciante = EmailComerciante,
		@Tipo = Tipo,
		@Descripcion = Descripcion
	FROM inserted

	if NOT EXISTS(
		SELECT * 
		FROM HistorialBusquedas
		WHERE Descripcion = @Descripcion AND EmailComerciante = @EmailComerciante
	)
		BEGIN
		INSERT INTO HistorialBusquedas(FechaYHora, EmailComerciante, Tipo, Descripcion, CantAparicion)
		VALUES ( @FechaYHora, @EmailComerciante, @Tipo, @Descripcion, 1);
		END
	ELSE BEGIN
		UPDATE HistorialBusquedas
		SET FechaYHora = @FechaYHora, CantAparicion = CantAparicion + 1
		WHERE Descripcion = @Descripcion AND EmailComerciante = @EmailComerciante
		END

END
