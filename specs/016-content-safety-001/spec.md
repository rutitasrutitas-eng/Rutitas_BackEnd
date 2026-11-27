# CONTENT-SAFETY-001 - Especificacion

Estado: CANDIDATE
Fecha: 2026-11-26

## Problema

Rutitas requiere prevenir la publicacion de contenido
inapropiado y proteger a su comunidad universitaria.

Las decisiones de moderacion no deben depender de
validaciones realizadas exclusivamente por Android.

## Requisitos funcionales

### RF-CS-01 - Moderacion de texto

Evaluar contenido textual antes de su publicacion.

Contextos iniciales:

- profile_description
- vehicle_description
- review

### RF-CS-02 - Moderacion de imagen

Evaluar imagenes de perfil y vehiculos.

Contextos iniciales:

- profile_photo
- vehicle_photo

Validar formato, contenido, dimensiones y tamano
antes de enviar al proveedor autorizado.

### RF-CS-03 - Estados

- approved: contenido permitido por la politica.
- rejected: contenido rechazado por la politica.
- pending_review: requiere revision o nueva evaluacion.

### RF-CS-04 - Politica centralizada

El Backend interpreta el resultado del proveedor.
Android no puede decidir unilateralmente el estado.

### RF-CS-05 - Publicacion segura

Un contenido nuevo o modificado solamente podra
publicarse cuando exista una decision approved
vinculada a su version exacta y propietario.

### RF-CS-06 - Indisponibilidad

Si el proveedor falla, el contenido no se aprueba
automaticamente.

Podra quedar pending_review, sujeto al flujo de
persistencia y revision autorizado.

### RF-CS-07 - Revision humana

Preparar un flujo futuro de revision humana con
autorizacion, trazabilidad y privacidad.

No implementar interfaces administrativas en esta fase.

### RF-CS-08 - Independencia del proveedor

Los contratos publicos de Rutitas no expondran
credenciales ni respuestas internas de Azure.

## Requisitos no funcionales

- RNF-CS-01: JWT y autorizacion en servidor.
- RNF-CS-02: identidad del proveedor protegida.
- RNF-CS-03: validacion de entradas.
- RNF-CS-04: timeout y cancelacion.
- RNF-CS-05: rate limiting.
- RNF-CS-06: minimizacion de datos y logs.
- RNF-CS-07: presupuesto adicional cero.
- RNF-CS-08: proveedor simulado para pruebas.
- RNF-CS-09: trazabilidad de decisiones.
- RNF-CS-10: prevencion de evasion de moderacion.

## Fuera de alcance

- Moderacion automatica de conversaciones privadas.
- Reconocimiento facial o biometrico.
- Entrenamiento de modelos.
- Eliminacion automatica de cuentas.
- Sanciones automaticas contra usuarios.
- Clasificacion de identidad personal.
- Llamadas reales a Azure.
- Cambios de infraestructura productiva.

## Criterios de aceptacion documental

- Contrato candidato documentado.
- Politica de moderacion explicita.
- Separacion Backend y proveedor.
- Casos de error y seguridad definidos.
- Matriz de pruebas sinteticas.
- Revision de Backend, Android y coordinacion.

Los criterios funcionales permanecen pendientes.
