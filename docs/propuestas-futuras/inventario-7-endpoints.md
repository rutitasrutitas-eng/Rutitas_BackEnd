# Inventario exploratorio — mínimo de 7 endpoints REST

> **Estado:** `BORRADOR_EXPLORATORIO` / `PENDIENTE_APROBACION`, fecha 2026-11-04.
> **Ubicación intencional:** `docs/propuestas-futuras/`, **no** dentro de `specs/`. **No es una spec funcional aprobada, contrato OpenAPI válido, tarea autorizada ni evidencia de implementación**.
> **Restricción:** la entrega docente actual solo abarca **arquitectura base**. El inventario es material de discusión para una fase posterior, cuando el profesor lo permita.
> **Decisiones relacionadas:** `BACKEND-D05` y `BACKEND-D06`; ver [propuesta tecnológica](stack-backend-y-cloud.md).

## 1. Objetivo de producto a futuro

Un estudiante institucionalmente habilitado consulta viajes, un conductor autorizado publica uno, un pasajero reserva un asiento y puede cancelar su propia reserva. El servidor protege permisos, capacidad y consistencia; la aplicación móvil solo representa los datos.

## 2. Alternativa A (preferida para evaluar): identidad administrada

Si se adopta **Firebase Authentication**, sus flujos propios de registro, login y verificación no se cuentan como endpoints desarrollados en ASP.NET Core. El servidor validará tokens Firebase y permisos de negocio.

| ID | Método | Ruta tentativa (`/api/v1` + ruta) | Propósito principal | Requiere autenticación |
|---|---|---|---|---|
| EP-01 | GET | `/usuarios/yo` | Consultar perfil y habilitación institucional | Sí |
| EP-02 | GET | `/viajes` | Buscar viajes mediante filtros y paginación | Sí (propuesta) |
| EP-03 | GET | `/viajes/{id}` | Consultar detalle permitido de viaje | Sí (propuesta) |
| EP-04 | POST | `/viajes` | Publicar viaje como conductor y vehículo habilitados | Sí |
| EP-05 | POST | `/viajes/{id}/reservas` | Reservar **un** asiento si existe cupo | Sí |
| EP-06 | GET | `/reservas/mias` | Consultar las reservas propias | Sí |
| EP-07 | DELETE | `/reservas/{id}` | Cancelar reserva propia según política | Sí |

**Precondiciones:** disponer de usuarios institucionalmente habilitados, conductores, vehículos y puntos públicos de prueba. Las altas/validaciones administrativas quedan fuera de los siete endpoints de esta primera hipótesis y **no se deben omitir para un piloto real**.

## 3. Alternativa B: identidad propia en ASP.NET Core

Si la evaluación académica requiere que los endpoints de autenticación sean implementados por el equipo, los 7 prioritarios cambiarían a:

| ID | Método | Ruta tentativa (`/api/v1` + ruta) | Propósito |
|---|---|---|---|
| EP-01 | POST | `/auth/registro` | Registro institucional, credenciales protegidas |
| EP-02 | POST | `/auth/login` | Inicio de sesión y token |
| EP-03 | POST | `/auth/verificar-correo` | Validación de correo con token de un solo uso |
| EP-04 | GET | `/viajes` | Búsqueda y filtros |
| EP-05 | POST | `/viajes` | Publicación por conductor habilitado |
| EP-06 | POST | `/viajes/{id}/reservas` | Reserva segura de un asiento |
| EP-07 | DELETE | `/reservas/{id}` | Cancelación de reserva propia |

Esta alternativa requiere adicionalmente políticas de almacenamiento seguro de contraseñas, recuperación de cuentas, verificación de correo, vencimiento/revocación de sesiones y controles antiabuso. **No mezclar A y B en una especificación definitiva sin acuerdo expreso**.

## 4. Contrato transversal propuesto (pendiente)

- API HTTPS/JSON con versión `/api/v1`; documentar formalmente en OpenAPI **solo cuando se autorice la etapa funcional**.
- Los ID de usuario/conductor/pasajero se extraen de identidad validada; nunca se confía en un `usuarioId` enviado para reclamar recursos ajenos.
- Respuestas HTTP coherentes: `200`, `201`, `204` cuando corresponda; `400`/`422` validación, `401` autenticación, `403` autorización, `404` ausencia/ocultamiento de recurso, `409` conflictos de estado/cupo, `429` límites de solicitudes. Los códigos concretos se definen en el contrato futuro.
- Documentar zonas horarias, fechas con offsets, paginación, estructura de errores, idempotencia de POST y reglas de cancelación.
- Aplicar mínimo privilegio: ningún pasajero cancela una reserva ajena ni publica con otro conductor; no exponer ubicaciones privadas.
- Diseñar idempotencia y unicidad por usuario/viaje para impedir duplicados; reservar y decrementar cupo dentro de la **misma transacción Firestore**.
- Caso de concurrencia obligatorio: al quedar 1 cupo, **máximo una reserva confirmada** ante dos solicitudes simultáneas.
- Cancelaciones deben transicionar estado y recuperar **un solo** cupo dentro de una operación atómica; decidir semántica de reintentos antes de implementar.
- Cuotas, consultas, índices compuestos y protección de datos deben medirse para el piloto.

## 5. Módulos y secuencia sugerida, no tareas ejecutables

| Módulo | Responsabilidad | Dependencias |
|---|---|---|
| Identidad/perfil | Validar identidad y habilitación; `usuarios` | Selección de Firebase Auth o identidad propia |
| Viajes | Consultas, publicación y permisos de conductor/vehículo | Modelo NoSQL e índices |
| Reservas | Capacidad, no duplicidad, cancelación y consistencia | Viajes + transacciones + usuario autorizado |
| Integración | OpenAPI, pruebas de contrato y adaptación móvil | Acuerdo BACKEND-D06 / MOVIL-D06 |

No se asignan integrantes ni fechas de implementación sin acuerdo humano. La verificación futura incluirá pruebas unitarias, integración, autorización, concurrencia, fallos/reintentos y validación de costos. El reporte de pruebas actuales sigue en **no ejecutado**.

## 6. Definición de listo futura (no satisfecha)

Un endpoint solo podría considerarse terminado después de aprobación de alcance, contrato versionado, lógica implementada, autorización comprobada, pruebas negativas/positivas y evidencia de revisión. Ninguno de estos siete está implementado ni autorizado por este documento.
