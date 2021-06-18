CREATE TABLE [dbo].[Comerciante_EtiquetaGustos]
(
	 Email_Comerciante nvarchar(255) not null, Etiqueta nvarchar(50) not null,
Constraint PK_Comerciante_EtiquetaGustos
	Primary key (Email_Comerciante, Etiqueta),
Constraint FK_Comerciante_EtiquetaGustos_ToComerciante
	foreign key (Email_Comerciante) references Comerciante(Email) on delete Cascade,
Constraint FK_Comerciante_EtiquetaGustos_ToEtiqueta
	foreign key (Etiqueta) references Etiquetas(Nombre_Etiqueta) on delete Cascade
)
