# PLAN — 004 Comunidades y verificación institucional

**Estado:** propuesto | **Fase:** diseño documental SDD | **Código, base de datos, nube:** sin modificaciones.

## 1. Dependencias previas
- Leer `../002-catalogo-funcional/inventario-funcional.md` para E05–E07/E49 y contrato implementado en `../001-unit2-api/contracts/openapi.yaml`.
- Coordinar con `003-identidad-perfil` la prueba de correo y la eliminación de cuenta.
- Confirmar con el líder y los responsables de Backend, Seguridad/Privacidad y Móvil el alcance UTTT e interuniversitario.

## 2. Secuencia de diseño propuesta
1. Aprobar reglas de elegibilidad de la comunidad y diferencia entre **cuenta validada**, **membresía institucional** y **conductor aprobado**.
2. Diseñar entidades lógicas *Community*, *Membership* y, si se aprueba, *VerificationEvidence*; no crear colecciones MongoDB todavía.
3. Resolver decisión de múltiples comunidades, alta administrativa, pruebas de pertenencia y revocación.
4. Fijar nombres canónicos de rutas y parámetros; reconciliar `/v1/me/...` del Excel con `/api/v1/users/me` del backend.
5. Proponer DTO de petición/respuesta, paginación, estados, errores (400/401/403/404/409/429 según operación) y reglas de idempotencia.
6. Revisar privacidad, almacenamiento mínimo, permisos de administración, políticas de auditoría y retención de evidencias.
7. Preparar OpenAPI **propuesto** en una futura carpeta de contratos de 004, sin mezclarlo con el OpenAPI ya implementado en 001.
8. Diseñar pruebas unitarias, integración con Mongo temporal, seguridad y contrato móvil.
9. Registrar aprobaciones explícitas antes de programar; después, y solo entonces, asignar implementación por capas y tareas ejecutables.

## 3. Posible arquitectura al implementarlo (no autorizada aún)
- `Rutitas.Domain`: reglas de comunidad, membresía y estado.
- `Rutitas.Application`: casos de uso y políticas de autorización independientes del controlador.
- `Rutitas.Infrastructure`: repositorios MongoDB, restricciones de unicidad y manejo de evidencia aprobado.
- `Rutitas.Api`: controladores y DTO; no aceptar roles ni `userId` arbitrarios desde el móvil en acciones `/me`.
- Proyectos de pruebas: cobertura de estados, acceso cruzado, duplicados, reintentos y protección de datos.

## 4. Decisiones de diseño bloqueantes
| Decisión | Responsable por confirmar | Evidencia necesaria |
|---|---|---|
| Instituciones admitidas y alcance piloto | Líder / Producto | Aprobación escrita |
| Métodos de validación institucional | Backend + Seguridad + Móvil | Política, flujo y casos de error |
| Administración y revocación de membresías | Líder + Seguridad | Matriz de roles/autorización |
| Retención de evidencias y datos de comunidad | Responsable de privacidad | Política y control de acceso |
| Contratos JSON y rutas finales | Backend + Móvil | Revisión firmada del contrato |

## 5. Estrategia de verificación futura
Casos normales; usuario no autenticado; acceso a membresía ajena; intento de autoasignarse rol admin; correo no verificado; comunidad inexistente; membresía duplicada; solicitud rechazada/revocada; cambios concurrentes; paginación; errores sin datos sensibles. Ejecutar regresión de los 63 casos existentes después de cualquier cambio de código, con Docker disponible.

**Sin despliegue:** las especificaciones de infraestructura Azure/IaC y CI/CD DevSecOps se elaborarán cuando se cierre el catálogo funcional y se autorice el alcance.
