# MAPS-001 - Seguridad y control de consumo

Estado: PROPUESTA

## Controles requeridos

- JWT valido y autorizacion en los cuatro endpoints.
- Validacion estricta de coordenadas.
- Limites de longitud para consultas.
- Limites para waypoints y cantidad de resultados.
- HttpClient con timeout y CancellationToken.
- ProblemDetails sin informacion interna.
- Rate limiting por usuario autenticado.
- Limite global de solicitudes.
- Restriccion de destino HTTP a Azure Maps autorizado.
- Prevencion de SSRF mediante endpoint fijo.
- TLS obligatorio para llamadas al proveedor.
- No registrar tokens ni ubicaciones precisas.
- No incorporar claves en Android.
- Cache de resultados deshabilitada inicialmente.

## Valores iniciales candidatos

- Timeout upstream: 8 segundos.
- Busquedas: 10 solicitudes por minuto por usuario.
- Recorridos: 5 solicitudes por minuto por usuario.
- Maximo de waypoints: 8.
- Maximo de resultados: 10.
- Reintentos automaticos deshabilitados inicialmente.

Estos valores requieren aprobacion y pruebas de consumo.
No representan cuotas oficiales de Azure.

## Presupuesto

Presupuesto adicional autorizado: 0 MXN.

El proveedor real permanecera deshabilitado.
Los limites de consumo se definiran despues de conocer
la cuota gratuita efectiva.

No confiar exclusivamente en alertas presupuestarias:
pueden no detener automaticamente el consumo.

## Indisponibilidad

- 429: limite alcanzado.
- 502: respuesta upstream invalida.
- 503: dependencia no disponible.
- 504: timeout upstream.

Android no debe inventar distancias o recorridos.
Las respuestas deben indicar que no fue posible
obtener informacion geoespacial actualizada.

## Privacidad

Minimizar el tratamiento de ubicacion.
No implementar seguimiento permanente.
No compartir coordenadas con sistemas no autorizados.
No usar resultados para entrenamiento de IA.
