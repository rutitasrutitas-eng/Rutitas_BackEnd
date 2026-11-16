# 012 — Alternativas y decisiones de arquitectura/costos

## Diseño candidato (no aprovisionado)
- Compute: **Azure Container Apps Consumption** para API Docker .NET 10.
- Imágenes: **Azure Container Registry** solo si el presupuesto lo permite; examinar si otro registro soportado/aprobado reduce costos.
- Secretos: **Azure Key Vault** con identidad administrada y control RBAC.
- Logs/métricas: Azure Monitor y volumen de ingesta/retención limitado.
- Datos: MongoDB Atlas **existente**; no migrar a Cosmos DB en esta fase.
- Despliegue: GitHub Actions → Microsoft Entra federación OIDC → RBAC mínimo, con gate `staging`.

## Estimación que DEBE completarse antes de aprobar

| Elemento | Parámetros a cotizar | Responsable | Estado |
|---|---|---|---|
| Container Apps | Región, vCPU, GiB, segundos activos, peticiones, escala mínima/máxima | Backend | Pendiente |
| Registry | SKU, almacenamiento de imágenes, tráfico y retención | DevSecOps | Pendiente |
| Key Vault | Operaciones/rotaciones y modelo de acceso | Seguridad | Pendiente |
| Logs/Monitor | Ingesta diaria, retención, alertas | Operaciones | Pendiente |
| Red a Atlas | Egress, NAT/IP fija o PrivateLink, transferencia | Seguridad/Backend | **Bloqueante** |
| Atlas | Plan actual, cuota, red, backups y eventual upgrade | Responsable Atlas | Pendiente |
| Contingencia | Margen de gastos inesperados y alertas en suscripción | Líder | Pendiente |

La capa gratuita del plan Consumption tiene cuotas mensuales *de ese servicio*, pero no supone registro, secretos, IP fija, logs ni Atlas sin costo. En el momento de la consulta Microsoft documenta 180 000 vCPU-segundos, 360 000 GiB-segundos y 2 millones de solicitudes al mes por suscripción; confirmar precio/condiciones por región antes de decidir. https://learn.microsoft.com/en-us/azure/container-apps/billing

## Riesgos técnicos para resolver
1. **Salida Atlas**: no confiar en egress dinámico sin una estrategia de allowlist; prohibido dar por aprobada la apertura global `0.0.0.0/0`.
2. **Arranque y health checks**: Container Apps usa puertos de ingress en sus pruebas predeterminadas. El Dockerfile escucha en `8080`; documentar readiness/health real. https://learn.microsoft.com/en-us/azure/container-apps/health-probes
3. **Seguridad JWT**: firmar con clave real desde almacén, evitar tokens en logs y validar hora/issuer/audience; valorar rotación y expiración.
4. **Móvil**: no introducir URL permanente en APK hasta confirmar ambiente, estabilidad del contrato y HTTPS; `localhost` de Android nunca referencia Azure.
5. **Escalado**: múltiples réplicas requieren que estado de revocación y reservas se conserve en base compartida, no en memoria.

## Decisiones por registrar
- ADR-AZ-01: ACA vs App Service.
- ADR-AZ-02: región y presupuesto.
- ADR-AZ-03: egress/allowlist Atlas.
- ADR-AZ-04: ACR, identidades, RBAC y Key Vault.
- ADR-AZ-05: control de revisiones y rollback.

**Ningún ADR se considera aprobado por incluirse en esta propuesta.**
