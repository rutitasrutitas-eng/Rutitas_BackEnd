# SPEC — 002 Catálogo funcional integral del Backend de Rutitas

**Estado:** propuesto / documental. **Ejecución:** no_iniciada para nuevas funciones. **Autorizaciones:** pendientes. **Fecha:** 11/11/2026.

## 1. Propósito y límites
Construir el inventario completo, sin ambigüedades ni duplicaciones, de las funciones que debe soportar el Backend para la aplicación Android. Este incremento **no** implementa endpoints, no cambia las 7 operaciones de `001-unit2-api` y no crea servicios Azure.

## 2. Fuentes y precedencia
- `Rutitas_Actividad_4.xlsx` (archivo recibido del equipo, fuera del árbol Backend) es fuente de propuesta de alcance, **no** un contrato ejecutable.
- `../001-unit2-api/spec.md`, `plan.md`, `task.md` y `contracts/openapi.yaml` documentan 7 operaciones previamente implementadas y probadas localmente.
- `inventario-funcional.md` registra las 72 fichas propuestas, separadas por módulo, con correspondencias y solapamientos.
- Constitución, decisiones `BACKEND-Dxx` y el acuerdo con Móvil prevalecen cuando corresponda. Si alguna contradicción está abierta, se registra y no se implementa hasta resolverla.

## 3. Requisitos funcionales de esta fase documental
| ID | Requisito comprobable |
|---|---|
| RF-CAT-01 | Identificar 52 operaciones E y 20 fichas N con fuente, módulo, método y ruta propuestos. |
| RF-CAT-02 | Registrar las 7 operaciones actuales, sus equivalencias E02/E03/E16/E17/N01/N03 y la ausencia de logout del catálogo E/N. |
| RF-CAT-03 | Distinguir función local Android N14 de funciones de servidor; impedir que se convierta en endpoint. |
| RF-CAT-04 | Señalar duplicidades, dependencias, diferencias de ruta y datos requeridos antes de planear desarrollos. |
| RF-CAT-05 | Agrupar futuras especificaciones por dominio (usuarios, comunidades, conductores, vehículos, rutas/viajes, reservas, seguridad y administración). |
| RF-CAT-06 | Crear criterios de aceptación de seguridad, privacidad, datos y pruebas para cada módulo **cuando su spec sea aprobado**. |
| RF-CAT-07 | Registrar acuerdos y aprobaciones con responsables de Backend y Móvil, sin inferir consentimiento ni autorizaciones. |

## 4. Reglas no negociables
- El backend sigue en ASP.NET Core 10/C# y MongoDB Atlas hasta una decisión formal; el Excel propone TypeScript/PostgreSQL y no autoriza una migración.
- IDs de MongoDB expuestos en contratos como `string` (ObjectId) para recursos existentes; no cambiar a UUID o `Long` por el prototipo.
- El control de autorización reside en Backend: JWT válido + verificación de propiedad/rol/estado vigente. La app no se autoaprueba como conductor ni administra asientos localmente en producción.
- Nueva funcionalidad: especificación y plan **antes** de código; pruebas unitarias, integración real aislada y seguridad por operación, más contratos con Android.
- No mezclar **catálogo de ruta** con **viaje publicado con cupos** sin modelado y acuerdo explícitos.
- Los datos sensibles (identidad, documentos, ubicaciones, chats) exigen minimización, retención y acceso por recurso.

## 5. Criterios de aceptación
| ID | Aceptación documental verificable |
|---|---|
| AC-CAT-01 | El inventario cuenta exactamente 52 E + 20 N y conserva fuente y métodos originales. |
| AC-CAT-02 | Las 7 operaciones ya existentes aparecen exactamente una vez como referencia del contrato actual, sin nuevas rutas duplicadas. |
| AC-CAT-03 | N14 es local; N09/E17 y N17/E21 se marcan como solapamientos posibles para decisión de equipo. |
| AC-CAT-04 | Cada módulo tiene una lista de dependencias y riesgos antes de desglosar specs de implementación. |
| AC-CAT-05 | Los DTO, semántica de rutas, autoridad de conductores y reglas de reservas quedan como decisiones pendientes, no hechos implementados. |
| AC-CAT-06 | El líder de Backend y el responsable Móvil aprueban por escrito alcance, rutas canónicas y prioridades; el estado pasa a aprobado **solo entonces**. |
| AC-CAT-07 | El spec Azure IaC se inicia **después** del cierre del inventario funcional, siguiendo la indicación del profesor. |

## 6. Exclusiones
No se programan endpoints, no se corrige Móvil, no se ejecutan nuevas tareas del producto, no se cambia Atlas, no se crean recursos Azure ni workflows. Este documento es de descubrimiento y alineación.

## 7. Evidencia disponible y pendiente
**Existente:** capturas de 63/63 pruebas en el Backend de 7 operaciones, contrato y repositorios C#; no se extrapola a las 72 fichas. **Pendiente:** ratificación de alcance E/N, deduplicación formal, contratos por módulo, firmas/decisiones SDD, pruebas de cliente Android y objetivos de infraestructura.
