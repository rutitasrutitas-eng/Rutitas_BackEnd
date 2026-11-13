# Matriz de trazabilidad y brechas — E01–E52 / N01–N20

> **Actualización técnica 13/11/2026:** la rama de Mark implementa únicamente las siete operaciones de identidad, perfil propio y lectura de rutas documentadas en [OpenAPI](../001-unit2-api/contracts/openapi.yaml). La matriz E01–E52/N01–N20 siguiente conserva el cotejo documental histórico y no afirma que viajes, reservas u otras capacidades estén implementadas. Véanse [validación](../../docs/VALIDACION_MARK.md) y [compatibilidad Android](../../docs/COMPATIBILIDAD_ANDROID.md).

**Fecha de corte:** 11/11/2026. **Estado:** revisión documental, no aprobada.
**Fuentes:** `002-catalogo-funcional/inventario-funcional.md` y `spec.md` de los módulos 003–010 entregados en las fases anteriores.
**Regla:** la aparición de un identificador en un `spec.md` NO acredita un contrato completo ni un endpoint implementado; aquí se distingue mención, especificación y ejecución.

| ID | Funcionalidad del inventario | Documento responsable propuesto | Resultado de cotejo |
|---|---|---|---|
| E01 | Verificar correo | `003-identidad-perfil` | Mencionado en spec; comprobar criterios/contrato |
| E02 | Consultar mi perfil | `003-identidad-perfil` | Equivalencia funcional con API implementada; ruta por alinear |
| E03 | Editar mi perfil | `003-identidad-perfil` | Equivalencia funcional con API implementada; ruta por alinear |
| E04 | Eliminar mi cuenta | `003-identidad-perfil` | Mencionado en spec; comprobar criterios/contrato |
| E05 | Listar comunidades | `004-comunidades-verificacion` | Mencionado en spec; comprobar criterios/contrato |
| E06 | Vincular comunidad | `004-comunidades-verificacion` | Mencionado en spec; comprobar criterios/contrato |
| E07 | Mis membresías | `004-comunidades-verificacion` | Mencionado en spec; comprobar criterios/contrato |
| E08 | Solicitar ser conductor | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E09 | Consultar mi solicitud | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E10 | Preparar documento | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E11 | Registrar vehículo | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E12 | Mis vehículos | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E13 | Editar vehículo | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E14 | Desactivar vehículo | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E15 | Autocompletar lugares | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E16 | Buscar rutas | `006-rutas-viajes` | Equivalencia funcional con API implementada; ruta por alinear |
| E17 | Detalle de ruta | `006-rutas-viajes` | Equivalencia funcional con API implementada; ruta por alinear |
| E18 | Publicar ruta | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E19 | Editar ruta | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E20 | Cancelar ruta | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E21 | Mis rutas publicadas | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E22 | Crear recurrencia | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E23 | Solicitar asiento | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E24 | Mis solicitudes | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E25 | Solicitudes de mi ruta | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E26 | Aceptar solicitud | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E27 | Rechazar solicitud | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E28 | Cancelar reserva | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E29 | Mis reservas | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| E30 | Lista de espera | `007-solicitudes-reservas` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E31 | Iniciar viaje | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E32 | Finalizar viaje | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E33 | Historial de viajes | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| E34 | Enviar mensaje | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E35 | Leer chat de viaje | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E36 | Notificaciones | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E37 | Marcar aviso leído | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E38 | Guardar favorito | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E39 | Mis favoritos | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E40 | Quitar favorito | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E41 | Valorar viaje | `009-comunicacion-valoraciones` | Mencionado en spec; comprobar criterios/contrato |
| E42 | Reportar incidente | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| E43 | Mis reportes | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| E44 | Bloquear usuario | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| E45 | Quitar bloqueo | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| E46 | Contactos de confianza | `010-soporte-privacidad-administracion` | Mencionado en spec; comprobar criterios/contrato |
| E47 | Crear ticket soporte | `010-soporte-privacidad-administracion` | Mencionado en spec; comprobar criterios/contrato |
| E48 | Mis tickets | `010-soporte-privacidad-administracion` | Mencionado en spec; comprobar criterios/contrato |
| E49 | Admin comunidades | `004-comunidades-verificacion` | Mencionado en spec; comprobar criterios/contrato |
| E50 | Admin aprobar conductor | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| E51 | Admin revisar reportes | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| E52 | Admin resolver reporte | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| N01 | Iniciar sesión | `003-identidad-perfil` | Equivalencia funcional con API implementada; ruta por alinear |
| N02 | Recuperación de acceso | `003-identidad-perfil` | Mencionado en spec; comprobar criterios/contrato |
| N03 | Crear cuenta | `003-identidad-perfil` | Equivalencia funcional con API implementada; ruta por alinear |
| N04 | Rutas cercanas y sugerid | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| N05 | Guardar zona de salida | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N06 | Consultar o actualizar h | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N07 | Consultar aviso de priva | `010-soporte-privacidad-administracion` | Mencionado en spec; comprobar criterios/contrato |
| N08 | Aceptar aviso de privaci | `010-soporte-privacidad-administracion` | Mencionado en spec; comprobar criterios/contrato |
| N09 | Consultar paradas de rut | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| N10 | Cotizar reserva | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| N11 | Consultar comprobante de | `007-solicitudes-reservas` | Mencionado en spec; comprobar criterios/contrato |
| N12 | Estado del trayecto en v | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N13 | Registrar abordaje | `006-rutas-viajes` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N14 | Permisos nativos Android | `Móvil / permisos locales Android` | Solo Android; sin endpoint |
| N15 | Finalizar expediente de | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N16 | Resumen del conductor | `005-conductores-documentos` | Referencia explícita añadida al anexo del spec dueño; contrato/aceptación pendiente |
| N17 | Filtrar mis rutas | `006-rutas-viajes` | Mencionado en spec; comprobar criterios/contrato |
| N18 | Confirmar incidencia del | `008-seguridad-incidencias` | Mencionado en spec; comprobar criterios/contrato |
| N19 | Resumen de aportaciones | `011-cierre-trazabilidad-funcional (decisión de alcance)` | Decisión de alcance sin contrato aprobado |
| N20 | Movimientos de aportacio | `011-cierre-trazabilidad-funcional (decisión de alcance)` | Decisión de alcance sin contrato aprobado |


## Revisión posterior del cotejo

Las 17 referencias que antes figuraban como **ID no localizado** ahora tienen una **mención explícita y trazable** en los anexos de `005`, `006` y `007`. Esto resuelve únicamente la ausencia del identificador: **no certifica requisitos completos, esquemas, contratos finales, aprobación ni implementación**. Persisten decisiones de alcance E15, E22, E30, N05, N06, N12 y N13, y las decisiones N19/N20 sobre aportaciones. Los módulos y la matriz deberán validarse conjuntamente con el equipo.
