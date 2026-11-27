# BACKEND-DELETE-001 - Retencion y privacidad

Observacion: DEL-FIX-05
Fecha: 2026-11-26

Estado: PROPUESTA EN REVISION.

Responsables de revision:
Seguridad y privacidad / Coordinacion.

## Objetivo

Minimizar datos personales conservados despues
de la eliminacion de una cuenta.

No implementar politicas definitivas de
retencion sin aprobacion expresa.

## Inventario actualizado

| Recurso | Tratamiento actual | Pendiente |
|---|---|---|
| users | Eliminacion fisica local del titular | Inventario productivo integral |
| account_deletion_jobs | Checkpoints y estado durable | Retencion y minimizacion |
| routes | Preservacion de datos compartidos | Propiedad y anonimimizacion |
| revoked_tokens | Revocacion temporal de JWT | Validar expiracion efectiva |
| Backups | Fuera del alcance local | Politica de restauracion y purga |
| Modulos futuros | No implementados en este alcance | Inventario y propiedad |

## Identificadores de trabajos

El identificador del trabajo incorpora UserId
y ExpectedSessionVersion.

La coleccion conserva datos vinculables al titular.

No considerar estos registros anonimos.

Propuesta:

- Conservar identificadores solo mientras sean
  necesarios para ejecutar y reconciliar trabajos.
- Definir minimizacion posterior a verificacion.
- Evaluar identificadores opacos de auditoria.
- No conservar credenciales, JWT o recovery tokens.
- No eliminar checkpoints necesarios para reconciliacion.

No se establece un plazo definitivo de retencion.

## Trabajos bloqueados

Un trabajo bloqueado no equivale a eliminacion exitosa.

Procedimiento propuesto:

1. Identificar estado y checkpoint persistido.
2. Verificar el resultado real del ultimo paso.
3. Revisar identidad y version del titular.
4. Conciliar operaciones con resultado desconocido.
5. Reanudar solo con autorizacion y fence vigente.
6. Verificar ausencia de los datos correspondientes.
7. Registrar evidencia minima sin datos sensibles.

Prohibido marcar como completado un trabajo
sin verificaciones suficientes.

## Respaldos

Pendiente de definir:

- Plazo de conservacion.
- Eliminacion programada.
- Restricciones legales.
- Control de acceso.
- Restauracion sin reactivar cuentas eliminadas.
- Reconciliacion de eliminaciones previas.

No se han modificado respaldos productivos.

## Recursos compartidos

No eliminar rutas u otros recursos de terceros
sin una relacion de propiedad comprobada.

Para futuros modulos, definir:

- Propietario.
- Dependencias.
- Datos personales.
- Datos compartidos.
- Eliminacion, desvinculacion o anonimimizacion.
- Excepciones autorizadas.

## Riesgos pendientes

- Conservacion de identificadores en jobs.
- Trabajos bloqueados tras agotar reintentos.
- Restauracion de datos previamente eliminados.
- Datos personales dentro de recursos compartidos.
- Inventario incompleto de servicios futuros.

## Ratificacion

Conformidad de Seguridad y Privacidad: PENDIENTE.
Ratificacion de Coordinacion: PENDIENTE.

No habilitar TTL ni purgas automaticas adicionales
hasta resolver las decisiones pendientes.

Produccion: NO-GO.
