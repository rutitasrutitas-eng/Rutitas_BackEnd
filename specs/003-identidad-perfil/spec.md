# SPEC — 003 Identidad, perfil y recuperación de acceso

**Estado:** propuesto — **Ejecución:** no iniciada para nuevas operaciones — **Aprobaciones:** pendientes — **Referencia:** `../002-catalogo-funcional/inventario-funcional.md`.

## 1. Objetivo
Definir el contrato pendiente del módulo de identidad sin reemplazar los cinco endpoints de autenticación/perfil que ya operan en `001-unit2-api`. El alcance nuevo del catálogo es E01 (verificación de correo), E04 (eliminar cuenta) y N02 (recuperar acceso). E02/E03 y N01/N03 corresponden a endpoints actuales.

## 2. Contratos existentes: NO duplicar
| Origen catálogo | Operación REST real | Estado |
|---|---|---|
| N03 | `POST /api/v1/auth/register` | Implementado en 001 |
| N01 | `POST /api/v1/auth/login` | Implementado en 001 |
| Sin ficha E/N | `POST /api/v1/auth/logout` | Implementado en 001 |
| E02 | `GET /api/v1/users/me` | Implementado en 001 |
| E03 | `PATCH /api/v1/users/me` | Implementado en 001 |

Los contratos y códigos de error existentes deben consultarse directamente en `../001-unit2-api/contracts/openapi.yaml`. No alterar su respuesta JSON, rutas ni reglas JWT sin revisión conjunta Backend–Móvil.

## 3. Funcionalidades nuevas a especificar (NO implementadas)
| ID | Propuesta de operación canónica | Requisitos de comportamiento |
|---|---|---|
| RF-ID-01 / E01 | `POST /api/v1/auth/verify-email` | Verificar titularidad del correo mediante token temporal de un solo uso, con vencimiento, control de intentos y estado de verificación en servidor. |
| RF-ID-02 / E04 | `DELETE /api/v1/users/me` | Autenticar al solicitante y confirmar intención; definir desactivación, anonimización y retención aplicable, invalidación de sesiones y efecto sobre rutas, reservas e incidencias. |
| RF-ID-03 / N02 | `POST /api/v1/auth/password-reset` (flujo por definir) | Solicitud sin filtración de existencia de cuentas; emisión de token temporal y confirmación de cambio de contraseña con revocación de sesiones vigentes. Podrían requerirse **dos** operaciones; decidir antes de OpenAPI. |

> Las rutas propuestas unifican el prefijo `/api/v1` y **no son un contrato aprobado**. En particular, no implementar recuperación de contraseña con un único POST hasta resolver sus pasos y DTO.

## 4. Seguridad y privacidad obligatorias
- Nunca devolver contraseñas, hashes, códigos de verificación, tokens de recuperación ni detalles que permitan enumerar correos.
- Controlar expiración, consumo único, reintentos, límite de solicitudes y respuestas uniformes de recuperación.
- Comprobar propiedad y estado de cuenta en el servidor; no confiar en campos de roles enviados por Android.
- Definir tratamiento de datos tras borrado de cuenta: conservación legal necesaria, minimización, relaciones de viajes y auditoría. Documentar política de retención antes de implementar.
- Determinar si correo institucional es obligatorio, dominios aceptados y proceso de desvinculación de comunidad; no asumir reglas no autorizadas.
- No almacenar claves de correo, JWT ni otras credenciales en el repositorio; administrar secretos fuera del código.

## 5. Criterios de aceptación documental
| ID | Comprobación exigida antes de implementar |
|---|---|
| AC-ID-01 | El contrato 001 mantiene sus cinco operaciones y los esquemas compatibles con Android o cuenta con migración aprobada. |
| AC-ID-02 | E01 define emisor, token, caducidad, reenvío, idempotencia y errores, incluyendo pruebas de reutilización. |
| AC-ID-03 | N02 especifica solicitud y confirmación de restablecimiento, sin enumeración de cuentas y con revocación. |
| AC-ID-04 | E04 define consecuencias de eliminación sobre reservas, mensajes, incidencias, auditoría y datos personales. |
| AC-ID-05 | Existe matriz RF → contrato → tarea → prueba unitaria/integración/seguridad y conformidad de Móvil. |
| AC-ID-06 | Las decisiones de privacidad y el responsable aprobador figuran por escrito antes de cambiar código. |

## 6. Exclusiones y decisiones abiertas
No incluye comunidades (E05–E07), consentimiento (N07–N08), aprobación de conductores ni nuevas rutas de viajes. Deben resolverse: política de verificación de correo, mecanismo de envío, recuperación en dos pasos, borrado/retención, códigos exactos y esquemas JSON. No se ejecuta despliegue Azure en este incremento.
