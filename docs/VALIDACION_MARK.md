# Validación técnica de la entrega de Mark

## Actualización de conflictos y advertencias (13/11/2026)

Esta sección es la evidencia más reciente y reemplaza para el estado actual los SHA y resultados históricos descritos más abajo; no borra su registro. Se creó la referencia local `backup/fix-mark-api-devsecops-review-before-merge-2026-11-13` antes de integrar cambios. La rama local comenzó en `84e9564733a03323cefe1f683d5f795521fd5ac0` y recibió mediante merge normal el HEAD vigente de Mark `0f05c07d099c8a4bb396719d804ec0698a29254a`. El merge quedó como `5c2824e` (primer padre 84e9564, segundo padre 0f05c07); el commit de correcciones de código es `c08f998`. No se modificó `main` ni la rama de Mark. La documentación de resultados y el push del PR están pendientes.

| Control | Resultado local comprobado | Alcance / límite |
|---|---|---|
| Conflictos | Resueltos en `.github/workflows/ci.yml`, `global.json`, `Rutitas.ArchitectureTests.csproj` y `Rutitas.ContractTests.csproj`. | Se conservaron los cinco proyectos de test, Gitleaks, auditoría NuGet, análisis C#, TRX y Cobertura. Se mantuvo una referencia por paquete. |
| SDK | `global.json` fija SDK `10.0.400`, presente localmente; `rollForward: disable`. | No se usa 10.0.401 porque no está instalado/verificado en este entorno. |
| Restore | `dotnet restore Rutitas.Api/Rutitas.Api.slnx -p:NuGetAudit=true -p:NuGetAuditMode=all` terminó correctamente. | Sin credenciales de MongoDB Atlas. |
| Build Release + análisis completo | Exit 0, 0 errores, 186 advertencias reportadas por MSBuild. | Inventario normalizado por archivo/regla/ubicación en [ADVERTENCIAS_ANALISIS_CSHARP.md](ADVERTENCIAS_ANALISIS_CSHARP.md). CodeQL y Trivy no están instalados; Docker tampoco. |
| Suites Architecture / Contract / Security / Unit | 2/2, 3/3, 23/23 y 29/29 aprobadas. | Resultado de la ejecución local posterior a los cambios correctivos. |
| Integration | 10/13 aprobadas; 3/13 fallaron al iniciar Testcontainers. | Bloqueo de infraestructura local: Docker no está instalado/en ejecución (`DockerUnavailableException`, pipe `docker_engine`). No se atribuye a fallo de la prueba ni se afirma suite completa aprobada. |
| Conteo local | 67 aprobadas, 3 bloqueadas por falta de Docker, 70 en total. | GitHub Actions debe correr en Linux con Docker para confirmar las tres pruebas Mongo. |
| Gitleaks | Gitleaks 8.30.0 local, escaneo del historial de 18 commits: sin hallazgos. | Falta comprobar el job remoto del SHA actualizado. |
| Auditoría NuGet | 0 advisories en auditoría transitive de la solución. | Falta comprobar el job remoto del SHA actualizado. |
| PR / Actions | PR #4 ya existente, abierto hacia `feature/sdd-azure-devsecops`; no se hizo merge. | El nuevo SHA aún no se ha publicado, por lo que no existe run CI asociado a esta actualización. Los runs anteriores no validan el merge nuevo. |

### Conflictos y regresiones de seguridad revisadas

Se conservaron las mejoras de la rama correctiva cuando el nuevo commit de Mark retiraba comprobaciones de cuenta activa y revocación, el rechazo de propiedades de rol no permitidas en registro, la prueba de fallo cerrado ante la caída del almacén de revocación y la prueba de rate limit en el detalle de ruta. Se verificó que las pruebas correspondientes siguen presentes; **Mark debe revisar este resultado** para confirmar su intención antes de aprobar el PR. Se conservaron también las referencias `coverlet.collector` únicas de Architecture y Contract.

