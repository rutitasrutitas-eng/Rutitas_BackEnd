# Checklist — CI/CD-002 Backend

- [x] Auditoría y respaldo verificable.
- [x] Rama nueva apilada sin cambios a main ni PR #1.
- [x] Workflow YAML revisado; CI valida sintaxis localmente en runner.
- [ ] CI y Gitleaks aprobados en el PR final.
- [ ] Dependency Review ejecutado con graph habilitado y manifiestos reales (no aplica/NO VERIFICADO mientras no exista proyecto).
- [ ] Build y tests de Backend real (bloqueados: no hay .NET source/test project).
- [ ] Aprobación y merge humano del PR.
- [ ] Stack, SDK, nube, NoSQL, secretos, environments y arquitectura aprobados.
- [ ] CD real, smoke tests, observabilidad y rollback (fuera de esta autorización).
