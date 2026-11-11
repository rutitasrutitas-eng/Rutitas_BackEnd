# CI/CD — Rutitas Backend (preparación)

**Estado**: la rama `main` revisada el 11 de noviembre de 2026 es documental; no contiene una solución .NET. La preferencia ASP.NET Core 10 / Cloud Run / Firestore es una propuesta, no una decisión aprobada. Hay otras alternativas previas (Azure/MongoDB). No se elige nube ni persistencia en este incremento.

- `.github/workflows/ci.yml`: comprueba documentos SDD y ejecuta Gitleaks. Si detecta una solución/proyecto .NET exige un proyecto de pruebas, ejecuta restore/build/test. Si no existe solución, el build es **NO ejecutado**, aunque SDD pase.
- `.github/workflows/package-release.yml`: empaqueta manualmente el `.csproj` de API que indiques por `workflow_dispatch`. No crea ni despliega infraestructura. Configura environment `backend-release` con revisores.

**Al incorporar backend real**: definir y aprobar stack, poner solución y tests en GitHub, fijar SDK en `global.json`, configurar tests unitarios/integración (p. ej. NoSQL mediante contenedores de pruebas si procede), contrato OpenAPI y análisis SAST. Reajustar detección si el repositorio tiene varias soluciones. Acordar proveedores, OIDC, dos instancias, load balancing, HTTPS, costos, rollback e IaC con Ulises antes de conectar CD productivo. Ningún token, URL privada o cadena de conexión debe quedar versionada.

**CD no terminado** hasta ejecutar despliegue real autorizado, smoke tests, observabilidad y rollback comprobado. Este paquete solo crea los elementos preparatorios para PR.

## Trazabilidad SDD de CI/CD-001

La correspondencia requisito → control → evidencia y sus límites está en [trazabilidad CI/CD-001](trazabilidad-ci-cd.md). Esta automatización es una propuesta local del Backend y queda sujeta a revisión humana; no altera ARCH-BE-01..11 ni sus siete tareas futuras.

El build condicional requiere solución/proyecto .NET y `global.json`; el SDK no se presupone ni se decide por este pipeline. El empaquetado manual es preparatorio y no se ejecuta durante esta revisión.

La etapa inicial también parsea los dos archivos YAML con la biblioteca estándar YAML de Ruby; los resultados se registran en la matriz de trazabilidad.
