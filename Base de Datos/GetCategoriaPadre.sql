GO
CREATE PROCEDURE GetCategoriaPadre (
    @categoria AS NVARCHAR (50)
)
AS
BEGIN
    WITH TablaDummy AS (
    SELECT Nombre_Categoria, Padre, 0 AS Nivel
    FROM Categoria
    WHERE Categoria.Nombre_Categoria = @categoria

    UNION ALL

    SELECT Categoria.Nombre_Categoria, Categoria.Padre, TablaDummy.[Nivel]+1
    FROM TablaDummy INNER JOIN Categoria ON TablaDummy.Padre = Categoria.Nombre_Categoria
    WHERE Categoria.Nombre_Categoria <> Categoria.Padre
    )

    SELECT TablaDummy.Padre
    FROM TablaDummy
    ORDER BY TablaDummy.Nivel DESC
END;