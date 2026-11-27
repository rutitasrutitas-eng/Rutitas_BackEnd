# MAPS-001 - Licenciamiento y restricciones

Fecha de revision: 2026-11-26
Estado: PENDIENTE DE RATIFICACION

## Azure Maps

Los terminos de Microsoft establecen restricciones
sobre reutilizacion de resultados para multiples usuarios,
almacenamiento, combinacion con otros proveedores
y visualizacion en mapas de terceros.

La aplicabilidad exacta depende de los terminos
contractuales correspondientes a la suscripcion.

## Decisiones preventivas

- No persistir resultados de Azure Maps en MongoDB.
- No cachear recorridos ni respuestas de busqueda.
- No almacenar tiles para uso offline.
- No superponer resultados Azure Maps sobre un mapa
  de terceros sin confirmar expresamente el permiso.
- No utilizar resultados Azure Maps para entrenar ML.
- Respetar las atribuciones del proveedor.
- No asumir que la licencia del motor MapLibre
  autoriza cualquier fuente cartografica.

## Alternativas

A. Azure Maps Web SDK con autenticacion autorizada
   y evaluacion de seguridad de WebView.

B. MapLibre Native con fuente cartografica y datos
   expresamente compatibles con sus respectivas licencias.

Ninguna alternativa queda aprobada definitivamente
en este documento.

## Validaciones pendientes

- Ulises: terminos aplicables, cuotas y permisos.
- Ame: compatibilidad tecnica y visualizacion.
- Backend: contrato y tratamiento de datos.
- DaRa: aprobacion de arquitectura definitiva.

## Referencias oficiales

https://www.microsoft.com/licensing/terms/productoffering/MicrosoftAzure/allprograms

https://learn.microsoft.com/es-es/azure/azure-maps/azure-maps-authentication

https://learn.microsoft.com/en-us/azure/azure-maps/rest-api-azure-maps

https://learn.microsoft.com/en-us/azure/azure-maps/how-to-use-best-practices-for-search

https://learn.microsoft.com/en-us/rest/api/maps/route/