### Correcciones de advertencias realizadas

- `CA1062`: se añadieron guardas de argumentos a acciones de `AuthController` y a constructores/métodos de los repositorios Mongo afectados. Las advertencias que quedan están en test fixtures y pruebas.
- `CA2000`: se liberan mediante `using` los `HttpContent` de tres pruebas de regresión.
- `CA1031`: permanecen cinco ubicaciones en `AuthController` y `Program.cs`; requieren validación contextual humana. No se suprimieron ni se reemplazaron globalmente.
- `CA1308`: se dejó intacta la normalización de correo por falta de una prueba de regresión específica.
- `CA2007`, `CA1707` y `CA2234`: siguen inventariadas; predominan ubicaciones de pruebas y no se aplicó una corrección masiva.

### GitHub Actions y estado remoto del commit `f03bdf2`

- Head publicado: `f03bdf2c1ebd50f8d830c9eeaf788e084d87837d`.
- Run [43537627859](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43537627859): **success**, con 3 jobs aprobados: detección .NET, SDD/YAML/Gitleaks y .NET 10 (restore, build, análisis, NuGet, cinco suites y artefactos).
- Suites de CI: Architecture 2/2, Contract 3/3, Integration 13/13, Security 23/23 y Unit 29/29 = **70/70**, 0 fallidas y 0 omitidas. Las pruebas Mongo/Testcontainers sí corrieron en el runner Linux.
- Gitleaks de historial completo: success. NuGet transitive audit: **0 advisories**. YAML, validación SDD, restore, build normal, análisis C# y publicación de artefactos TRX/Cobertura/JSON: success.
- Análisis `AnalysisMode=All`: 186 advertencias, 0 errores; inventario en [ADVERTENCIAS_ANALISIS_CSHARP.md](ADVERTENCIAS_ANALISIS_CSHARP.md). Las cifras locales de test difieren solo por falta de Docker en Windows; CI es la verificación integral.
- Artefactos [TRX y Cobertura](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43537627859/artifacts/14090023278) y [auditoría NuGet](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43537627859/artifacts/14090457341).
- [Run 43538064486](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43538064486), para el commit documental `e6ad72b61e23e63e849e20d850204d52cd9ddc9b`: los 3 jobs también terminaron en **success**, incluyendo la ejecución de las cinco suites y publicación de artefactos.
- [Run 43538524666](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43538524666), para SHA `9d60ab2c21fc52ea43addc64273b2df21594f3b0`: 3/3 jobs success; YAML/SDD/Gitleaks, restore, build, análisis, NuGet y las cinco suites pasaron. Conteo verificado en el log: Architecture 2, Contract 3, Integration 13, Security 23, Unit 29 = **70/70**, 0 fallidas y 0 omitidas. Build normal 0 warnings/0 errores; análisis completo 186 warnings/0 errores; NuGet 0 advisories.
- GitHub confirmó el PR #4 **abierto, sin fusionar y mergeable** contra `feature/sdd-azure-devsecops`, cuyo SHA continúa `0f05c07d099c8a4bb396719d804ec0698a29254a`. El SHA de merge de prueba generado por GitHub no se ha integrado en la rama destino.

Se intentó actualizar el cuerpo del PR #4 y, por separado, agregar la evidencia como comentario; ambas operaciones de escritura del conector GitHub fueron rechazadas con HTTP 403 `Resource not accessible by integration`. El cuerpo/comentarios del PR no se modificaron desde esta sesión; los cambios de código y documentación sí están en la rama publicada. No se intentó eludir el rechazo. Mark aún debe revisar y aprobar el mantenimiento de las comprobaciones de seguridad indicadas arriba. CodeQL y Trivy siguen pendientes/no disponibles localmente; no se desplegó a Azure ni se modificó `main`.

