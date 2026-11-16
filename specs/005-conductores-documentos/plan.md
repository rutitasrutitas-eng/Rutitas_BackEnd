# 005 — Conductores y documentos | Plan de diseño

**Estado:** Propuesto; no ejecutar tareas de implementación hasta la aprobación correspondiente.  
**Tecnología de referencia actual:** ASP.NET Core 10/C#, MongoDB Atlas y JWT Bearer.  
**Dependencias:** módulos 003/004, decisiones de producto y privacidad.

## 1. Secuencia propuesta

1. **Reconciliar alcance:** cotejar `002-catalogo-funcional/inventario-funcional.md` y el prototipo Móvil; asignar referencias E/N exactas, sin inventar equivalencias.
2. **Definir política:** acordar elegibilidad, criterios de revisión, documentos mínimos, roles, plazos de retención y tratamiento de rechazos/revocaciones.
3. **Diseñar estados:** revisar transiciones de solicitud y vehículo, evitando saltos desde Móvil a `approved`.
4. **Diseñar datos:** entidades y repositorios para solicitudes, vehículos, metadatos documentales y eventos de auditoría; no guardar archivos binarios ni secretos en logs.
5. **Diseñar contratos:** esquemas request/response, errores, autenticación, autorización, idempotencia y paginación; agregar OpenAPI solo tras revisión del equipo.
6. **Diseñar almacenamiento seguro:** evaluar archivo privado fuera de MongoDB, antivirus/inspección y URL temporal, conforme al entorno aprobado; evitar exponer datos en CDN pública.
7. **Diseñar integración Móvil:** pantallas de solicitud, consulta, corrección y estado; confirmar nombres, identificadores y formatos de respuesta.
8. **Planificar pruebas:** unitarias de estados y permisos; integración de persistencia; contrato OpenAPI; seguridad para acceso horizontal y vertical; pruebas de archivos; auditoría de revocación.
9. **Obtener aprobación formal:** registrar decisiones, responsables, fecha y cambios, sin marcar etapas como implementadas por el hecho de estar especificadas.

## 2. Diseño lógico preliminar

- `DriverApplication`: `id`, `userId`, `status`, `submittedAt`, `reviewedAt`, `reviewerId`, motivo y versión de concurrencia.
- `Vehicle`: `id`, `ownerUserId`, campos autorizados de vehículo, estado y fechas de actualización.
- `DriverDocumentMetadata`: `id`, `applicationId`, categoría validada, referencia privada de almacenamiento, estado de revisión, vencimiento y fechas.
- `DriverReviewAudit`: acción, recurso afectado, actor, marca temporal y razón.

Estos nombres son **propuestas de diseño**, no clases existentes ni campos aprobados.

## 3. Seguridad por diseño

- Autorización JWT y controles server-side en operaciones propias y administrativas.
- Validaciones de tamaño, tipo real del archivo y límites por usuario; escaneo antimalware cuando se implemente almacenamiento.
- No almacenar documentos personales en directorios públicos del contenedor.
- Configuración secreta fuera de `appsettings.json` y Git.
- Auditoría de decisiones y políticas de eliminación configurables y aprobadas.
- Protección contra doble revisión concurrente y cambios de estado ilegítimos.

## 4. Riesgos y mitigación

| Riesgo | Mitigación documental / técnica propuesta |
|---|---|
| Solicitud de documentos excesivos | Revisión de minimización de datos antes de habilitar carga. |
| Escalación de privilegios desde Android | Derivar la autorización exclusivamente de decisiones del servidor. |
| Exposición de archivos sensibles | Almacenamiento privado, acceso temporal y controles RBAC. |
| Decisiones repetidas por concurrencia | Versionado de registro e idempotencia. |
| Bloqueo de usuarios legítimos | Flujo de corrección y revisión motivada. |

## 5. Entregables antes de implementar

- Especificación de reglas y roles aprobada.
- Tabla de equivalencia con catálogo E/N.
- Contrato de API validado por Móvil, Backend y Seguridad.
- Diseño de almacenamiento y conservación aprobado.
- Matriz de pruebas esperadas.

**No se ejecuta código, no se altera MongoDB y no se crea infraestructura Azure en esta fase.**
