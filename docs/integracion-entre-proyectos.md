# Integración propuesta entre Rutitas Backend y el proyecto hermano

Este documento es un registro informativo dentro del proyecto Backend. No es una Constitución compartida, no modifica el otro proyecto y no constituye un contrato API aprobado.

## Límites conceptuales

- Cliente y servicio deben acordar explícitamente entradas/salidas, validaciones, errores, compatibilidad y seguridad cuando existan requisitos funcionales.
- En una integración real, el servidor será responsable de autorizar sus recursos y proteger el estado compartido; la interfaz no puede sustituir esa responsabilidad.
- La comunicación remota debe proteger los datos; HTTPS se considera un principio de diseño cuando aplique.
- Móvil considera desconexión/ciclo de vida; Backend contempla control de errores y observabilidad, cada uno según su propio ámbito.

## Pendiente entre ambos equipos

Autoridad, formato de contratos, versionado, modelo de errores, políticas de identidad, seguridad, datos, compatibilidad y pruebas de contrato. Ninguno se fija todavía. La aprobación de uno de los proyectos no ratifica automáticamente al otro.

**La fase de arquitectura base no incluye operaciones específicas, endpoints, pantallas, datos personales ni selección de proveedores.** Los anexos informativos de propuestas futuras no son contratos aprobados ni autorizan la implementación.

## Anexo informativo de futuro (registrado 2026-11-04)

La propuesta de ASP.NET Core 10/C#, Cloud Run y Firestore NoSQL, junto con posibles Maps/Firebase Authentication, está **fuera del alcance académico vigente** en [este anexo](propuestas-futuras/stack-backend-y-cloud.md). Existe un [inventario de siete endpoints](propuestas-futuras/inventario-7-endpoints.md) **únicamente exploratorio**, con dos alternativas excluyentes según el sistema de identidad. No es contrato de integración ni autorización para desarrollar. `BACKEND-D06` continúa abierto y requiere acuerdo con Rutitas_App.
