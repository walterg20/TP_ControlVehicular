# Tareas de Implementación: Workflow de Órdenes y Roles

## Task 1: Formato de Nombre del Mecánico
- **Archivo:** `Negocio/DTOs/UsuarioDto.cs`
- **Acción:** Agregar la propiedad de solo lectura: `public string NombreCompletoYDni => $"{Nombre} {Apellido} [{Dni}]";`.

## Task 2: ViewModel (Estados Automáticos y Roles)
- **Archivo:** `Presentacion/ViewModels/OrdenServicioViewModel.cs`
- **Acción 1 (Rol):** Crear una propiedad booleana `EsRecepcionista` que devuelva `true` si el usuario en sesión es recepcionista/admin, o `false` si es mecánico. (Puedes basarte en cómo ya se chequea el rol `IdRol != 3` o comprobando si es Mecánico en `Application.Current.MainWindow`).
- **Acción 2 (Estado Abierto):** En el constructor, cuando se crea una orden vacía (`OrdenSeleccionada == null`), inicializar `Estado = "Pendiente"`.
- **Acción 3 (Máquina de Estados):** Crear el método `public void EvaluarEstadoGeneral()`:
  - Si `DetallesOrdenActual.Count == 0`, no hace nada.
  - Si `All(d => d.Realizado == true)`, entonces `Estado = "Finalizada"`.
  - Si `Any(d => d.Realizado == true)`, entonces `Estado = "En Proceso"`.
  - Si `All(d => d.Realizado == false)`, entonces `Estado = "Pendiente"`.

## Task 3: Interfaz Gráfica (Bloqueos XAML)
- **Archivo:** `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml`
- **Acción 1:** En la columna del ComboBox de Mecánico, cambiar `DisplayMemberPath="Nombre"` por `DisplayMemberPath="NombreCompletoYDni"`.
- **Acción 2:** Agregar `IsEnabled="{Binding DataContext.EsRecepcionista, RelativeSource={RelativeSource AncestorType=UserControl}}"` al ComboBox de Mecánico.
- **Acción 3:** Al CheckBox de la columna "Revisado / OK", agregarle eventos `Checked="CheckBoxRevisado_Changed"` y `Unchecked="CheckBoxRevisado_Changed"`.

## Task 4: Code-Behind (Filtro y Eventos)
- **Archivo:** `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`
- **Acción 1 (Filtro):** El filtro del `CollectionView` ya existe parcialmente. Asegurarse de que aplique `view.Filter` correctamente usando el `IdUsuario` de la sesión.
- **Acción 2 (Cambio de Estado):** Implementar el manejador `CheckBoxRevisado_Changed(object sender, RoutedEventArgs e)` que simplemente llame a `vmOrdenServicioForm.EvaluarEstadoGeneral()`.
- **Acción 3 (Servicio Duplicado):** Validar que `BtnAgregarItem_Click` arroja la alerta correcta (ya existe un `Any`, validar que funcione). Setear `UsuarioId = currentUserId` si es mecánico (ya está parcialmente).
