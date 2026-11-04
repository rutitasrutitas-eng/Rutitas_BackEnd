# Propuesta tecnológica para Rutitas Backend — evaluación previa

> **Tipo:** anexo de alternativas para una fase futura, **fuera del alcance de la entrega académica actual** de arquitectura base.
> **Fecha de registro:** 2026-11-04. **Estado:** `propuesto` / `PENDIENTE_APROBACION` / ejecución=`no_iniciado`.
> **Autoridad:** conversaciones del equipo comunicadas por el usuario; no equivalen a ratificación docente, ADR aprobada ni selección tecnológica ejecutiva.
> **Relaciones:** `BACKEND-D04`, `BACKEND-D05`, `BACKEND-D06`, `BACKEND-D07` en [`../decisiones-pendientes.md`](../decisiones-pendientes.md).
> **Regla:** no crear proyecto C#, infraestructura, credenciales ni endpoints por la mera existencia de este documento.

## 1. Restricciones y decisiones diferenciadas

| Asunto | Información disponible | Estado / evidencia requerida |
|---|---|---|
| Paradigma de datos | El usuario comunicó el **requisito expreso del profesor de emplear NoSQL**. | Restricción docente **reportada**; anexar en el futuro la fuente primaria fechada. |
| Proveedor de alojamiento | El usuario indicó que las alternativas permitidas son **Azure, AWS o Google Cloud**. | Restricción docente **reportada**; pendiente de soporte documental primario. |
| Framework Backend | El equipo considera que **ASP.NET Core 10 con C#** es la mejor opción por el ecosistema .NET. | **Preferencia del equipo**, sin ADR/ratificación. |
| Nube | Se prefiere **Google Cloud** por la facilidad de integrar servicios administrados y mapas. | **Preferencia**, comparar costos y responsabilidades frente a Azure/AWS. |
| Base de datos | **Cloud Firestore** (documental NoSQL) es candidata principal. | **Propuesta**, comprobar consultas, índices, costo y concurrencia. |
| Autenticación | **Firebase Authentication** es preferencia inicial frente a implementar identidad propia. | **Alternativa en evaluación**; cambia el alcance de los 7 endpoints propios. |
| Aplicación Móvil | Su stack sigue abierto en el SDD independiente; un prototipo React/TS/Vite/Capacitor no equivale a selección oficial. | No adoptar framework móvil desde este documento. |

No cambiar el estado `PENDIENTE_RATIFICACION` de las Constituciones ni las 7 tareas de arquitectura base (`no_iniciado` / `no_ejecutada`).

## 2. Stack candidato

| Capa | Propuesta | Motivo y condición |
|---|---|---|
| API | **ASP.NET Core 10 / C#** | API REST, configuración por entorno, inyección de dependencias, autenticación/autorización, OpenAPI, pruebas. Verificar SDK y versiones en el momento de aprobar. |
| Estilo | **Monolito modular** | Menor carga de operación para un piloto; separar responsabilidades sin microservicios prematuros. |
| Hospedaje | **Google Cloud Run** | Plataforma administrada para el servicio .NET, sin operar VM propia; revisar región, cuotas, mínimos de instancia, red, imágenes y facturación. |
| Persistencia | **Cloud Firestore — NoSQL** | Colecciones/documentos, índices y transacciones; validar modelo a partir de las consultas y políticas de retención. |
| SDK servidor | **Google.Cloud.Firestore** | Cliente oficial para C#/.NET; emplear identidades administradas/ADC cuando aplique. |
| Identidad | **Firebase Authentication** (a evaluar) | Externalizar registro/login/verificación; el Backend **siempre** valida tokens y aplica permisos institucionales propios. |
| Mapas | **Google Maps Platform**, solo SKUs necesarios | Visualización, puntos de encuentro, geocodificación y rutas si aportan valor. No requiere que el Backend se hospede en Google Cloud. |
| Contrato | **OpenAPI** con HTTPS/JSON | Versionado, errores y comprobación de compatibilidad entre proyectos, una vez autorizada la fase funcional. |
| Calidad | **xUnit** o equivalente; pruebas unitarias/integración/concurrencia | Diseño de evidencias antes de implementar; nada probado aún. |
| CI/CD | GitHub + revisiones por pull request, futuro | Control de cambios y revisión de secretos; no crear pipelines en esta fase. |

