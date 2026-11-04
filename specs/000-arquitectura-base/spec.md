# Especificación — Arquitectura base de Backend

**ID:** ARCH-BE-BASE  
**Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `—`

## 1. Objetivo

Establecer requisitos arquitectónicos transversales para organizar un futuro componente Backend de Rutitas de forma comprensible, modular, segura, configurable y comprobable. Esta especificación no describe funcionalidades del producto.

## 2. Alcance y exclusiones

Incluye responsabilidades, límites internos, configuración, contratos como concepto, validación, errores, seguridad, persistencia como abstracción, observabilidad, pruebas y mantenibilidad. Excluye casos de uso, endpoints, roles de producto, entidades/tablas de negocio, reglas funcionales, proveedor cloud, lenguaje/framework, base de datos y despliegue concreto.

## 3. Fuentes y restricciones

Fuentes: Constitución propuesta de Rutitas, AGENTS de esta entrega, instrucciones académicas disponibles (fecha exacta pendiente de verificación), análisis SDD del curso y ejemplos de arquitectura de clase. Los ejemplos tecnológicos no son decisiones de Rutitas. No se ha confirmado un backend funcional aprobado; la propuesta no presupone stack.

### 3.1 Actores genéricos

- Equipo de arquitectura y revisión técnica del proyecto Backend.
- Responsables de seguridad, privacidad, operación y calidad, cuando sean designados.
- Autoridad de producto y autoridad de ratificación, aún `PENDIENTE_DE_DESIGNAR`.
- Agentes y skills de revisión documental, sin autoridad de aprobación.

### 3.2 Supuestos

- La Constitución y AGENTS locales son candidatos y no están ratificados.
- El stack, la persistencia, el proveedor y el despliegue siguen sin decidir.
- No existe un backend funcional aprobado ni contratos con Movil.
- Las decisiones abiertas se registran en `docs/decisiones-pendientes.md`.
- La entrega permanece en fase exclusivamente documental.

## 4. Requisitos arquitectónicos

| ID | Requisito | Criterio documental de aceptación |
|---|---|---|
| ARCH-BE-01 | Separación de responsabilidades | Se distinguen adaptadores de entrada, aplicación, dominio e infraestructura; cada uno tiene responsabilidad y dependencias documentadas. |
| ARCH-BE-02 | Modularidad proporcional | Las capacidades futuras pueden tener límites explícitos; no se requieren microservicios ni módulos de dominio vacíos. |
| ARCH-BE-03 | Organización mantenible | El plan propone una estructura lógica identificable y reglas para evitar ciclos y acoplamiento a proveedores. |
| ARCH-BE-04 | Configuración por entorno | Se separan valores de configuración y secretos; no se codifican secretos ni valores de producción en el cliente o repositorio. |
| ARCH-BE-05 | Contratos entre capas | Toda frontera futura identifica entradas/salidas, validación, errores, permisos, compatibilidad y propietario; no se define una API concreta aquí. |
| ARCH-BE-06 | Errores y validación | Se propone validación de entrada confiable y una estrategia consistente de errores que no exponga datos internos. |
| ARCH-BE-07 | Seguridad por diseño | El diseño futuro contempla mínimo privilegio, límites de confianza, autorización en el servidor por recurso y operación cuando existan capacidades conectadas, y gestión segura de secretos. |
| ARCH-BE-08 | Persistencia como abstracción | Las reglas de aplicación no dependen directamente del motor; las decisiones de consistencia, transacción y migración se especifican cuando haya requisitos. |
| ARCH-BE-09 | Observabilidad y logging | Se identifican señales operativas útiles, correlación y redacción de información sensible; se documentan propósito y retención. |
| ARCH-BE-10 | Pruebas previstas | El plan diferencia validaciones unitarias, integración/contrato y seguridad según riesgos; no se reporta ninguna como ejecutada. |
| ARCH-BE-11 | Mantenibilidad | Decisiones, dependencias, límites y convenciones se documentan con trazabilidad y revisión. |

## 5. Calidad, privacidad y riesgos

La futura arquitectura debe reducir privilegios innecesarios, validar en límites de confianza, evitar secretos en repositorio/logs y limitar datos tratados. Los criterios aplicables dependen del alcance y clasificación futura de datos. Riesgos actuales: decisiones de tecnología/operación aún abiertas, restricciones del equipo no confirmadas y posibles contradicciones al pasar de plantilla a repositorio real.

## 6. Criterios de aceptación de esta especificación

| ID | Criterio documental |
|---|---|
| AC-BE-01 | Define requisitos arquitectónicos generales y comprobables documentalmente. |
| AC-BE-02 | No especifica funcionalidad, endpoint, entidad ni regla de negocio de Rutitas. |
| AC-BE-03 | No elige lenguaje, framework, base de datos, nube ni herramienta. |
| AC-BE-04 | Se alinea con Constitución y AGENTS, y conserva las decisiones pendientes. |
| AC-BE-05 | Distingue criterios documentales de validación técnica futura. |

## 7. Decisiones pendientes

Repositorio y entorno objetivo; lenguaje/runtime/framework; patrón y límites de módulos; protocolo y contrato; persistencia; autenticación/autorización aplicable; política de secretos; logging/retención; calidad y releases. Ver [decisiones de este proyecto](../../docs/decisiones-pendientes.md).
