# CONTENT-SAFETY-001 - Seguridad

Estado: PROPUESTA

## Acceso

- JWT obligatorio.
- Autorizacion por propietario.
- Revision humana limitada a roles autorizados.
- No permitir acceso a contenido de otros usuarios.
- No permitir aprobar contenido mediante un DTO cliente.

## Validacion

- Texto no vacio.
- Contexto permitido mediante enumeracion.
- Longitud limitada por politica.
- Imagen JPEG o PNG inicialmente.
- Validar firma binaria y formato real.
- Limitar dimensiones y tamano decodificado.
- Prevenir imagenes corruptas y bombas de descompresion.
- Rechazar URLs arbitrarias como entrada al proveedor.

## Valores candidatos

- Texto: maximo 2000 caracteres.
- Imagen: maximo 4 MiB.
- Dimensiones: hasta 4096 x 4096 pixeles.
- Timeout upstream: 8 segundos.
- Rate limit texto: 10 solicitudes/minuto/usuario.
- Rate limit imagen: 5 solicitudes/minuto/usuario.

Los valores requieren ratificacion y conciliacion
con los limites oficiales del proveedor.

## Credenciales

- Sin claves en Android.
- Sin claves en Git.
- No registrar Authorization headers.
- Identidad administrada preferida cuando sea viable.
- Confirmar RBAC y configuracion antes de consumir Azure.

## Privacidad

- No registrar el texto completo ni las imagenes.
- No persistir respuestas crudas del proveedor.
- Minimizar datos de auditoria.
- Eliminar metadatos innecesarios de las imagenes.
- Definir retencion antes de persistir resultados.
- Conciliar informacion de moderacion con DELETE-001.

## Errores

- 400: solicitud invalida.
- 401: autenticacion invalida.
- 403: operacion no autorizada.
- 413: contenido demasiado grande.
- 415: formato no admitido.
- 429: limite alcanzado.
- 502: respuesta upstream invalida.
- 503: proveedor indisponible.
- 504: timeout.

Ningun error del proveedor debe convertirse
automaticamente en approved.
