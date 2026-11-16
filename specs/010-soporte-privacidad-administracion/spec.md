# 010 — Soporte, privacidad y administración transversal | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado:** PROPUESTO · solo documentación · NO implementado ni aprobado.  
**Origen:** `specs/002-catalogo-funcional/inventario-funcional.md`, IDs **E46–E48, N07–N08**.  
**Referencias sin duplicación:** E49 pertenece al 004; E50 al 005; E51–E52 al 008.  
**Dependencias:** 003 identidad/perfil, 004 comunidades, 005 conductores y 008 seguridad/incidencias.

## 1. Objetivo y alcance

Definir la gestión de solicitudes de soporte, contactos de confianza y aceptación verificable del aviso de privacidad para el piloto universitario, sin duplicar la moderación o la administración de comunidades y conductores ya asignada a otros módulos.

**Estos endpoints no existen en el backend actual de siete operaciones.** El presente archivo propone comportamiento y criterios para revisión; no autoriza implementación, ni representa cumplimiento legal comprobado.

## 2. Requisitos funcionales y criterios de aceptación propuestos

| ID interno | ID catálogo | Requisito | Criterio de aceptación propuesto |
|---|---|---|---|
| SPA-01 | E46 | Contactos de confianza | Persona autenticada puede registrar contacto con consentimiento/base legal definidos, validación de formato y límite de cantidad; el contacto no recibe ubicación automáticamente. |
| SPA-02 | E47 | Crear ticket de soporte | Persona autenticada registra asunto, categoría y descripción con límites; servidor asigna `id`, fecha UTC y estado inicial. |
| SPA-03 | E48 | Mis tickets | Persona autenticada consulta únicamente sus tickets con orden y paginación; evita exposición de datos de otros usuarios. |
| SPA-04 | N07 | Consultar aviso de privacidad | Cliente obtiene la versión vigente, fecha de vigencia y referencia al documento publicado; disponibilidad pública sujeta a aprobación. |
| SPA-05 | N08 | Registrar aceptación | Usuario autenticado registra una aceptación auditable vinculada a versión, finalidad y fecha; nunca se toma como prueba suficiente un booleano local en Android. |
| SPA-06 | Transversal | Auditoría y gestión de datos | Acceso de personal autorizado por rol y motivo, trazabilidad de cambios, retención y mecanismos de atención de derechos conforme a revisión legal. |
| SPA-07 | Transversal | Integración con otros módulos | Mantiene separados los tickets de soporte (010) de los reportes de seguridad (008); cualquier vinculación es explícita. |

## 3. Contratos HTTP candidatos — SIN IMPLEMENTAR

El inventario fuente usa `/v1`; el backend actual usa `/api/v1`. Para evitar duplicidad se propone conservar `/api/v1`, **pendiente de consenso**.

| ID | Método | Ruta candidata | Acceso |
|---|---|---|---|
| E46 | POST | `/api/v1/me/trusted-contacts` | Autenticado, titular de la cuenta |
| E47 | POST | `/api/v1/support/tickets` | Autenticado |
| E48 | GET | `/api/v1/support/tickets` | Autenticado, solo tickets propios |
| N07 | GET | `/api/v1/legal/privacy-notice` | Público si así se aprueba |
| N08 | POST | `/api/v1/me/consents` | Autenticado |

**Fuera de alcance sin duplicar:** `POST /api/v1/admin/communities` (E49 → 004), revisión de conductores E50 (→ 005), y `GET/PATCH /api/v1/admin/reports...` (E51/E52 → 008). La atención administrativa de tickets requeriría un contrato adicional y autorización específica; **no aparece como endpoint separado en el inventario E/N**.

## 4. Datos candidatos (no son tablas/colecciones existentes)

- `TrustedContact`: `id`, `ownerUserId`, `name`, `phone?`, `email?`, `createdAtUtc`, `revokedAtUtc?`. Definir base legal, aviso al tercero, uso y plazo de conservación antes de almacenar datos de terceros.
- `SupportTicket`: `id`, `ownerUserId`, `category`, `subject`, `description`, `status`, `createdAtUtc`, `updatedAtUtc`. Restringir adjuntos, datos sensibles y exportación.
- `PrivacyNoticeVersion`: `version`, `effectiveAtUtc`, `documentUrl` o `content`, `status`, `publishedAtUtc`. La publicación requiere contenido aprobado por responsables.
- `UserConsent`: `id`, `userId`, `noticeVersion`, `purpose`, `acceptedAtUtc`, `channel`, `revokedAtUtc?`. Separar aceptación de aviso, consentimiento opcional y obligaciones que tengan distinta base jurídica.

## 5. Seguridad, privacidad y decisiones de producto

1. No confiar en `ownerUserId`, `userId`, rol o estado enviados por Móvil; derivarlos del JWT y reglas de servidor.
2. Proteger tickets y contactos contra IDOR, enumeración e inyección; validar longitud y contenido, aplicar límites de solicitudes y registros de auditoría sin secretos.
3. No publicar teléfono, correo, documentación, ubicación en tiempo real ni información de terceros en respuestas de otros usuarios.
4. La lista de contactos de confianza **no equivale** a un botón de emergencia funcional ni garantiza notificaciones/llamadas automáticas.
5. El texto del aviso y las finalidades deben ser aprobados por responsables de producto/privacidad; no redactar declaraciones legales ficticias como si ya fueran política oficial.
6. Decidir mecanismos de revocación, actualización de avisos y derechos de acceso, corrección, cancelación u oposición conforme al marco aplicable y asesoría correspondiente.
7. Restringir tickets administrativos; uso de rol en servidor, auditoría, tratamiento de datos sensibles y protocolos para incidentes.
8. Un ticket de soporte no sustituye el flujo de denuncia de seguridad E42/E43/E51/E52 del módulo 008.

## 6. Pruebas y criterios de salida

- Pruebas `401` sin JWT, `403/404` por recurso ajeno según política aprobada, `400` por validación, `409` o idempotencia de aceptación repetida si así se pacta, y `429` ante exceso de peticiones.
- Verificar versionado correcto de aviso, identidad/propiedad de tickets, acceso a contactos, eliminación/retención, sanitización y auditoría.
- Casos Android: sincronización de versión vigente, no aceptación silenciosa y mensaje claro ante aviso actualizado.
- Diseñar después casos unitarios, integración MongoDB/Testcontainers, contrato OpenAPI 3.1 y seguridad. **Las 63 pruebas existentes no validan estas funciones.**

## 7. Gate de aprobación

Pendientes de aprobación expresa de liderazgo/producto, Backend, Móvil y Seguridad/Privacidad: texto y versión de aviso, naturaleza y retención de los consentimientos, recolección de datos de terceros, flujo de tickets y permisos de personal. Estado de ejecución: **NO INICIADO**. No modificar controladores, persistencia ni despliegue con base solamente en este documento.
