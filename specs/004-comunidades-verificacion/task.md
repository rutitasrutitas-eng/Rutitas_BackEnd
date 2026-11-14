# TASK — 004 Comunidades y verificación institucional

**Estado general:** propuesto | **Ejecución:** no iniciada | **Autorización:** pendiente. Ninguna tarea equivale a permiso de implementar o desplegar.

| ID | Actividad | Evidencia/entregable | Estado | Dependencia |
|---|---|---|---|---|
| COM-01 | Revisar E05, E06, E07 y E49 frente a 001 y 002 | Matriz de cobertura y no duplicidad | Propuesto | Inventario 002 |
| COM-02 | Acordar UTTT piloto, comunidades futuras y múltiples membresías | Decisión formal de alcance | Propuesto | COM-01 |
| COM-03 | Definir reglas de pertenencia y prueba institucional | Política de elegibilidad/verificación | Propuesto | 003 + COM-02 |
| COM-04 | Definir roles admin, estados y revocación | Matriz de estados/autorización | Propuesto | COM-03 |
| COM-05 | Definir datos mínimos, retención y tratamiento de evidencias | Revisión de seguridad y privacidad | Propuesto | COM-03 |
| COM-06 | Reconciliar rutas Excel `/v1` vs backend `/api/v1` | Convención contractual Backend–Móvil | Propuesto | COM-01 |
| COM-07 | Especificar OpenAPI propuesto para E05–E07/E49 | Contrato DTO + códigos de respuesta | Propuesto | COM-04/05/06 |
| COM-08 | Crear matriz RF → AC → pruebas por operación | Plan de pruebas y casos negativos | Propuesto | COM-07 |
| COM-09 | Obtener aceptación formal del contrato y alcance | Registro de aprobación de responsables | Pendiente | COM-02 a COM-08 |
| COM-10 | Implementar por capas, si se autoriza | Código + pruebas + revisión | **Bloqueado** | COM-09 |
| COM-11 | Integrar pantallas Android con endpoints reales | Evidencia de pruebas de integración | **Bloqueado** | COM-10 |

## Cierre del diseño
El módulo solo se considera documentalmente listo cuando existen rutas/DTO/errores, reglas institucionales, estados, seguridad, privacidad, matriz de pruebas y aprobación explícita. La existencia de este documento no cambia el estado de los endpoints actuales ni de la nube.
