# Lista de comprobación antes de publicar el Backend en GitHub

- [ ] Tengo permiso `write` para `rutitasrutitas-eng/Rutitas_BackEnd` desde la cuenta de Git usada por Visual Studio.
- [ ] Existe una copia de seguridad de la carpeta `Backend` actual.
- [ ] Cloné el repositorio oficial, conservé su historial y preparé una rama `feature/*`.
- [ ] Confirmé qué directorio es la raíz esperada del proyecto en el repositorio remoto (evitar `Backend/Backend/`).
- [ ] No copiaré `.git` de otra carpeta ni archivos de compilación.
- [ ] Excluí `*.local.http`, `.env`, archivos locales, claves, certificados y tokens.
- [ ] Revisé TODO el contenido a publicar, incluso `*.http`, `*.md`, YAML, scripts y configuraciones de pruebas.
- [ ] Revisé `git diff --cached --name-only` y `git diff --cached --stat`.
- [ ] Confirmé que el workflow de CD sigue SIN desplegar a Azure.
- [ ] Registraré URL de la rama/PR y no fusionaré sin revisión aprobada.

## Decisión de seguridad
Si se encuentra una credencial que ha aparecido en un ZIP, repositorio o conversación, **revocarla o rotarla** antes del uso; simplemente eliminarla del commit nuevo no garantiza que siga siendo segura.

## Comandos útiles (PowerShell)
```powershell
git status --short
git remote -v
git branch --show-current
git diff --cached --name-only
git diff --cached --stat
git ls-files | Select-String -Pattern '(^|/)(bin|obj|\.vs|TestResults)/|\.local\.http$|(^|/)\.env'
```
No interpretar un resultado vacío como auditoría completa de secretos. Antes de `push`, usar también Gitleaks u otro escáner sobre los archivos reales que se publicarán.
