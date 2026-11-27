# CONTENT-SAFETY-001

## Estado

- Especificacion: CANDIDATE
- Arquitectura: CANDIDATE
- Implementacion: NOT_IMPLEMENTED
- Integracion Azure: NOT_RUN
- Produccion: NO-GO
- Presupuesto adicional autorizado: 0 MXN

## Objetivo

Preparar un mecanismo centralizado de moderacion
de textos e imagenes para Rutitas.

La aplicacion Android no debe consumir directamente
Azure AI Content Safety ni almacenar sus credenciales.

## Alcance

- Fotografias de perfiles.
- Fotografias de vehiculos.
- Descripciones de usuarios.
- Descripciones de vehiculos.
- Resenas y valoraciones escritas.

## Documentos

- spec.md
- plan.md
- tasks.md
- arquitectura.md
- seguridad.md
- politica-moderacion.md
- matriz-pruebas.md
- trazabilidad.md
- memoria-content-safety.md

## Restricciones

Sin llamadas reales a Azure.
Sin cambios funcionales al Backend o Android.
Sin modificacion de infraestructura.
Sin merge ni despliegue productivo.
