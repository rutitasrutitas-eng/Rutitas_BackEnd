# 010 — Plan SDD | Soporte, privacidad y administración transversal

**Estado:** PROPUESTO · trabajo documental; sin implementación autorizada.  
**Trazabilidad principal:** E46, E47, E48, N07, N08.  
**Referencias:** E49 → 004; E50 → 005; E51–E52 → 008.

## 1. Orden de trabajo previsto

1. Validar que los IDs se cubren una sola vez en el catálogo `002` y no se crean rutas administrativas duplicadas.
2. Revisar requisitos de producto, riesgos de datos de terceros y contenido legal con Seguridad/Privacidad.
3. Aprobar si el piloto necesita contactos de confianza y qué pueden hacer realmente; no prometer alertas automáticas.
4. Definir flujo de tickets (categorías, estados, respuesta, responsable, plazos de conservación, escalamiento de reportes de seguridad).
5. Aprobar el aviso de privacidad y su versionado. Distinguir mera visualización del aviso de los consentimientos donde sean necesarios.
6. Acordar contratos `/api/v1`, DTO, errores, JWT, paginación, límites y compatibilidad con Android.
7. Diseñar modelos MongoDB, índices, auditoría y política de retención/acceso, sin implementar aún.
8. Preparar OpenAPI 3.1 y matriz de casos de prueba cuando el contrato sea aprobado.
9. Obtener firmas/gate de liderazgo, Móvil y Seguridad. Solo después planificar cambios de código y despliegue.

## 2. Diseño por capas (candidato)

| Capa | Responsabilidad prevista |
|---|---|
| `Rutitas.Api` | Endpoints de contactos, tickets, aviso y consentimientos; autenticación, validaciones y límites |
| `Rutitas.Application` | Casos de uso, permisos por propietario, política de versiones, validación y ciclo de tickets |
| `Rutitas.Domain` | Entidades y estados puros del dominio; invariantes y reglas de consentimientos |
| `Rutitas.Infrastructure` | Repositorios MongoDB, índices, almacenamiento seguro y trazas de auditoría |
| Pruebas | Contratos, unitarias, integración, seguridad/IDOR y aceptación Android |

## 3. Hitos y salidas

| Hito | Entregable esperado | Condición para avanzar |
|---|---|---|
| A | Alcance y trazabilidad | E46–E48 y N07–N08 asignados sin duplicar 004/005/008 |
| B | Reglas de privacidad y soporte | Aprobadas por responsables pertinentes |
| C | Contrato Backend–Móvil | Rutas, DTO, errores, semántica de consentimiento y pruebas aceptadas |
| D | Diseño de datos y amenazas | Retención, accesos, terceros y abuso revisados |
| E | Gate documental | Aprobación expresa antes de implementar |

## 4. Riesgos relevantes

- Tratar datos de contacto de terceros sin base jurídica ni salvaguardas.
- Confundir aviso de privacidad, consentimiento opcional y obligatoriedad de aceptar.
- Habilitar soporte administrativo sin controles de acceso ni registro de acciones.
- Duplicar reportes de incidentes o supuestas capacidades de emergencia.
- Prometer cumplimiento legal, envío automático de alertas o supervisión 24/7 sin evidencia.

**Dependencias para Azure:** primero fijar categorías de datos, secretos, almacenamiento, límites de logs y retención; después diseñar infraestructura como código y CI/CD. No crear recursos aún.