**Base examinada:** `feature/sdd-azure-devsecops` en `2cb6e016cf7e012e4b5f07e71ff330e0789694ae` (13/11/2026). **Correcciones:** rama independiente `fix/mark-api-devsecops-review`, PR hacia la rama de Mark. Este informe registra hechos técnicos; la Constitución, las decisiones de stack y la aceptación del contrato móvil siguen pendientes de aprobación humana.

## Alcance observado

La solución `Rutitas.Api/Rutitas.Api.slnx` contiene API, Application, Domain, Infrastructure y cinco proyectos de pruebas: Unit, Integration, Security, Contract y Architecture. Los siete métodos HTTP implementados son `POST /auth/register`, `POST /auth/login`, `POST /auth/logout`, `GET/PATCH /users/me` y `GET /routes`, `GET /routes/{id}`, todos bajo `/api/v1`. No existen operaciones de viajes o reservas en este backend.

## Estado de evidencia

| Control | Estado | Evidencia y límite |
|---|---|---|
| Restore .NET 10 | Probado localmente y validado en CI | Solución restaurada con SDK `10.0.400`; run final del código `c5653dc` exitoso. |
| Build Release | Probado localmente y validado en CI | Compilación normal del run `43465415801`: cero advertencias y cero errores. |
| Cinco suites, TRX y cobertura | Validado en CI para `c5653dc` | [Run 43465415801](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43465415801): 29 Unit + 3 Contract + 2 Architecture + 23 Security + 13 Integration = **70/70 aprobadas, 0 omitidas**. Los tres casos `MongoRepositoryTests` pasaron en Testcontainers. TRX y Cobertura XML de las cinco suites en el [artefacto del run](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43465415801/artifacts/14058866194). Windows Control de aplicaciones bloqueó las pruebas locales (`0x800711C7`) incluso fuera de OneDrive. |
| CI del SHA correctivo | Validado en CI para `c5653dc` | Los tres jobs terminaron exitosamente. El workflow escucha PR hacia `feature/sdd-azure-devsecops` y `push` a la rama correctiva. La primera creación del PR no disparó inmediatamente un run porque el workflow de la rama base sólo filtraba PR hacia `main`; el push sí aportó evidencia del SHA revisado. |
| Gitleaks | Validado en CI para `c5653dc` | Escaneo de historial Git completo con imagen fijada por digest; job SDD/Gitleaks exitoso. |
| Análisis estático C# | Probado localmente y validado en CI | `dotnet build Rutitas.Api/Rutitas.Api.slnx -c Release --no-restore --no-incremental -p:EnableNETAnalyzers=true -p:AnalysisMode=All -p:RunAnalyzersDuringBuild=true`: en CI compiló con **198 advertencias** de calidad/API y cero errores. Requieren clasificación; CodeQL/Semgrep no se ejecutaron. |
| NuGet | Probado localmente y validado en CI | `dotnet package list --project Rutitas.Api/Rutitas.Api.slnx --vulnerable --include-transitive --format json` devolvió **0 advisories** en los nueve proyectos. El [artefacto JSON](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43465415801/artifacts/14058911228) conserva el resultado. |
| Docker/Trivy | Pendiente | Docker no está instalado en este equipo. Comandos reproducibles: `docker build -f Rutitas.Api/Rutitas.Api/Dockerfile -t rutitas-api:review Rutitas.Api`; `trivy image --severity HIGH,CRITICAL --exit-code 1 rutitas-api:review`. No se construyó ni publicó imagen. |

## Seguridad y persistencia

