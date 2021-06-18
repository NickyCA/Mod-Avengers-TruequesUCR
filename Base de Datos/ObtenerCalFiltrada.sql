CREATE PROCEDURE [dbo].[ObtenerCalFiltrada](
    @pais NVARCHAR (100)
)
AS
BEGIN

    IF (@pais = '')
    BEGIN
		DECLARE @SQL nvarchar (max) = '
        SELECT c.Calificacion_Comerciante, c.Email_Comerciante
        FROM  Calificacion c join Comerciante co on c.Email_Comerciante = co.Email join Perfil p on co.Email = p.ComercianteEmail'
		EXEC sp_executesql @SQL, N'@pais nvarchar(1000)', @pais = @pais;
    END
    ELSE
    BEGIN
        DECLARE @DSQL nvarchar (max) = 'SELECT c.Calificacion_Comerciante, c.Email_Comerciante
        FROM  Calificacion c join Comerciante co on c.Email_Comerciante = co.Email join Perfil p on co.Email = p.ComercianteEmail
        where p.Pais IN (SELECT * FROM dbo.splitstring(@pais)) OR p.Provincia_Estado IN (SELECT * FROM dbo.splitstring(@pais)) OR  p.Canton_Ciudad IN (SELECT * FROM dbo.splitstring(@pais))'
		EXEC sp_executesql @DSQL, N'@pais nvarchar(1000)', @pais = @pais;
    END
END;	
	