GO
CREATE PROCEDURE GetItemPorEtiqueta (
	@pais AS NVARCHAR (150)
)
AS
BEGIN

	IF (@pais = '')
	BEGIN
		DECLARE @SQL nvarchar (max) = 'SELECT TOP (10) Etiquetas.Nombre_Etiqueta,COUNT(Rel_Item_Etiqueta.Etiqueta_Rel) as Cantidad_Etiquetas
		FROM Etiquetas join Rel_Item_Etiqueta on Rel_Item_Etiqueta.Etiqueta_Rel = Etiquetas.Nombre_Etiqueta join Item on Rel_Item_Etiqueta.Email_Item_Rel = Item.Email_Comerciante and Rel_Item_Etiqueta.Id_Item_Rel = Item.Id_Item
					   join Comerciante on Item.Email_Comerciante = Comerciante.Email join Perfil on Comerciante.Email = Perfil.ComercianteEmail
		GROUP BY Etiquetas.Nombre_Etiqueta
		ORDER BY Cantidad_Etiquetas DESC'
		EXEC sp_executesql @SQL, N'@pais nvarchar(1000)', @pais = @pais;
	END
	ELSE
	BEGIN
		DECLARE @DSQL nvarchar(max) = 'SELECT TOP (10) Etiquetas.Nombre_Etiqueta,COUNT(Rel_Item_Etiqueta.Etiqueta_Rel) as Cantidad_Etiquetas
		FROM Etiquetas join Rel_Item_Etiqueta on Rel_Item_Etiqueta.Etiqueta_Rel = Etiquetas.Nombre_Etiqueta join Item on Rel_Item_Etiqueta.Email_Item_Rel = Item.Email_Comerciante and Rel_Item_Etiqueta.Id_Item_Rel = Item.Id_Item
					   join Comerciante on Item.Email_Comerciante = Comerciante.Email join Perfil on Comerciante.Email = Perfil.ComercianteEmail
		WHERE Perfil.Pais IN (SELECT * FROM dbo.splitstring(@pais)) OR perfil.Provincia_Estado IN (SELECT * FROM dbo.splitstring(@pais)) OR  perfil.Canton_Ciudad IN (SELECT * FROM dbo.splitstring(@pais))
		GROUP BY Etiquetas.Nombre_Etiqueta
		ORDER BY Cantidad_Etiquetas DESC'
		EXEC sp_executesql @DSQL, N'@pais nvarchar(1000)', @pais = @pais;

	END
END;