### Alternativas permitidas no descartadas

- **Azure:** ASP.NET Core 10 alojado en App Service o Container Apps y NoSQL con Azure Cosmos DB. Buen ajuste al ecosistema Microsoft; hay que comparar precios, límites y operación.
- **AWS:** ASP.NET Core 10 con servicio de contenedores/hosting equivalente y NoSQL con DynamoDB. Flexible, con más decisiones operativas posibles.
- **Google Cloud (preferida):** Cloud Run + Firestore, con servicios Maps consumibles desde cualquier nube. Elegir por costo total, experiencia del equipo y requisitos; no por una supuesta exclusividad de Maps.

## 3. Diseño lógico sugerido — no implementado

```text
Rutitas_App (cliente aún sin stack ratificado)
     |
     | HTTPS + JSON + token autenticado
     v
ASP.NET Core 10 (API REST)
  |-- Frontera HTTP: validación / versión / errores
  |-- Aplicación: casos de uso
  |-- Dominio: viajes, cupos, reservas, permisos
  |-- Infraestructura: Firestore / proveedor identidad / Maps
     |
     +--> Cloud Firestore (NoSQL)
     +--> Firebase Authentication (solo si se aprueba)
     +--> Google Maps Platform (por necesidad concreta)

Hospedaje sugerido del servicio: Google Cloud Run.
```

Se contemplan cuatro responsabilidades lógicas (API, aplicación, dominio, infraestructura). Un proyecto/ensamblado por responsabilidad es una alternativa, **no** un scaffold autorizado ni obligatorio.

## 4. Modelo documental NoSQL candidato

Colecciones principales, **sin crear aún colecciones reales**:

| Colección | Identificador | Datos mínimos ilustrativos | Riesgo o consideración |
|---|---|---|---|
| `usuarios` | `usuarioId` | Nombre, referencia de identidad, universidad, verificación, permisos | Minimización y privacidad; no guardar contraseñas si autenticación es administrada. |
| `vehiculos` | `vehiculoId` | Propietario, capacidad, estado de autorización | Propiedad y vigencia deben verificarse en servidor. |
| `viajes` | `viajeId` | Conductor, vehículo, origen, destino, salida, cupos, estado | Consultas por fechas/puntos/estado; índices compuestos. |
| `reservas` | `reservaId` | Pasajero, viaje, estado, fecha de alta/cancelación | Consistencia transaccional y reservas duplicadas. |
| `puntosEncuentro` | `puntoId` | Punto público, coordenadas, universidad/área | No publicar domicilios privados. |

- Usar identificadores estables para referencias y evaluar desnormalización mínima guiada por consultas reales; Firestore no ofrece JOIN SQL.
- Definir índices a partir de las búsquedas previstas; planear paginación y límites para controlar lecturas.
- Reservar/cancelar debe combinar lectura de estado, validación y actualización de cupo **atómicamente**, con transacciones de Firestore y estrategias de idempotencia contra solicitudes duplicadas.
- Evitar efectos externos (ej. correos, pagos, notificaciones) dentro de funciones de transacción reintentables; diseñarlos como pasos posteriores confirmados.
- Las operaciones con SDK servidor requieren controles de **IAM** y reglas de autorización en la API: las reglas de seguridad de clientes Firestore no reemplazan la seguridad del servidor.
- Una cuenta con correo institucional verificado **no** queda automáticamente habilitada para conducir; distinguir estados de identidad e institución y autorización de conductor/vehículo.

