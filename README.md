
# Circuit Breaker en C# .NET 8

## Descripción de la prueba

Se implementó el patrón **Circuit Breaker** en una aplicación .NET 8 con dos enfoques una implementación **manual** y la otra utilizando la librería **Polly**. La implementación del Circuit Breaker en ambos casos permite fallar 3 veces antes de abrir el circuito por 20 segundos. Además, la lógica del Circuit Breaker se separó del controlador a través de la inyección de dependencias para mejorar la mantenibilidad y la reutilización del código. Se construye una aplicacion que realiza llamados a elasticSearch en el que se usa HttpClient para obtener respuesta del servicio externo, el docker compose esta configurado para ejecutar el servicio de elasticSearch y la aplicacion del CircuitBreaker.

## Objetivo(s) de la prueba

- Implementar el patrón **Circuit Breaker** para manejar fallos temporales en servicios externos.
- Configurar el circuito para permitir 3 fallos consecutivos antes de abrirlo por 20 segundos.
- Separar la lógica del Circuit Breaker del controlador utilizando inyección de dependencias.
- Probar los diferentes estados del circuito: **cerrado**, **semiabierto** y **abierto**.

## Pasos implementados para llevar a cabo la prueba
1. **Implementación Manual:**
  - Se creó una clase `CircuitBreakerManual` que maneja la lógica del Circuit Breaker
  - Definición de los estados del Circuit Breaker (Abierto, Cerrado, Semi-Abierto).
  - Control de errores mediante excepciones, contabilizando fallos consecutivos.
  - Implementación de una lógica de reintento y tiempos de espera cuando el circuito está en estado "Abierto".
  - El controlador `ManualCircuitBreakerProductController` utiliza el servicio `CircuitBreakerPolicy` para ejecutar las operaciones y manejar el estado del Circuit Breaker de forma centralizada.
  - Simulación de fallos aleatorios para probar el comportamiento del Circuit Breaker.
  - El estado del Circuit Breaker (abierto, cerrado y semiabierto) es manejado y registrado en la consola para pruebas.

2. **Implementación con Polly:**
  - Se instala el paquete de la libreria Polly en la version 8.4.1
  - Se creó una clase `CircuitBreakerPolicy` que maneja la lógica del Circuit Breaker con Polly.
  - Se configura la politica del circuit breaker para: 
      - Permitir hasta 3 excepciones antes de abrir el circuito.
      - Mantener el circuito abierto durante 20 segundos antes de intentar pasar a un estado semiabierto.
  - El controlador `PollyCircuitBreakerProductController` utiliza el servicio `CircuitBreakerPolicy` para ejecutar las operaciones y manejar el estado del Circuit Breaker de forma centralizada.
  - Simulación de fallos para observar cómo Polly abre y cierra el circuito automáticamente.
  - El estado del Circuit Breaker (abierto, cerrado y semiabierto) es manejado y registrado en la consola para pruebas.

## Tecnologías usadas en la prueba

- **Lenguaje**: C#
- **Framework**: .NET 8
- **Librerías**:
  - [Polly](https://github.com/App-vNext/Polly): Librería de resiliencia para manejar patrones como el Circuit Breaker.

## Resultados

Durante las pruebas, se observó el comportamiento esperado del Circuit Breaker:

- **Estado Cerrado**: El servicio funcionó correctamente hasta que se alcanzó el límite de 3 excepciones.
- **Estado Abierto**: Después de 3 fallos, el circuito se abrió y rechazó las solicitudes durante 20 segundos.
- **Estado Semiabierto**: Después de los 20 segundos, el circuito pasó a estado semiabierto y permitió probar si el servicio estaba disponible nuevamente.
- **Recuperación**: El circuito volvió al estado cerrado cuando el servicio respondió correctamente en estado semiabierto.

## Conclusiones

- Este patron mejora la resilencia del sistema, permitiendo que los servicios que presenten algun problema se puedan recuperar de manera controlada.
- El patron permite monitorear el conteo de fallos consecutivos, esto nos ayuda a gestionar los riesgos asociados a interdependencias de servicios, especialmente en sistemas distribuidos.
- Optimiza el uso de recursos, evitando que se realicen intentos fallidos de comunicacion con servicios que no estan disponibles.
- La implementación del patrón **Circuit Breaker** con **Polly** proporciona una manera eficaz de manejar fallos temporales en servicios externos, mejorando la resiliencia de la aplicación.
