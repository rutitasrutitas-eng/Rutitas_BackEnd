# MAPS-001 - Matriz de trazabilidad

Estado: CANDIDATE

| Requisito | Operacion | Pruebas propuestas |
|---|---|---|
| RF-01 | GET /maps/search | MAP-V01, V02, V03 |
| RF-02 | GET /maps/geocode | MAP-V04, V05 |
| RF-03 | GET /maps/reverse-geocode | MAP-V06, V07 |
| RF-04 | POST /maps/directions | MAP-V08, V09, V10 |
| RF-05 | DTO normalizados | MAP-V11 |
| RF-06 | ProblemDetails | MAP-V12 a V17 |
| RNF-01 | JWT y autorizacion | MAP-V12, V13 |
| RNF-02 | Identidad administrada | MAP-V18 |
| RNF-03 | Proteccion de secretos | MAP-V19 |
| RNF-04 | Timeout y cancelacion | MAP-V16, V17 |
| RNF-05 | Rate limiting | MAP-V14 |
| RNF-06 | Privacidad de logs | MAP-V20 |
| RNF-07 | Reintentos controlados | MAP-V21 |
| RNF-08 | Restricciones de cache | MAP-V22 |
| RNF-09 | Presupuesto | MAP-V23 |

## Gates propuestos

- G0: alcance y autorizacion documental recibidos.
- G1: especificacion y contrato candidatos.
- G2: arquitectura y seguridad propuestas.
- G3: implementacion simulada pendiente.
- G4: pruebas y conciliacion Android pendientes.
- G5: revision independiente y cierre pendientes.

Esta numeracion es propuesta y debe conciliarse
con la metodologia oficial de Rutitas.

No equivale a la aprobacion formal de los gates.
