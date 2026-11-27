# BACKEND-DELETE-001 — Inventario de datos

Estado: en_revision
Ejecucion: no_iniciado
Verificacion: no_ejecutada
Fecha: 2026-11-18

## 1. Colecciones implementadas en Fase A

### users
Datos: Id, Email, PasswordHash, DisplayName,
Phone, Role, Status, CreatedAt, UpdatedAt,
LastLoginAt.

Tratamiento propuesto:
- Suspender acceso durante los 7 dias.
- Eliminar credenciales y datos personales al vencer.
- Verificar ausencia de datos al finalizar.
- No declarar eliminacion antes de completar el proceso.

### routes
Datos: Id, Code, Name, Description, Origin,
Destination, Stops, DistanceKm,
EstimatedDurationMin, Status, Visibility,
CreatedAt, UpdatedAt.

Hallazgo:
- No existe relacion de propiedad con UserId.
- No eliminar rutas publicas de forma indiscriminada.
- Revisar ubicaciones o textos con datos personales.
- Definir reglas para futuras rutas de usuarios.

### revoked_tokens
Datos: Jti, ExpiresAt, RevokedAt.

Hallazgo:
- No existe UserId en el documento.
- No se pueden eliminar por propietario directamente.
- Conservar solo el tiempo necesario para seguridad.
- Revisar retencion y expiracion en el diseño.

## 2. Modulos futuros no implementados

Reservas, viajes, mensajes, incidencias, fotografias,
documentos y otras relaciones personales.

No declarar su eliminacion implementada.

Antes de habilitar esos modulos, definir relaciones de
propiedad, dependencias y politica de borrado.

## 3. Riesgos y decisiones pendientes

DEL-D01: tratamiento de datos compartidos.
DEL-D02: politica legal de conservacion.
DEL-D03: tratamiento de respaldos.
DEL-D04: invalidacion de sesiones y recuperacion.
DEL-D05: concurrencia, fallos parciales y reintentos.
DEL-D06: inventario de futuros modulos.

Todas las decisiones permanecen en revision.

## 4. Evidencias

Fuente:
- Rutitas.Domain/Entities/User.cs
- Rutitas.Domain/Entities/Route.cs
- MongoUserRepository.cs
- MongoRouteRepository.cs
- MongoTokenRevocationRepository.cs

No se realizaron borrados ni modificaciones de datos.
