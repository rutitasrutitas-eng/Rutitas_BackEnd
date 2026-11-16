# 012 — Plan de infraestructura Azure (propuesta sin ejecución)

**Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `—`.

## Estrategia
Mantener la API ASP.NET Core 10 y MongoDB Atlas existentes. El primer despliegue futuro sería **staging**, limitado a siete endpoints actuales. Infraestructura en una sola tecnología declarativa (**Bicep recomendado como candidato** por integración nativa con Azure, no decidido) y componentes mínimos necesarios, con control de costos. CI/CD debe definirse en `013-cicd-devsecops`, sin mezclar despliegue con la revisión de arquitectura.

## Etapas y entregables

| Etapa | Entregable documental | Revisión/gate |
|---|---|---|
| A. Baseline | Inventario de Dockerfile, Program.cs, appsettings, variables, CI y CD actual | Verificar diferencias respecto de specs/001; sin cambiar código |
| B. ADR de plataforma | Comparación Container Apps / App Service; región; recursos y límites; escala a cero | Líder y técnico aprueban costo y operación |
| C. Datos y conectividad | Diagrama de flujo Atlas, salida IP fija o conexión privada, TLS, alternativa y costo | Seguridad/Privacidad y dueño de Atlas |
| D. Identidades y secretos | Diagrama GitHub OIDC → identidad de despliegue; Container Apps → identidad de ejecución → ACR/Key Vault | Separación de permisos, sin secretos persistentes |
| E. Costo | Tabla por recurso y escenarios inactivo/uso esperado/pico; presupuesto y alertas | Responsable financiero aprueba por escrito |
| F. IaC | Estructura propuesta `infra/azure/{main.bicep,modules/,parameters/}` o Terraform si ADR lo decide | Solo generar código **después de aprobar la spec**; revisión `what-if` antes de aplicar |
| G. Operabilidad | Health probes, logs sin PII, retención, métricas, avisos, imagen con SHA y rollback | Seguridad y operaciones |
| H. Pruebas staging | Verificación TLS, auth 401, rutas 200/404, índice Mongo, reinicios, recuperación, acceso móvil | Evidencia + aceptación; solo tras autorización de despliegue |

## Configuración propuesta para la futura implementación
- Docker build: `docker build -f Rutitas.Api/Rutitas.Api/Dockerfile Rutitas.Api` ejecutado desde `Backend/`.
- En contenedor: `ASPNETCORE_URLS=http://+:8080`; ingress externo HTTPS target port `8080`.
- Environment vars configurables (sin valores reales): `ASPNETCORE_ENVIRONMENT=Staging`, `MongoDb__ConnectionString`, `MongoDb__DatabaseName`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SigningKey`. **Validar nombre/encoding real de Jwt__SigningKey contra Program.cs** antes de activar.
- GitHub Actions: federación OIDC y entornos protegidos; deploy por digest/commit SHA, con aprobación manual en staging.
- Sondas: empezar con TCP o endpoint de salud explícito si se aprueba agregarlo; **no asumir** `/health` implementado.

## Dependencias y riesgos
- Los siete endpoints existentes son el único alcance ejecutable validado localmente.
- Atlas puede bloquear conexiones por IP de salida de Azure; soluciones de IP fija o conectividad privada pueden exceder un presupuesto gratuito.
- Escala a cero ahorra recursos pero agrega arranque en frío; el comportamiento de inicio de Mongo debe probarse.
- Escaneo de imagen no sustituye pruebas de red, privacidad ni revisión de dependencias.
- Microsoft puede cambiar precios, regiones y cuotas: revalidar al aprobar.

## Estrategia de reversión
Mantener la última imagen aprobada y una revisión previa de Container Apps; en caso de fallo, redirigir tráfico a revisión sana según procedimiento documentado. Reversiones de schema/documentos Mongo requieren su propio plan, no asumirse reversibles automáticamente.

## Condiciones para pasar a ejecución
`spec.md` y ADR aprobados, permisos otorgados, responsables identificados, suscripción y tope de gasto confirmados, datos sintéticos, conectividad Atlas resuelta, IaC revisada y autorización humana de staging. Sin ello, tareas en `no_iniciado`.
