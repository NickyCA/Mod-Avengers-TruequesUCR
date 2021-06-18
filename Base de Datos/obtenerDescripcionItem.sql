CREATE PROCEDURE [dbo].[obtenerDescripcionItem]
	@idItem int = 0,
	@EmailComerciante nvarchar(255)
AS
BEGIN
	SELECT Descripcion
    FROM Item
    WHERE Id_Item = @idItem AND Email_Comerciante = @EmailComerciante

END; 