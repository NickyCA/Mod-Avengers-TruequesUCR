GO
CREATE PROCEDURE GetBusquedaPorItem (
	@pais AS NVARCHAR (150)
)
AS
BEGIN
	IF (@pais = '')
	BEGIN
		DECLARE @DSQL nvarchar (max) = 'SELECT TOP (10) Descripcion, CantAparicion
		FROM HistorialBusquedas  join Perfil on HistorialBusquedas.EmailComerciante = Perfil.ComercianteEmail
		GROUP BY Descripcion,CantAparicion, Perfil.Pais, perfil.Provincia_Estado, perfil.Canton_Ciudad
		ORDER BY CantAparicion DESC'
		EXEC sp_executesql @DSQL, N'@pais nvarchar(1000)', @pais = @pais;

	END
	ELSE
	BEGIN
		DECLARE @SQL nvarchar (max) = 'SELECT TOP (10) Descripcion, CantAparicion
		FROM HistorialBusquedas  join Perfil on HistorialBusquedas.EmailComerciante = Perfil.ComercianteEmail
		GROUP BY Descripcion,CantAparicion, Perfil.Pais, perfil.Provincia_Estado, perfil.Canton_Ciudad
		HAVING Perfil.Pais IN (SELECT * FROM dbo.splitstring(@pais)) OR perfil.Provincia_Estado IN (SELECT * FROM dbo.splitstring(@pais)) OR  perfil.Canton_Ciudad IN (SELECT * FROM dbo.splitstring(@pais))
		ORDER BY CantAparicion DESC'
		EXEC sp_executesql @SQL, N'@pais nvarchar(1000)', @pais = @pais;
	END
END;