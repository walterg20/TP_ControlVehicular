# Design: Ocultar Modelos y Marcas del Menú

## Arquitectura y Componentes Afectados

La modificación es puramente visual y de navegación, por lo que solo se verá afectada la Capa de Presentación. Las pantallas subyacentes (`CtlModelo.xaml`, `CtlMarca.xaml`) y sus controladores seguirán existiendo en el código base sin modificaciones, listos para ser invocados si en el futuro se decide abrir estas ventanas desde otro lugar (ej. desde un botón dentro de la pantalla de Vehículos).

### 1. `Presentacion/MainWindow.xaml`
* **XAML Visual**: Cambiar la propiedad `Visibility` de `Visible` a `Collapsed` directamente en la definición de los botones `btnModelo` y `btnMarca`.

### 2. `Presentacion/MainWindow.xaml.cs`
* **Lógica de Roles**: En el método `AplicarRestriccionesPorRol()`, donde se resetea la visibilidad inicial a `Visibility.Visible` para el modo Administrador, es necesario **remover o establecer en `Collapsed`** las líneas correspondientes a `btnModelo` y `btnMarca` para evitar que el sistema por código vuelva a hacerlos visibles accidentalmente tras iniciar sesión.

## Riesgos y Consideraciones
* **Pantallas Huérfanas**: Al ocultar estos botones, las pantallas `CtlModelo` y `CtlMarca` quedarán "huérfanas" (sin un punto de entrada en la interfaz gráfica). Esto es aceptable en esta fase, pero a futuro se recomienda agregar un acceso a estas tablas maestras, idealmente dentro de un modal o pestaña en el ABM de Vehículos, para que los usuarios puedan dar de alta nuevas marcas si lo necesitan.
