# 012 — Especificación de infraestructura Azure para Rutitas Backend

**Fecha:** 11/11/2026 · **Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `—` (documentación de infraestructura)  
**Ámbito:** exclusivamente Backend. **Entorno objetivo inicial:** `staging`; no se autoriza producción.

## 1. Objetivo y fuentes internas
Definir una infraestructura reproducible, segura y de costo controlado para publicar la API ASP.NET Core 10 de Rutitas en Azure, manteniendo MongoDB Atlas como servicio de datos externo. Esta especificación **no** es autorización para aprovisionar recursos, gastar dinero, cambiar el código ni desplegar.

Alinear con `constitution.md`, `AGENTS.md`, `specs/001-unit2-api/`, `specs/002-catalogo-funcional/`, `specs/011-cierre-trazabilidad-funcional/` y `docs/`. Los módulos 003–010 son propuestas, no evidencia de endpoints desplegables.

## 2. Estado existente comprobado / limitado
- **Existente en el ZIP revisado:** solución .NET 10, API, Dockerfile multi-etapa con `ASPNETCORE_URLS=http://+:8080`, usuario no root y scripts GitHub Actions de CI y CD manual limitado a build/escaneo/SBOM.
- **Evidencia comunicada por el equipo:** 63 de 63 pruebas locales aprobadas con Docker activo. No es prueba de despliegue Azure ni de seguridad integral.
- **No implementado/verificado en Azure:** contenedor publicado, imagen en registro, entorno Container Apps, OIDC, permisos RBAC, Key Vault, red a Atlas, observabilidad, rollback y costo real.
- **Alcance del piloto:** 7 endpoints existentes. Publicar esta API no habilita reservas, mensajería, nuevos viajes, moderación ni funciones nuevas sin código y pruebas posteriores.

## 3. Arquitectura candidata — NO APROBADA

```text
Móvil Android (una vez integrado y probado)
       | HTTPS + JWT
       v
Azure Container Apps, ingress externo HTTPS / targetPort 8080
       | identidad administrada / configuración segura
       +--> Azure Key Vault (secretos de app)
       +--> Azure Monitor / logs (telemetría sin PII)
       | TLS + red permitida
       v
MongoDB Atlas (rutitas_db existente)

GitHub Actions -- OIDC --> Azure RBAC
       | publicar imagen aprobada
       v
Azure Container Registry -- pull identidad administrada --> Container Apps
```

La elección de **Container Apps** se propone por el Dockerfile ya disponible y escalado de bajo volumen. Comparar formalmente con App Service antes de aprobar. Azure Container Registry, Key Vault, Log Analytics y egreso pueden tener costos separados. **No presuponer cero pesos**.

## 4. Requisitos verificables

