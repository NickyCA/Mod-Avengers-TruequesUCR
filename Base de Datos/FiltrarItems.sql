CREATE PROCEDURE FiltrarItems (
	@categoria AS NVARCHAR (50),
	@estado AS NVARCHAR (50),
	@email AS NVARCHAR (255)
)
AS
BEGIN
	SET IMPLICIT_TRANSACTIONS OFF
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	BEGIN TRY
		BEGIN TRANSACTION t1
			IF (@categoria is not NULL or @estado is not NULL)
			BEGIN
				IF (@categoria is not NULL and @estado is not NULL)
				BEGIN
					SELECT *
					FROM Item
					WHERE Item.Email_Comerciante = @email and Item.Nombre_Categoria_Item = @categoria and Item.Nombre_Estado_Item = @estado and Item.Eliminado = 0
				END
				ELSE IF (@categoria is not NULL and @estado is NULL)
				BEGIN
					SELECT *
					FROM Item
					WHERE Item.Email_Comerciante = @email and Item.Nombre_Categoria_Item = @categoria and Item.Eliminado = 0
				END
				ELSE IF (@categoria is NULL and @estado is not NULL)
				BEGIN
					SELECT *
					FROM Item
					WHERE Item.Email_Comerciante = @email and Item.Nombre_Estado_Item = @estado and Item.Eliminado = 0
				END
			END
			ELSE
			BEGIN
				SELECT *
				FROM Item
				WHERE Item.Email_Comerciante = @email and Item.Eliminado = 0
			END
		COMMIT TRANSACTION t1
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE() AS ErrorMeesage
		ROLLBACK TRANSACTION t1
	END CATCH
END;

-- En este procedimiento con el nivel de aislamiento read commited es suficiente para realizar su funcionalidad
-- Ya que, lo que necesitamos leer son los datos actuales que tenga en el inventario el usuario
-- Además, como este procedimiento responde a solo un usuario, no es posible que mientras filtre sus items, agregue o elimine un item
-- Por lo tanto, este nivel de aislamiento es adecuado para este procedimiento.