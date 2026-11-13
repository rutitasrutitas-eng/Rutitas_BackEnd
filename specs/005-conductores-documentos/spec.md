# 005 — Conductores y documentos | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado documental:** Propuesto, pendiente de revisión y aprobación.  
**Ejecución:** No iniciada para este módulo.  
**Dependencias:** 000-arquitectura-base, 001-unit2-api, 002-catalogo-funcional, 003-identidad-perfil y 004-comunidades-verificacion.  
**Alcance:** Requisitos y contratos preliminares; este documento no constituye autorización para implementar.

## 1. Objetivo

Permitir que una persona usuaria verificada solicite habilitación como conductora de viajes compartidos, registre la información necesaria de su vehículo, presente documentación cuando corresponda y reciba una decisión revisable por el personal autorizado. La verificación de identidad o pertenencia universitaria **no** implica automáticamente la habilitación para conducir.

## 2. Límites

- Incluye: solicitud, consulta de estado, actualización controlada de datos, carga protegida de evidencias, revisión administrativa, aprobación o rechazo motivado y revocación de la habilitación.
- No incluye: publicación de rutas, disponibilidad de asientos, reservaciones, cobros, seguimiento GPS continuo ni despliegue en Azure. Se tratarán en módulos separados.
- Se propone **minimizar la documentación solicitada**; la lista definitiva, fundamento y periodos de conservación necesitan aprobación del responsable de privacidad y el equipo.

## 3. Roles y estados

**Roles:** solicitante autenticado, conductor autorizado y revisor administrativo autorizado.  
**Estados propuestos de solicitud:** `draft`, `submitted`, `under_review`, `approved`, `rejected`, `revoked`.  
**Estados del vehículo:** `pending_review`, `verified`, `rejected`, `inactive` (por confirmar con producto y Móvil).

No se permitirá que la aplicación Móvil establezca por su cuenta el rol de conductor aprobado; la autorización final debe validarse y aplicarse en el servidor.

## 4. Requisitos funcionales

| ID | Requisito | Criterio de aceptación propuesto |
|---|---|---|
| COND-01 | Presentar solicitud de conductor | Requiere autenticación y pertenencia universitaria verificada; previene solicitudes activas duplicadas. |
| COND-02 | Consultar solicitud propia | Solo la persona titular o personal autorizado puede consultar sus datos. |
| COND-03 | Registrar o actualizar vehículo | Valida datos obligatorios sin asumir que registrar un vehículo lo habilita. |
| COND-04 | Adjuntar evidencias necesarias | Restringe formato, tamaño y número de archivos; almacenamiento privado y acceso temporal. |
| COND-05 | Consultar revisión y motivos | El solicitante ve estado, fecha y observaciones permitidas sin información de terceros. |
| COND-06 | Revisar solicitudes | Únicamente perfiles administrativos autorizados pueden aceptar, solicitar correcciones o rechazar. |
| COND-07 | Revocar habilitación | Se registra motivo, revisor, fecha, auditoría y efectos en la publicación futura de viajes. |
| COND-08 | Seguridad y auditoría | Los cambios de estado se verifican en el backend y se registran sin exponer documentos ni secretos. |
| COND-09 | Minimización y retención | Solo se solicita información necesaria, con plazos de eliminación pendientes de autorización. |

## 5. Contratos API propuestos; NO implementados

Se documentan rutas **candidatas**, sujetas a revisión de OpenAPI, trazabilidad del catálogo y aprobación; no asumir que funcionan en el backend actual.

| Método | Ruta propuesta | Propósito |
|---|---|---|
| POST | `/api/v1/driver-applications` | Crear solicitud de conductor. |
| GET | `/api/v1/driver-applications/me` | Consultar estado de la solicitud propia. |
| PATCH | `/api/v1/driver-applications/me` | Completar datos editables según el estado. |
| POST | `/api/v1/vehicles` | Registrar vehículo vinculado al solicitante. |
| GET | `/api/v1/vehicles/me` | Listar vehículos propios. |
| POST | `/api/v1/driver-applications/me/documents` | Solicitar o efectuar carga controlada de evidencias según diseño aprobado. |
| GET | `/api/v1/admin/driver-applications` | Listar solicitudes pendientes de revisión. |
| PATCH | `/api/v1/admin/driver-applications/{id}/decision` | Aprobar, rechazar o revocar conforme a reglas autorizadas. |