| ID | Requisito | Criterio de aceptación / evidencia |
|---|---|---|
| AZ-01 | Entorno `staging` independiente de producción | Inventario y nombre de recursos por ambiente aprobados; ninguna credencial de producción reutilizada |
| AZ-02 | IaC declarativa y versionada | Bicep o Terraform elegido por ADR, validación/lint y `what-if`/plan revisados antes de cualquier apply |
| AZ-03 | API servida por HTTPS | Ingress externo HTTPS, target port 8080, aplicación escucha en `0.0.0.0:8080`; API responde a solicitudes esperadas |
| AZ-04 | Imagen reproducible | Build con contexto `Backend/Rutitas.Api`, Dockerfile correcto, imagen versionada por SHA, análisis de vulnerabilidades y SBOM |
| AZ-05 | Secretos fuera del repositorio | JWT signing key y cadena MongoDB solo en almacén/variables secretas autorizadas; no aparecen en logs ni artifacts |
| AZ-06 | Mínimo privilegio | Identidades de ejecución y despliegue separadas; RBAC restringido al entorno específico; sin credenciales permanentes en Actions |
| AZ-07 | Conexión segura a Atlas | TLS, autenticación de Atlas, salida de red controlada y regla IP/PrivateLink aprobada con costo estimado; no abrir `0.0.0.0/0` como solución definitiva |
| AZ-08 | Protección de datos | Logs sin JWT, contraseñas, documentos, ubicación precisa ni cadenas de conexión; política de retención definida |
| AZ-09 | Disponibilidad observable | Probes startup/readiness/liveness configuradas y verificadas, métricas y alertas para errores y reinicios |
| AZ-10 | Control de costos | Presupuesto mensual, alertas y estimación por región, SKU, registro, Key Vault, logs, red y salida de datos aprobados |
| AZ-11 | Despliegue reversible | Release inmutable + procedimiento probado de retorno a revisión anterior; migraciones de datos se controlan por separado |
| AZ-12 | Gate humano | Evidencia de autorización explícita para Azure, gasto, red, secretos y staging antes de ejecutar apply/deploy |
| AZ-13 | Configuración .NET correcta | Verificar claves `MongoDb__ConnectionString`, `MongoDb__DatabaseName`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SigningKey`; no dejar valores de Development en staging |
| AZ-14 | Health y startup realistas | Revisar el comportamiento de inicialización de índices Mongo y dependencias antes de seleccionar sondas; no confundir apertura de puerto con disponibilidad DB |
| AZ-15 | Alcance API | Smoke tests de los 7 endpoints existentes con usuarios sintéticos, incluyendo rechazos 401/403/404 sin publicar datos personales |

## 5. Decisiones abiertas bloqueantes
1. **Suscripción, responsable financiero y presupuesto máximo**: sin evidencia de disponibilidad ni autorización.
2. **Región Azure**: comparar costo, latencia a Atlas, residencia y disponibilidad de SKUs; no elegir una región por inferencia.
3. **Servicio:** Container Apps vs App Service y justificación de costo/operación.
4. **Registro de imágenes:** ACR vs opción aprobada, considerando autenticación, costos y cuotas.
5. **Red de salida a MongoDB Atlas:** Atlas IP Access List requiere origen de salida confiable; NAT Gateway, IP pública fija, conectividad privada o alternativa viable deben evaluarse y costearse. IP dinámica no garantiza una allowlist segura.
6. **Key Vault:** mecanismo de referencia a secreto, identidades y permisos; renovación de JWT y credenciales Atlas.
7. **Plan de escalado:** instancias mínimas 0 o 1 con justificación por latencia de arranque y costos, límites de CPU/memoria y rate limits.
8. **Dominio público:** URL gratuita del servicio para staging; dominio propio opcional posterior.
9. **Gobernanza:** quién aprueba infraestructura, acceso, secretos, despliegues, rollback y gasto.

## 6. Límites y seguridad
- No colocar secrets en `appsettings.json`, `Dockerfile`, archivos `.http` versionados, Bicep/Terraform ni variables visibles de GitHub.
- No publicar endpoints de prueba ni OpenAPI de desarrollo indiscriminadamente en staging.
- No utilizar conexiones Atlas abiertas globalmente como política de producción.
- No tratar la asignación gratuita de Container Apps como gratuidad de otros componentes.
- No hacer despliegue automático a producción al fusionar una rama.
- El certificado HTTPS terminado en el proxy no elimina la obligación de validar correctamente encabezados forwarded, origen, auth y pruebas end-to-end.

## 7. Criterios para aprobar esta spec
Revisión del líder, Backend, Seguridad/Privacidad y contraparte Móvil; ADRs cerradas, presupuesto costeado, autorización explícita; matriz de trazabilidad AZ-01..AZ-15 y `plan.md` consistente. Hasta entonces conservar `decisión=propuesto`.

## 8. Referencias técnicas consultadas (11/11/2026)
- [Facturación Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/billing)
- [OIDC para GitHub Actions](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect)
- [Sondas de salud](https://learn.microsoft.com/en-us/azure/container-apps/health-probes)
- [Target port](https://learn.microsoft.com/en-us/azure/container-apps/troubleshoot-target-port-settings)
