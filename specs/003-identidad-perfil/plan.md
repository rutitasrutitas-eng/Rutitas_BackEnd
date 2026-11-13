# PLAN — 003 Identidad y perfil

**Estado:** propuesto; planificación SDD exclusivamente. Sin implementación ni aprobación de despliegue.

## Secuencia propuesta
1. Conciliar `001-unit2-api/contracts/openapi.yaml` con `ApiService.kt` y DTOs Android; conservar cinco operaciones implementadas.
2. Validar con el equipo las prioridades de E01, E04 y N02 y quién autoriza verificación/borrado.
3. Modelar estados: correo no verificado/verificado; token emitido/consumido/expirado; cuenta activa/en proceso de baja/desactivada.
4. Resolver primero políticas de privacidad, retención, notificaciones de correo y revocación de JWT.
5. Especificar DTO, errores, autenticación y ejemplos de OpenAPI para cada operación nueva, luego solicitar revisión con Móvil.
6. Definir estrategia de pruebas por endpoint: éxito, token incorrecto, expirado, reutilizado, suplantación, tasa de peticiones y acceso tras revocación.
7. Solo con aprobación documental, dividir implementación entre `Rutitas.Domain`, `Rutitas.Application`, `Rutitas.Infrastructure`, `Rutitas.Api` y los proyectos de pruebas.

## Diseño y compatibilidad
- C#/ASP.NET Core 10/MongoDB Atlas/JWT existentes; no migrar tecnología por el Excel de Actividad 4.
- Usar `/api/v1` como prefijo propuesto, sujeto a contrato definitivo.
- Solicitud de contraseña y confirmación deben quedar distinguibles; evitar un contrato ambiguo de un solo paso.
- Mantener interoperabilidad con Android mediante objetos JSON versionados y nombres documentados.
- Al ejecutar futuras tareas, correr las 63 pruebas de línea base y pruebas nuevas en Docker/Testcontainers.

## Dependencias y bloqueos
- Aprobación de alcance por responsable Backend y representante Móvil.
- Política de correo institucional, protección de datos y eliminación; servicio de correo seleccionado con presupuesto aprobado.
- Definición de responsabilidades de recursos asociados al usuario: rutas, viajes, reservas y reportes.
- Azure IaC y CI/CD se especifican después del cierre del catálogo de módulos, por indicación docente.

**Evidencia actual:** 63/63 pruebas del backend existente reportadas por el usuario el 11/11/2026; este dato no verifica las operaciones propuestas aquí.