### Prueba crítica futura de reserva (no ejecutada)

Si un viaje tiene **1 asiento**, dos intentos simultáneos deben producir **como máximo 1 reserva confirmada**. Una cancelación repetida no debe sumar dos cupos. Exigir evidencia automatizada antes de activar un piloto real.

## 5. Estrategia de mapas y costos

- Empezar con **puntos públicos predefinidos** y ubicación aproximada; no recolectar GPS continuo en la primera etapa.
- Evaluar uso de Maps SDK para visualización; Places/Autocomplete, Routes y Geocoding solo cuando exista una necesidad del producto y aprobación de presupuesto.
- **Google Maps Platform no es ilimitadamente gratuito**: las cuotas/cargos dependen del SKU y cambian. No hay requisito de alojar la API en Google Cloud para usar Maps Platform.
- Controlar consumo en **Cloud Run, Firestore, Maps, red, almacenamiento de imágenes/artefactos y autenticación**; revisar precios y cuotas antes de habilitar servicios.
- Configurar alertas de presupuesto, límites/cuotas aplicables, restricciones de claves y alertas operativas. **Una alerta de presupuesto no detiene automáticamente los cargos**.
- Usar cuentas de servicio y Secret Manager o equivalente cuando se apruebe; nunca subir llaves JSON, `.env`, contraseñas o tokens a GitHub.

## 6. Decisiones que deben cerrarse

| ID | Decisión | Preguntas mínimas | Autoridad |
|---|---|---|---|
| TD-01 | Ratificar .NET 10/C# | ¿Capacidad del equipo? ¿Soporte/despliegue? ¿Comparativa simple? | Equipo y revisión docente según reglas vigentes |
| TD-02 | Elegir GCP / Cloud Run | ¿Región? ¿Costo estimado? ¿Cuenta y responsables? ¿Comparación con Azure/AWS? | Equipo autorizado |
| TD-03 | Ratificar Firestore NoSQL | ¿Consultas, índices, transacciones y cuotas suficientes? ¿Retención? | Equipo autorizado |
| TD-04 | Identidad | ¿Firebase Auth o autenticación propia? ¿Qué exige el profesor para los endpoints? | Equipo + docente |
| TD-05 | Primer alcance | ¿Cuáles 7 endpoints cuentan como implementados por nosotros? | Equipo + docente |
| TD-06 | Contrato Móvil ↔ Backend | ¿OpenAPI, tokens, errores, versionado y políticas de datos? | Ambos equipos |
| TD-07 | Seguridad y presupuesto | ¿Responsables, cuotas, protección de datos, pruebas de concurrencia? | Equipo autorizado |

**Gatillo de autorización:** registrar decisiones, responsables, fecha y aprobación humana en un ADR/acta futura. Hasta entonces no actualizar `specs/000-arquitectura-base/{spec,plan,task}.md`, no ejecutar tareas, ni crear recursos cloud.

## 7. Fuentes de consulta técnica (no decisiones del equipo)

- Microsoft .NET releases/support: https://learn.microsoft.com/dotnet/core/releases-and-support
- Google Cloud Run con .NET: https://docs.cloud.google.com/run/docs/quickstarts/build-and-deploy/deploy-dotnet-service
- Biblioteca .NET de Firestore: https://docs.cloud.google.com/dotnet/docs/reference/Google.Cloud.Firestore/latest
- Transacciones de Firestore: https://firebase.google.com/docs/firestore/manage-data/transactions
- Verificación de tokens con Firebase Admin: https://firebase.google.com/docs/auth/admin/verify-id-tokens
- Precios de Google Maps Platform: https://mapsplatform.google.com/intl/es-419_mx/pricing/
- OWASP API Security Top 10: https://owasp.org/API-Security/editions/2023/en/0x11-t10/

La documentación oficial deberá revisarse en la fecha de la decisión, especialmente los precios, cuotas, bibliotecas y versiones.
