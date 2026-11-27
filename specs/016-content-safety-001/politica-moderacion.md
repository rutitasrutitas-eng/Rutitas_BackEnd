# CONTENT-SAFETY-001 - Politica candidata

Estado: PENDIENTE DE RATIFICACION

## Categorias iniciales

- Hate.
- Sexual.
- Violence.
- SelfHarm.

## Estados internos

### approved

Contenido evaluado y permitido segun politica.

### pending_review

Contenido que requiere revision humana,
resultado ambiguo o evaluacion no disponible.

### rejected

Contenido que supera un umbral de rechazo
aprobado por coordinacion.

## Mapeo de severidad propuesto

Para una escala normalizada de cuatro niveles:

| Severidad | Decision candidata |
|---|---|
| 0 | approved |
| 2 | pending_review |
| 4 | rejected |
| 6 | rejected |

La decision toma el resultado mas restrictivo
de las categorias evaluadas.

No asumir que todas las versiones y modalidades
de Azure devuelven la misma escala.

Confirmar el formato del proveedor y normalizarlo
antes de aplicar cualquier umbral.

Severidad ausente, desconocida o respuesta
incompleta nunca debe producir approved.

## Consideraciones

- Los umbrales no representan politicas aprobadas.
- Contenido educativo o de emergencia puede requerir
  revision humana en lugar de sanciones automaticas.
- La moderacion no garantiza ausencia de contenido danino.
- Evitar discriminacion y sanciones desproporcionadas.
- Definir mecanismos de revision y correccion.

## Restricciones

No eliminar cuentas automaticamente.
No imponer sanciones por una unica clasificacion.
No publicar contenido pendiente o rechazado.