**Códigos previstos:** `200/201`, `400`, `401`, `403`, `404`, `409`, `413`, `415`, `429`. El contrato definitivo debe definir esquemas JSON, errores y versiones; no se incorporan estas rutas todavía al OpenAPI de los siete endpoints actuales.

## 6. Restricciones de privacidad y seguridad

1. No registrar identificaciones, fotografías, licencias o evidencia en logs, URLs públicas o repositorios Git.
2. Proteger archivos mediante controles de acceso, cifrado y enlaces con vencimiento si se usan.
3. Nunca aceptar un campo `approved` o `role=driver` suministrado por el cliente como autorización.
4. Validar permisos en todos los endpoints administrativos y registrar la justificación de cada decisión.
5. Definir qué documentos son necesarios, consentimiento/aviso aplicable, conservación y mecanismo de eliminación antes del desarrollo.
6. Definir quién revisa la documentación, con qué criterios y cómo se atienden correcciones o apelaciones.
7. Limitar exposición de datos de vehículo: ningún documento privado debe aparecer al consultar rutas públicas.

## 7. Integración con Móvil

Móvil necesita mostrar: requisitos, envío de solicitud, estado de revisión, corrección de datos, confirmación y mensajes de rechazo comprensibles. No podrá activar funciones de conductor hasta recibir una autorización verificable del backend. Los campos y nombres definitivos se acuerdan en un contrato compartido antes de codificar.

## 8. Reglas de trazabilidad y aprobación

- Relacionar cada requisito `COND-*` con su referencia exacta del inventario `002-catalogo-funcional/inventario-funcional.md`. **No se asignan códigos E/N sin cotejar y confirmar sus descripciones originales.**
- Validar con líder/producto, Backend, Móvil y Seguridad/Privacidad.
- Cualquier cambio de estados, campos, endpoints o documentos requeridos debe reflejarse en `spec.md`, `plan.md`, `task.md` y el futuro contrato OpenAPI.

## 9. Fuera de alcance y puntos abiertos

- ¿Qué documentos son verdaderamente obligatorios y quién puede revisarlos?
- ¿Se necesita verificar permisos legales de conducción, vehículo y seguro? Definir requisitos aplicables antes de operar viajes reales.
- ¿Se admite más de un vehículo por conductor?
- ¿Qué sucede con rutas futuras al revocar la autorización?
- ¿Cómo se vincula la aprobación de una comunidad con la solicitud de conductor?

**Criterio para cerrar esta especificación:** aprobación documental de alcance, privacidad, requisitos de revisión y contrato de API; sin ella el módulo permanece propuesto.

## 10. Anexo de trazabilidad explícita (revisión 11/11/2026)

Los identificadores siguientes proceden del catálogo `002-catalogo-funcional/inventario-funcional.md`. Su asociación **no significa implementación, aprobación ni contrato OpenAPI definitivo**.

| ID de origen | Función de catálogo | Relación con requisitos de este módulo | Pendiente para contrato/aceptación |
|---|---|---|---|
| E08 | Solicitar ser conductor | COND-01 | Definir DTO y reglas de elegibilidad |
| E09 | Consultar mi solicitud | COND-02 y COND-05 | Precisar estados y visibilidad |
| E10 | Preparar documento | COND-04 y COND-09 | Diferenciar preparación/carga; validar datos sensibles y almacenamiento |
| E11 | Registrar vehículo | COND-03 | Precisar campos, propietario y validación |
| E12 | Mis vehículos | COND-03 | Especificar listado y paginación; ruta aún candidata |
| E13 | Editar vehículo | COND-03 | Definir campos editables por estado y contrato PATCH |
| E14 | Desactivar vehículo | COND-03 y COND-07 | Definir impacto en viajes publicados y reservas |
| E50 | Admin aprobar conductor | COND-06 y COND-08 | Precisar decisiones, auditoría y autorizaciones |
| N15 | Finalizar expediente de conductor | COND-01, COND-04 y COND-05 | Definir qué convierte borrador a enviado; no solicitar documentación excesiva |
| N16 | Resumen del conductor | COND-02, COND-03 y COND-05 | Determinar datos visibles y si requiere endpoint propio |

**Acciones de cierre:** revisar cada descripción original con Producto y Móvil; aprobar campos y métodos, reglas de revisión, conservación de documentos y criterios de aceptación; solo entonces actualizar OpenAPI y tareas de implementación.
