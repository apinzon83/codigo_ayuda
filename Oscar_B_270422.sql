SELECT 	cd.id, 
				MM.IdInternoPe Cliente, 
				MM.nombre _Nombre, 
				MM.apellidoPaterno A_Paterno, 
				MM.apellidoMaterno A_Materno, 
				mm.curp _CURP, 
				mm.fechahora _Fecha, 
				cp.fechaCreacion _FCrea,
		 		MM.clabe _CLABE, 
		 		MM.num_sec_ac_vista _Cta, 
		 		CP.correo e_mail, 
		 		CP.pin _PIN, 
		 		cd.documento _Doc
  FROM	BsfAcnSucursales.dbo.BSFDocumentoOperacion DO,
				BsfAcnSucursales.dbo.CargaDocumentos CD,
				BsfAcnSucursales.dbo.BsfCuentasMigrantes MM,
				BsfAcnSucursales.dbo.BsfPortalMovilCuentaPerfil CP
  WHERE (MM.num_sec_ac_vista = DO.IdOperacion)
  AND    DO.IdDocumento      = CD.id
  and    MM.idcorreo		 		 = CP.id
	and	 	 MM.num_sec_ac_vista in ('1247784000','','','')
  order by cd.id