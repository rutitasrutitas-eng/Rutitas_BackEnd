# TASK — 003 Identidad y perfil

**Estado general:** propuesto — **Ejecución:** no iniciada. **Ninguna tarea constituye permiso para implementar o desplegar**.

| ID | Tarea | Entregable | Estado | Depende de |
|---|---|---|---|---|
| ID-01 | Confirmar equivalencia N01/N03/E02/E03 con endpoints de 001 | Tabla contractual Android ↔ Backend | Propuesto | Contrato 001 |
| ID-02 | Definir política de correo institucional y verificación E01 | Decisión y casos de uso | Propuesto | Aprobación de alcance |
| ID-03 | Definir flujo N02 de solicitud y confirmación de restablecimiento | Secuencia y DTO propuestos | Propuesto | ID-02 |
| ID-04 | Definir borrado E04 y política de retención | Matriz de efectos en datos relacionados | Propuesto | Revisión privacidad |
| ID-05 | Preparar OpenAPI propuesto para E01/E04/N02 | Contrato revisable, no implementado | Propuesto | ID-02/03/04 |
| ID-06 | Diseñar pruebas unitarias, integración, seguridad y contrato | Matriz RF → prueba | Propuesto | ID-05 |
| ID-07 | Obtener conformidad de Backend/Móvil y autorización SDD | Registro de acuerdos y responsables | Pendiente | ID-01 a ID-06 |
| ID-08 | Implementar operaciones aprobadas, sin alterar contratos existentes | Código, pruebas y revisión de seguridad | **Bloqueado** | ID-07 |
| ID-09 | Integrar Android y verificar flujos completos | Evidencia de pruebas de aceptación | **Bloqueado** | ID-08 |

## Criterio de cierre documental
Todas las rutas, DTO, casos de error, reglas de autorización, privacidad, pruebas y decisiones abiertas tienen respaldo y aprobación explícita. No se modifican estados a “aprobado” sin evidencia de responsables.
