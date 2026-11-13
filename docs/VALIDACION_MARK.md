# Validación técnica de la entrega de Mark

**Base examinada:** `feature/sdd-azure-devsecops` en `2cb6e016cf7e012e4b5f07e71ff330e0789694ae` (13/11/2026). **Correcciones:** rama independiente `fix/mark-api-devsecops-review`, PR hacia la rama de Mark. Este informe registra hechos técnicos; la Constitución, las decisiones de stack y la aceptación del contrato móvil siguen pendientes de aprobación humana.

## Alcance observado

La solución `Rutitas.Api/Rutitas.Api.slnx` contiene API, Application, Domain, Infrastructure y cinco proyectos de pruebas: Unit, Integration, Security, Contract y Architecture. Los siete métodos HTTP implementados son `POST /auth/register`, `POST /auth/login`, `POST /auth/logout`, `GET/PATCH /users/me` y `GET /routes`, `GET /routes/{id}`, todos bajo `/api/v1`. No existen operaciones de viajes o reservas en este backend.

## Estado de evidencia

| Control | Estado | Evidencia y límite |
|---|---|---|
| Restore .NET 10 | Probado localmente | Solución restaurada con SDK `10.0.400`; repetir después de la corrección final. |
| Build Release | Probado localmente | Compilación inicial: cero advertencias y cero errores; repetir después de la corrección final. |
| Cinco suites, TRX y cobertura | Validado en CI para `77d3e1e` | [Run 43464562923](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43464562923): 29 Unit + 3 Contract + 2 Architecture + 22 Security + 13 Integration = **69/69 aprobadas, 0 omitidas**. Los tres casos `MongoRepositoryTests` pasaron en Testcontainers. TRX y Cobertura XML de las cinco suites en el [artefacto del run](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43464562923/artifacts/14058880312). Windows Control de aplicaciones bloqueó las pruebas locales (`0x800711C7`) incluso fuera de OneDrive. |
| CI del SHA correctivo | Validado para `77d3e1e`; commit posterior pendiente | El workflow escucha PR hacia `feature/sdd-azure-devsecops` y `push` a la rama correctiva. El PR #4 no disparó inicialmente un run porque el workflow en la rama base sólo filtraba PR hacia `main`; el push sí ejecutó el run citado. El cambio posterior de rate limiting/análisis requiere otro run. |
| Gitleaks | Validado en CI para `77d3e1e` | Escaneo de historial Git completo con imagen fijada por digest; job SDD/Gitleaks exitoso en el run citado. |
| Análisis estático C# | Probado localmente, CI pendiente | `dotnet build Rutitas.Api/Rutitas.Api.slnx -c Release --no-restore -p:EnableNETAnalyzers=true -p:AnalysisMode=All -p:RunAnalyzersDuringBuild=true`: compiló con **172 advertencias** de analizadores y cero errores. El workflow lo ejecuta; las advertencias requieren clasificación, no se afirma que CodeQL/Semgrep estén activos. |
| NuGet | Validado en CI para `77d3e1e`; commit posterior pendiente | `dotnet package list --project Rutitas.Api/Rutitas.Api.slnx --vulnerable --include-transitive --format json` devolvió **0 advisories** en los nueve proyectos, localmente y en el run citado. El [artefacto JSON](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43464562923/artifacts/14056934668) conserva el resultado. Repetir en el SHA final. |
| Docker/Trivy | Pendiente | Docker no está instalado en este equipo. Comandos reproducibles: `docker build -f Rutitas.Api/Rutitas.Api/Dockerfile -t rutitas-api:review Rutitas.Api`; `trivy image --severity HIGH,CRITICAL --exit-code 1 rutitas-api:review`. No se construyó ni publicó imagen. |

## Seguridad y persistencia

