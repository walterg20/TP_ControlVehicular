# Plan de Ejecución — Alineación del Catálogo y Formulario de Servicios con el Patrón de Clientes

**Fecha:** 2026-11-02  
**Autor:** Antigravity / Walter Velazco  
**Objetivo:** Alinear la gestión, selección y edición de servicios (`CtlServicio`, `FrmServicio` y `ServicioViewModel`) al estándar establecido en Clientes (`CtlCliente`, `FrmCliente` y `ClienteViewModel`).

---

## 1. Contexto y Objetivos
Actualmente, la pantalla de Servicios difiere de Clientes en dos aspectos clave:
1. **Selección en grilla:** `CtlServicio` no enlaza bidireccionalmente el elemento seleccionado (`SelectedItem`) al ViewModel ni utiliza la propiedad `ServicioSeleccionado`. El botón se llama `"✏️ Editar"` en lugar de `"✏️ Editar Seleccionado"` y no avisa de forma consistente si no hay un elemento elegido desde el ViewModel.
2. **Formulario modal y estado de modificación:** `FrmServicio` no implementa el patrón con bandera booleana `_esModificacion`, ni cuenta con los dos constructores (nuevo sin parámetros y modificación recibiendo el DTO). Además, en `CtlServicio` se seteaban propiedades a mano en lugar de pasar el DTO al modal para que configure su ViewModel.

---

## 2. Cambios a Implementar

### A. ViewModel (`ServicioViewModel.cs`)
- Incorporar la propiedad `ServicioDto? ServicioSeleccionado`:
  ```csharp
  private ServicioDto? _servicioSeleccionado;
  public ServicioDto? ServicioSeleccionado
  {
      get => _servicioSeleccionado;
      set => SetProperty(ref _servicioSeleccionado, value);
  }
  ```
- Envolver `SaveAsync()` con `try/catch` para capturar cualquier posible fallo de base de datos de forma segura retornando `false`.

### B. Vista del Listado (`CtlServicio.xaml` y `CtlServicio.xaml.cs`)
1. **En `CtlServicio.xaml`:**
   - Enlazar en `dgServicios`: `SelectedItem="{Binding ServicioSeleccionado, Mode=TwoWay}"`, `SelectionMode="Single"`, `CanUserAddRows="False"`.
   - Agregar soporte para doble clic de fila (`MouseDoubleClick="DgServicios_MouseDoubleClick"`).
   - Actualizar textos de botones inferiores:
     - `"✏️ Editar Seleccionado"` (con `IsEnabled="{Binding PuedeEditar}"`).
     - `"🗑️ Eliminar Seleccionado"` (con `IsEnabled="{Binding PuedeEliminar}"`).
     - `"♻️ Reactivar Seleccionado"` (con `IsEnabled="{Binding PuedeEditar}"`).
2. **En `CtlServicio.xaml.cs`:**
   - En `BtnNuevo_Click`: Instanciar `var frm = new FrmServicio();` (constructor para nuevo).
   - En `BtnEditar_Click`:
     - Validar `if (ViewModel?.ServicioSeleccionado is null)` y alertar con:
       `FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un servicio.", "Aviso", Window.GetWindow(this));`
     - Instanciar `var frm = new FrmServicio(ViewModel.ServicioSeleccionado);`.
     - Si retorna `true`, recargar con `await ViewModel.LoadAsync();`.
   - Conectar `DgServicios_MouseDoubleClick` para que llame a `BtnEditar_Click`.
   - En `BtnBorrar_Click` y `BtnReactivar_Click`: usar `ViewModel.ServicioSeleccionado` validando previamente la selección con aviso si es nulo.

### C. Formulario Modal (`FrmServicio.xaml` y `FrmServicio.xaml.cs`)
1. **En `FrmServicio.xaml.cs`:**
   - Campo privado: `private bool _esModificacion = false;`.
   - **Constructor Nuevo:**
     ```csharp
     public FrmServicio()
     {
         InitializeComponent();
         lblTituloFormulario.Text = "Registrar Nuevo Servicio";
         _esModificacion = false;
         this.Title = "Registrar Nuevo Servicio";
         this.ConfigurarViewModel(null);
         Loaded += (s, e) => txtNombre.Focus();
         ConfigurarValidacionAlPerderFoco();
     }
     ```
   - **Constructor Modificar:**
     ```csharp
     public FrmServicio(ServicioDto servicioDto)
     {
         InitializeComponent();
         lblTituloFormulario.Text = "Modificar Servicio";
         _esModificacion = true;
         this.Title = "Modificar Servicio";
         this.ConfigurarViewModel(servicioDto);
         Loaded += (s, e) => txtNombre.Focus();
         this.ConfigurarValidacionAlPerderFoco();
     }
     ```
   - **Método `ConfigurarViewModel(ServicioDto? servicio)`:**
     - Resuelve `ServicioViewModel` desde `App.ServiceProvider`.
     - Si `_esModificacion && servicio != null`:
       - Asigna `vmServicio.IdServicio = servicio.IdServicio;`
       - Asigna `vmServicio.Nombre = servicio.Nombre;`
       - Asigna `vmServicio.Precio = servicio.Precio;`
       - Asigna `vmServicio.Activo = servicio.Activo;`
     - Si no:
       - Llama a `vmServicio.LimpiarFormulario();`
     - Llama a `vmServicio.ClearAllErrors();`.
   - **En `BtnGuardar_Click`:**
     - Forzar actualización de fuente del binding para el elemento que tenga foco (para que no falle al presionar Enter).
     - Validar formulario (`vmServicio.ValidateAll()`).
     - Ejecutar `await vmServicio.SaveAsync()`.
     - Si es exitoso, `this.DialogResult = true; Close();`.
     - Si falla, mostrar aviso con `FrmConfirmacion.MostrarAviso("Por favor, corrija los errores del formulario antes de continuar.", "Validación", this);`.

---

## 3. Plan de Tareas
- [x] **Tarea 1:** Actualizar `ServicioViewModel.cs` agregando `ServicioSeleccionado` y manejo seguro en `SaveAsync`.
- [x] **Tarea 2:** Reestructurar `FrmServicio.xaml.cs` adoptando `_esModificacion`, doble constructor y `ConfigurarViewModel`.
- [x] **Tarea 3:** Actualizar `CtlServicio.xaml` y `CtlServicio.xaml.cs` con el binding `ServicioSeleccionado`, validaciones de selección previa, botón "Editar Seleccionado" y doble clic.
- [x] **Tarea 4:** Corregir permisos RBAC en `BaseViewModel.TienePermiso`, refrescar permisos en `Loaded` de `CtlServicio` y bloquear edición a usuarios Mecánico (Rol 3).
- [x] **Tarea 5:** Compilar y ejecutar suite de pruebas (`129/129 passed`).
