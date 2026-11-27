# BACKEND-DELETE-001 - Cobertura consolidada

## Identificacion

- Observacion: DEL-FIX-07.
- PR: Draft #7.
- Rama: feature/backend-delete-001.
- Fecha local: 2026-11-26.
- Configuracion: Release.
- Framework: .NET 10.
- Entorno: Testing.
- Base HEAD: 19a86238a3dd55cf4f86fe28cfb9fed9f28b33ea.
- Estado: cambios locales aun no publicados.

## Regresion automatizada

| Proyecto | Total | PASS | FAIL | NOT_RUN |
|---|---:|---:|---:|---:|
| ArchitectureTests | 2 | 2 | 0 | 0 |
| ContractTests | 9 | 9 | 0 | 0 |
| IntegrationTests | 91 | 91 | 0 | 0 |
| SecurityTests | 26 | 26 | 0 | 0 |
| UnitTests | 38 | 38 | 0 | 0 |
| Total | 166 | 166 | 0 | 0 |

Resultado: FULL REGRESSION PASS.

Se incorporaron 18 casos respecto a la suite
anterior de 148 pruebas.

## Cobertura consolidada de lineas

| Ensamblado | Cubiertas | Instrumentadas | Cobertura |
|---|---:|---:|---:|
| Rutitas.Api | 1153 | 1655 | 69.67% |
| Rutitas.Application | 3 | 5 | 60.00% |
| Rutitas.Domain | 50 | 54 | 92.59% |
| Rutitas.Infrastructure | 753 | 851 | 88.48% |
| Total | 1959 | 2565 | 76.37% |

Lineas pendientes: 606.

## Metodologia

- Inspeccion directa de los XML Cobertura.
- Cinco informes principales.
- Un informe duplicado descartado.
- Normalizacion de rutas de archivos fuente.
- Union por ensamblado, archivo y numero de linea.
- Una linea se considera cubierta cuando al menos
  una suite registra hits superiores a cero.
- Exclusivamente los cuatro ensamblados del Backend.
- Exclusion de ensamblados de pruebas.
- No se suman ni promedian porcentajes.

## Cobertura individual de lineas

| Suite | Cubiertas | Instrumentadas | Cobertura |
|---|---:|---:|---:|
| IntegrationTests | 1841 | 2565 | 71.77% |
| SecurityTests | 687 | 2565 | 26.78% |
| UnitTests | 478 | 2565 | 18.64% |
| ContractTests | 0 | 2565 | 0.00% |
| ArchitectureTests | 0 | 59 | 0.00% |

La ausencia de hits en pruebas estructurales
no implica que estas hayan fallado.

## Cobertura de ramas

La cobertura consolidada exacta permanece pendiente.

Los XML individuales contienen resultados agregados
de condiciones que no permiten identificar todos los
caminos ejecutados de forma inequivoca.

No se publica una cifra consolidada no verificable.

## Artefacto

DELETE001-COVERAGE.zip

SHA256:
8349237ce950eca245345214099895a8ab33f2ff218b7c15d3c9938be820d335

El artefacto permanece en DELETE001-Evidencias.
No incorporar archivos temporales ni logs al repositorio.

## Limitaciones

- Cobertura de lineas no equivale a seguridad.
- Exito de pruebas no equivale a cobertura total.
- No acredita SAST o DAST integral.
- No acredita seguridad TLS productiva.
- No acredita pruebas conjuntas Android-Backend.
- El resultado corresponde al estado local probado,
  no al SHA remoto anterior ni a una nueva ejecucion CI.
- Cualquier cambio posterior de codigo requiere
  actualizar la evidencia correspondiente.

## Dictamen

Regresion local: PASS.
Cobertura consolidada de lineas: 76.37%.
Cobertura consolidada de ramas: PENDIENTE.
DEL-FIX-07: evidencia local de lineas disponible.
Produccion: NO-GO.

Sin datos reales.
Sin despliegue.
Sin modificaciones de Atlas o Azure.
Sin gastos adicionales.


## Actualizacion final de verificacion

La regresion Release posterior a las correcciones
aprobo 170/170 casos, sin fallos ni omisiones.

Cobertura consolidada de lineas:
2037 de 2650, equivalente a 76.87%.

La evidencia anterior conserva su valor historico.
Consultar evidencia-final-correcciones-pr7.md
para el estado actualizado.

CI del nuevo commit: pendiente de verificacion.
Aprobaciones pendientes y restricciones:
sin modificaciones.

Produccion: NO-GO.