| Severidad | Hallazgo y respuesta | Estado |
|---|---|---|
| Alta | JWT de una cuenta inactiva podía leer rutas. La validación ahora comprueba `sub`, existencia y estado activo contra `IUserRepository`, y falla cerrada si la consulta a MongoDB falla. | Corregido en rama de revisión; prueba negativa añadida. |
| Media | Registro ignoraba la propiedad `role` enviada por cliente. Los DTO de registro y login ahora rechazan propiedades ajenas al contrato. | Corregido en rama de revisión; prueba negativa actualizada. |
| Media | La CI no se disparaba para un PR dirigido a la rama de Mark y faltaba `global.json`; por tanto no había evidencia de las cinco suites en el código entregado. | Workflow y SDK corregidos; evidencia CI pendiente. |
| Media | Cobertura faltaba en Contract y Architecture. | Se añadió `coverlet.collector`; ambos generaron Cobertura XML en el run `77d3e1e`, con 0% de líneas de producto ejercitadas por esas suites. |
| Media | El detalle de rutas no tenía rate limiting, aunque el listado sí. | Política `routes` aplicada al detalle y prueba de la solicitud 61; CI del commit posterior pendiente. |
| Pendiente | Los índices `ux_users_email` y `ttl_revoked_tokens_expiration` se crean al iniciar fuera de `Testing`. La cuenta MongoDB necesitará permisos `createIndex`; no se probó en Atlas. | Revisar en un entorno autorizado. |
| Pendiente | `MongoUserRepository.UpdateAsync` reemplaza el documento completo; no está expuesto en los siete endpoints. Vigilar usos futuros para evitar sobrescribir rol/estado por datos de cliente. | Revisión de diseño futura. |

`PasswordHasher<User>` aplica hash de contraseña antes de guardar. El registro exige 12–128 caracteres y el login devuelve un mensaje genérico ante credenciales inválidas. JWT valida firma HS256, emisor, audiencia y tiempo; la revocación usa `jti` y TTL. `GET /users/me` y `PATCH /users/me` derivan el ID de `sub`; PATCH sólo acepta `displayName` y `phone`. Las rutas públicas se filtran en MongoDB por `Status=active` y `Visibility=public`, y la búsqueda usa `Regex.Escape`. Hay rate limiting en registro, login, listado y detalle de rutas. `page`, `pageSize`, `q` e ID de ruta tienen validación. Faltan pruebas completas de fallos MongoDB de todos los repositorios, paginación real con muchas rutas, expiración TTL y permisos mínimos en un entorno de despliegue.

Las pruebas `MongoRepositoryTests` arrancan `mongo:7.0` mediante Testcontainers y crean una base `rutitas_test_<GUID>` por prueba. `TestApiFactory` usa repositorios en memoria y configura una conexión sintética a `127.0.0.1:1` que no se usa. Ninguna prueba necesita Atlas ni credenciales personales. No hay transacción necesaria en los endpoints actuales de lectura, perfil y revocación; futuras reservas sí requieren atomicidad entre cupo y reserva.

### Medición de cobertura del run 43464562923

El artefacto contiene Cobertura XML por proyecto: Integration **53.59%** de líneas y **26.15%** de ramas; Security **31.68%** y **16.82%**; Unit **33.08%** y **23.14%**. Contract y Architecture registran **0% de líneas** porque sus pruebas inspeccionan contrato/ensamblados y no ejercitan líneas de producto bajo el colector. Son mediciones por suite; no se calculó una cobertura combinada ni se estableció umbral aprobado. El próximo run debe renovar estas cifras para el SHA final.

## Pendientes humanos y de integración

- Mark debe revisar el PR correctivo; sólo él controla su PR a `main`.
- Ejecutar CI para el SHA final y registrar resultados, TRX, cobertura, Gitleaks y auditoría de dependencias. Una suite omitida o fallida impide declarar la API validada.
- Acordar stack, contrato API, política de identidad, datos y retención con Backend y Móvil. No existe acta de aprobación en este repositorio.
- Definir permisos de MongoDB, configuración TLS/red, operación de índices y fallos de arranque antes de staging.
- Habilitar SAST y escaneo Docker con versiones confiables, revisar hallazgos por severidad y decidir bloqueos en CI.
- Resolver las diferencias Android de [la matriz](COMPATIBILIDAD_ANDROID.md). No se agregan endpoints de negocio en este PR.
