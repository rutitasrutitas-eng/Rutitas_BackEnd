# PLAN — 002 Catálogo funcional integral

**Estado:** propuesto; planificación documental. **Sin autorización de implementación ni nube.**

## 1. Método
1. Inventariar E01–E52 y N01–N20 con sus rutas originales (hecho en `inventario-funcional.md`).
2. Cruzar las 7 operaciones existentes con la matriz del inventario y el OpenAPI `001-unit2-api`.
3. Detectar duplicaciones, funciones móviles locales, cambios de tecnología y contratos incompletos.
4. Definir dominios funcionales y dependencias; obtener aprobación de líder Backend y Móvil.
5. Crear **un spec/plan/task por módulo** priorizado, sin tocar los siete endpoints anteriores.
6. Solo tras cerrar alcance, preparar el SDD de Azure (IaC) y luego SDD de CI/CD DevSecOps. No desplegar en este incremento.

## 2. Orden de specs por dependencia (propuesta, no aprobación)
| Ola | Módulos | Dependencias / motivo |
|---|---|---|
| 0 | Conciliación del contrato 001 y alcance | Base para cualquier consumidor Android |
| 1 | Identidad, perfiles, verificación, comunidades y consentimiento | Usuario y membresía verificada |
| 2 | Solicitud/aprobación conductor y vehículos | Prohibir autoaprobación; documentos privados |
| 3 | Catálogo/ruta publicable y programación/viajes | Separar trayecto de oferta fechada y conductor autorizado |
| 4 | Solicitudes, reservas, cancelación, espera y abordaje | Concurrencia y estados consistentes |
| 5 | Chat, notificaciones, valoraciones, incidentes y soporte | Seguridad por recurso y retención |
| 6 | Funciones administrativas, favoritos, informes y aportaciones | Control de acceso y costos; ajustar alcance real |

## 3. Lineamientos para los specs funcionales futuros
Cada módulo aprobado incluirá: RF/AC, actores y permisos, entidades Mongo, índices, contratos JSON con ejemplos y errores, estados, validación, concurrencia, privacidad/retención, dependencias, amenazas y una matriz RF → tarea → prueba unitaria/integración/seguridad. `OpenAPI` será fuente contractual única acordada con Android. Los cambios incompatibles deberán versionarse o migrarse formalmente.

## 4. Riesgos/decisiones a cerrar
- `E02/E03/E16/E17/N01/N03` representan operaciones existentes con prefijos/rutas distintos en el Excel.
- E21 y N17 pueden representar la misma consulta; N09 puede estar contenido en E17; N14 es permiso Android local.
- `routes` del Backend actual es catálogo; E18/E23 implican viajes publicados con cupos y fechas.
- Excel propone PostgreSQL/TypeScript; código local usa MongoDB/C#.
- Verificación institucional, documentos, identidad, geolocalización, reservas concurrentes y contribuciones necesitan decisiones y protección adicional.
- Los acuerdos con Móvil, aprobación de specs, presupuesto Azure y permisos para despliegue todavía no están acreditados.

## 5. Evidencia y controles
No modificar código en esta fase. Validación documental: conteo de 52+20, correspondencias sin duplicados, enlaces relativos correctos y revisión de responsables. Mantener 63 pruebas existentes como línea base local histórica; reejecutarlas cuando haya cambios de código. No afirmar 100 % de cobertura ni autorización docente.
