# Technical Design: Mejoras de UX en Orden de Servicio

## Design Details

### 1. ViewModel (`OrdenServicioViewModel.cs`)
- Inyectar `IClienteRepository` (o usar el endpoint si existe) para poblar `ObservableCollection<ClienteDto> ClientesDisponibles`.
- Propiedad `ClienteId` que al cambiar (setter) dispare un método `FiltrarVehiculosPorCliente()`.
- Modificar el combo de Vehículos para que dependa de esta lista filtrada.
- Agregar un método `RecargarClientesYVehiculosAsync()` para que sea invocado luego de cerrar un modal de creación rápida.

### 2. UI - Filtros y Modales (`CtlOrdenServicioForm.xaml` y `.cs`)
- Mover/Reorganizar la cabecera (Grid) para acomodar:
  - Label y ComboBox "Cliente" + Botón `[+]`
  - Label y ComboBox "Vehículo" + Botón `[+]`
- En Code-behind, atrapar el evento Click de los botones `[+]`:
  ```csharp
  var frm = new FrmCliente();
  if (frm.ShowDialog() == true) {
      await viewModel.LoadCombosAsync(); // o Recargar clientes
      // Setear ClienteId si es posible recuperarlo
  }
  ```

### 3. UI - Autoasignación de Mecánicos
- Al cargar el formulario (`ConfigurarViewModel` o `Loaded`), si `esMecanico` es true:
  - En la grilla `dgChecklist`, buscar la columna del Combo de Mecánico y setear su propiedad `IsReadOnly` o en el XAML usar un Trigger o desde código en el evento `BeginningEdit` para cancelar la edición de esa columna. Una forma fácil por XAML es binding: `IsEnabled="{Binding IsMecanico, Converter={StaticResource InverseBooleanConverter}}"` pero no tenemos converter, así que en CodeBehind cancelamos el `BeginningEdit` de esa columna si es mecánico.
- En el botón `BtnAgregarItem_Click`:
  ```csharp
  int idMecanico = esMecanico ? MainWindow.UsuarioSesionActual.IdUsuario : 0; // O el primero del combo si se desea
  // Setear ese id al DetalleServicioDto recién instanciado
  ```