| Severidad | Hallazgo y respuesta | Estado |
|---|---|---|
| Alta | JWT de una cuenta inactiva podía leer rutas. La validación ahora comprueba `sub`, existencia y estado activo contra `IUserRepository`, y falla cerrada si la consulta a MongoDB falla. | Corregido en rama de revisión; prueba negativa añadida. |
| Media | Registro ignoraba la propiedad `role` enviada por cliente. Los DTO de registro y login ahora rechazan propiedades ajenas al contrato. | Corregido en rama de revisión; prueba negativa actualizada. |
| Media | La CI no se disparaba para un PR dirigido a la rama de Mark y faltaba `global.json`; por tanto no había evidencia de las cinco suites en el código entregado. | Workflow y SDK corregidos; CI del código correctivo validado. |
| Media | Cobertura faltaba en Contract y Architecture. | Se añadió `coverlet.collector`; ambos generaron Cobertura XML en el run `43465415801`, con 0% de líneas de producto ejercitadas por esas suites. |
| Media | El detalle de rutas no tenía rate limiting, aunque el listado sí. | Política `routes` aplicada al detalle y prueba de la solicitud 61; CI del código correctivo validado. |
| Pendiente | Los índices `ux_users_email` y `ttl_revoked_tokens_expiration` se crean al iniciar fuera de `Testing`. La cuenta MongoDB necesitará permisos `createIndex`; no se probó en Atlas. | Revisar en un entorno autorizado. |
| Pendiente | `MongoUserRepository.UpdateAsync` reemplaza el documento completo; no está expuesto en los siete endpoints. Vigilar usos futuros para evitar sobrescribir rol/estado por datos de cliente. | Revisión de diseño futura. |

`PasswordHasher<User>` aplica hash de contraseña antes de guardar. El registro exige 12–128 caracteres y el login devuelve un mensaje genérico ante credenciales inválidas. JWT valida firma HS256, emisor, audiencia y tiempo; la revocación usa `jti` y TTL. `GET /users/me` y `PATCH /users/me` derivan el ID de `sub`; PATCH sólo acepta `displayName` y `phone`. Las rutas públicas se filtran en MongoDB por `Status=active` y `Visibility=public`, y la búsqueda usa `Regex.Escape`. Hay rate limiting en registro, login, listado y detalle de rutas. `page`, `pageSize`, `q` e ID de ruta tienen validación. Faltan pruebas completas de fallos MongoDB de todos los repositorios, paginación real con muchas rutas, expiración TTL y permisos mínimos en un entorno de despliegue.

Las pruebas `MongoRepositoryTests` arrancan `mongo:7.0` mediante Testcontainers y crean una base `rutitas_test_<GUID>` por prueba. `TestApiFactory` usa repositorios en memoria y configura una conexión sintética a `127.0.0.1:1` que no se usa. Ninguna prueba necesita Atlas ni credenciales personales. No hay transacción necesaria en los endpoints actuales de lectura, perfil y revocación; futuras reservas sí requieren atomicidad entre cupo y reserva.

### Medición de cobertura del run 43465415801

El artefacto contiene Cobertura XML por proyecto: Integration **53.59%** de líneas y **26.15%** de ramas; Security **34.93%** y **17.43%**; Unit **33.08%** y **23.14%**. Contract y Architecture registran **0% de líneas** porque sus pruebas inspeccionan contrato/ensamblados y no ejercitan líneas de producto bajo el colector. Son mediciones por suite; no se calculó una cobertura combinada ni se estableció umbral aprobado.

## Pendientes humanos y de integración

- Mark debe revisar el PR correctivo; sólo él controla su PR a `main`.
- Mantener CI en verde tras cualquier nuevo cambio de código. Una suite omitida o fallida impide declarar la API validada.
- Acordar stack, contrato API, política de identidad, datos y retención con Backend y Móvil. No existe acta de aprobación en este repositorio.
- Definir permisos de MongoDB, configuración TLS/red, operación de índices y fallos de arranque antes de staging.
- Habilitar SAST y escaneo Docker con versiones confiables, revisar hallazgos por severidad y decidir bloqueos en CI.
- Resolver las diferencias Android de [la matriz](COMPATIBILIDAD_ANDROID.md). No se agregan endpoints de negocio en este PR.
