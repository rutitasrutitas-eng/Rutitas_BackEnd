# Plan — CI/CD-002 Backend

1. Auditar la rama CI/CD-001, main remoto, cambios locales, archivos, código, tests, workflows y decisiones.
2. Respaldar refs/historial en bundle externo y verificar su restauración.
3. Crear `feature/ci-cd-002-backend` desde `feature/ci-cd-001-bootstrap`, sin alterar el PR previo.
4. Agregar Dependency Review con umbral alto, manteniendo SDD/Gitleaks y construcción condicional.
5. Probar workflows y registrar evidencia en PR apilado hacia la rama CI/CD-001.
6. Dejar build real/CD pendientes hasta que existan proyecto, pruebas, stack y aprobaciones.

**Riesgos:** PR base todavía sin merge; el workflow solo revisa cambios de dependencias en PR y no acredita el inventario de dependencias del servicio; GitHub Actions puede omitir build porque no hay `.sln`/`.csproj`.
