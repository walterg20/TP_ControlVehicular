# Plan de Alineación de Pantallas CRUD con CtlModelo

## Descripción del Objetivo
Alinear integralmente las pantallas de administración de entidades que aún difieren del estándar establecido en `Presentacion/Pantalla/Modelo/CtlModelo.xaml` (y ya replicado en `CtlMarca` y `CtlVehiculo`).

Específicamente, el fix estandariza en las pantallas pendientes (**Cliente**, **Usuario**, **Rol**, **Servicio** y **Taller**):
1. **Filtro de Estado**: Reemplazar el CheckBox `"Mostrar inactivos"` (o ausencia de filtro) por el combo `Estado:` con opciones `["Todos", "Activos", "Inactivos"]` (con visibilidad controlada por `PuedeVerFiltroEstado` según permisos RBAC).
2. **Estilo visual de inactivos en DataGrid**: Aplicar `DataGrid.RowStyle` con `DataTrigger` sobre `Activo == False` para colorear la fila en rojo (`Foreground="#C0392B"`, `Background="#FDECEA"`).
3. **Columna Activo en DataGrid**: Eliminar la columna `DataGridCheckBoxColumn Header="Activo"` de las grillas que la poseían, ya que el estado inactivo queda identificado claramente por el resaltado en rojo de la fila.
4. **Botonera inferior (Pie de Acciones)**: Homogeneizar orden, estilo, paleta de colores y visibilidad de acciones:
   - `✏️ Editar Seleccionado` (`#F39C12` Naranja, `Margin="0,0,8,0"`, `IsEnabled="{Binding PuedeEditar}"`)
   - `🗑️ Eliminar Seleccionado` (`#C0392B` Rojo, `Margin="0,0,8,0"` o final, `IsEnabled="{Binding PuedeEliminar}"`)
   - `♻️ Reactivar` (`#27AE60` Verde, `Margin="8,0,0,0"`, `Visibility="{Binding PuedeVerFiltroEstado, Converter={StaticResource BoolToVis}}"`)
   - *(En Rol se preserva `🔑 Administrar Permisos` `#8E44AD` alineado a la izquierda de Editar).*

---

## Revisión de Usuario Requerida
> [!IMPORTANT]
> **Consistencia en Repositorios y Handlers**:
> Para soportar el filtro tri-estado (`Todos`, `Activos`, `Inactivos`), los métodos `GetPaginadoAsync` de los repositorios (`ClienteRepository`, `UsuarioRepository`, `RolRepository`, `ServicioRepository`, `TallerRepository`) y sus respectivos Handlers pasarán de recibir `bool incluirInactivos` (o nada) a recibir `string? filtroEstado`. Esto replica exactamente lo implementado en `MarcaRepository`, `ModeloRepository` y `VehiculoRepository`.

---

## Estado Actual vs. Estado Propuesto por Pantalla

| Pantalla | Filtro Estado Actual | Columna Activo en Grilla | Fila Roja en Inactivo | Botonera Inferior Actual | Botonera Propuesta |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Modelo** *(Base)* | `ComboBox Estado` (Todos/Activos/Inactivos) | ❌ *No tiene* | ✅ `#FDECEA` / `#C0392B` | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar | *Ya implementado (Referencia)* |
| **Marca** | `ComboBox Estado` (Todos/Activos/Inactivos) | ❌ *No tiene* | ✅ `#FDECEA` / `#C0392B` | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar | *Ya implementado* |
| **Vehiculo** | `ComboBox Estado` (Todos/Activos/Inactivos) | ❌ *No tiene* | ✅ `#FDECEA` / `#C0392B` | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar | *Ya implementado* |
| **Cliente** | `CheckBox Mostrar inactivos` | ⚠️ Sí tiene (`Width="80"`) | ❌ No tiene | ♻️ Reactivar (Azul), ✏️ Editar, 🗑️ Eliminar | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar (Verde) |
| **Usuario** | ❌ No tiene filtro de estado | ⚠️ Sí tiene (`Width="80"`) | ❌ No tiene | ✏️ Modificar, 🔄 Cambiar Activo | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar (Verde) |
| **Rol** | `CheckBox Mostrar inactivos` | ⚠️ Sí tiene (`Width="100"`) | ❌ No tiene | ♻️ Reactivar (Azul), 🔑 Permisos, ✏️ Modificar, 🔄 Cambiar Activo | 🔑 Permisos, ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar (Verde) |
| **Servicio** | `CheckBox Mostrar inactivos` | ⚠️ Sí tiene (`Width="90"`) | ❌ No tiene | ♻️ Reactivar (Azul), ✏️ Editar, 🗑️ Eliminar | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar (Verde) |
| **Taller** | `CheckBox Mostrar inactivos` | ⚠️ Sí tiene (`Width="80"`) | ❌ No tiene | ♻️ Reactivar (Azul), ✏️ Editar, 🗑️ Eliminar | ✏️ Editar, 🗑️ Eliminar, ♻️ Reactivar (Verde) |

---

## Cambios Propuestos

### 1. Capa de Datos y Negocio (Repositorios y Handlers)

#### [MODIFY] `Negocio/Interfaces/IClienteRepository.cs` & `Datos/Repositories/ClienteRepository.cs`
- Cambiar firma de `GetPaginadoAsync` de `bool incluirInactivos` a `string? filtroEstado`.
- Aplicar expresión lambda para `"Activos"` (`c => c.Activo`), `"Inactivos"` (`c => !c.Activo`) o `"Todos"` (sin filtro).

#### [MODIFY] `Negocio/Interfaces/IUsuarioRepository.cs` & `Datos/Repositories/UsuarioRepository.cs`
- Extender firma de `GetPaginadoAsync(string? busqueda, string? filtroEstado, int pagina, int tamanoPagina)`.
- Aplicar `filtroEstado` en la consulta con `u => u.Activo` / `u => !u.Activo`.
- Actualizar `ListarUsuariosHandler.cs` para propagar `filtroEstado`.

#### [MODIFY] `Negocio/Interfaces/IRolRepository.cs` & `Datos/Repositories/RolRepository.cs`
- Actualizar firma a `string? filtroEstado`.
- Actualizar `ListarRolHandler.cs` para propagar `filtroEstado`.

#### [MODIFY] `Negocio/Interfaces/IServicioRepository.cs` & `Datos/Repositories/ServicioRepository.cs`
- Actualizar firma a `string? filtroEstado`.
- Actualizar `ListarServiciosHandler.cs` para propagar `filtroEstado`.

#### [MODIFY] `Negocio/Interfaces/ITallerRepository.cs` & `Datos/Repositories/TallerRepository.cs`
- Actualizar firma a `string? filtroEstado`.
- Actualizar `ListarTallerHandler.cs` para propagar `filtroEstado`.

---

### 2. ViewModels (`Presentacion/ViewModels/`)

En cada uno de los ViewModels (`ClienteViewModel`, `UsuarioViewModel`, `RolViewModel`, `ServicioViewModel`, `TallerViewModel`):
- Incorporar propiedades de filtro de estado:
  ```csharp
  private string _filtroEstado = "Todos";
  public string FiltroEstado
  {
      get => _filtroEstado;
      set
      {
          if (_filtroEstado == value) return;
          _filtroEstado = value;
          OnPropertyChanged();
          ReiniciarPagina();
          _ = CargarPaginaAsync();
      }
  }

  public bool PuedeVerFiltroEstado => PuedeEliminar;

  public List<string> EstadosDisponibles { get; } = new List<string> { "Todos", "Activos", "Inactivos" };

  public new void NotificarPermisos()
  {
      base.NotificarPermisos();
      OnPropertyChanged(nameof(PuedeVerFiltroEstado));
  }
  ```
- En `CargarPaginaAsync()`:
  `var estado = PuedeVerFiltroEstado ? FiltroEstado : "Activos";`
  Pasar `estado` al método paginado.
- En `UsuarioViewModel`: agregar métodos `EliminarUsuarioAsync(int id)` y `ReactivarUsuarioAsync(int id)` para reemplazar el `ToggleActivoAsync` y seguir el estándar de confirmación unificada.
- En `RolViewModel`: estandarizar `EliminarRolAsync(int id)` y `ReactivarRolAsync(int id)`.
- En `TallerViewModel`: estandarizar `EliminarTallerAsync(int id)` y `ReactivarTallerAsync(int id)`.

---

### 3. Vistas y Code-Behind XAML (`Presentacion/Pantalla/`)

Para `CtlCliente`, `CtlUsuario`, `CtlRol`, `CtlServicio` y `CtlTaller`:

#### A. Barra superior de búsqueda (Row 1):
Reemplazar `CheckBox Mostrar inactivos` por:
```xaml
<TextBlock Text="Estado:" VerticalAlignment="Center" FontWeight="SemiBold" Foreground="#34495E" Margin="10,0,6,0"
           Visibility="{Binding PuedeVerFiltroEstado, Converter={StaticResource BoolToVis}}"/>
<ComboBox ItemsSource="{Binding EstadosDisponibles}"
          SelectedItem="{Binding FiltroEstado, Mode=TwoWay}"
          SelectionChanged="CmbFiltroEstado_SelectionChanged"
          Width="110" Padding="6,4" FontSize="13" BorderBrush="#BDC3C7" BorderThickness="1"
          Visibility="{Binding PuedeVerFiltroEstado, Converter={StaticResource BoolToVis}}">
    <ComboBox.Resources>
        <Style TargetType="Border">
            <Setter Property="CornerRadius" Value="6"/>
        </Style>
    </ComboBox.Resources>
</ComboBox>
```

#### B. Tabla DataGrid (Row 2):
- Agregar el bloque `DataGrid.RowStyle`:
  ```xaml
  <DataGrid.RowStyle>
      <Style TargetType="DataGridRow">
          <Style.Triggers>
              <DataTrigger Binding="{Binding Activo}" Value="False">
                  <Setter Property="Foreground" Value="#C0392B"/>
                  <Setter Property="Background" Value="#FDECEA"/>
              </DataTrigger>
          </Style.Triggers>
      </Style>
  </DataGrid.RowStyle>
  ```
- Eliminar `<DataGridCheckBoxColumn Header="Activo" Binding="{Binding Activo}" .../>`.

#### C. Pie de acciones inferior (Row 4):
Estandarizar el `StackPanel HorizontalAlignment="Right"`:
```xaml
<Button Content="✏️ Editar Seleccionado" Click="BtnEditar_Click" Padding="12,8" Background="#F39C12" Foreground="White" FontWeight="Bold" BorderThickness="0" Margin="0,0,8,0" Cursor="Hand" IsEnabled="{Binding PuedeEditar}">
    <Button.Resources>
        <Style TargetType="Border">
            <Setter Property="CornerRadius" Value="6"/>
        </Style>
    </Button.Resources>
</Button>
<Button Content="🗑️ Eliminar Seleccionado" Click="BtnBorrar_Click" Padding="12,8" Background="#C0392B" Foreground="White" FontWeight="Bold" BorderThickness="0" Cursor="Hand" IsEnabled="{Binding PuedeEliminar}">
    <Button.Resources>
        <Style TargetType="Border">
            <Setter Property="CornerRadius" Value="6"/>
        </Style>
    </Button.Resources>
</Button>
<Button Content="♻️ Reactivar" Click="BtnReactivar_Click" Padding="12,8" Background="#27AE60" Foreground="White" FontWeight="Bold" BorderThickness="0" Cursor="Hand" Margin="8,0,0,0"
        Visibility="{Binding PuedeVerFiltroEstado, Converter={StaticResource BoolToVis}}">
    <Button.Resources>
        <Style TargetType="Border">
            <Setter Property="CornerRadius" Value="6"/>
        </Style>
    </Button.Resources>
</Button>
```
*(En `CtlRol.xaml`, conservar además `🔑 Administrar Permisos` `#8E44AD`).*

#### D. Code-Behind (`Ctl*.xaml.cs`):
- Agregar `CmbFiltroEstado_SelectionChanged`:
  ```csharp
  private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
      if (this.DataContext is MiViewModel vm)
      {
          vm.CancelarBusquedaProgramada();
          await vm.BuscarAsync();
      }
  }
  ```
- Unificar los nombres de manejadores a `BtnEditar_Click`, `BtnBorrar_Click`, `BtnReactivar_Click`.
- Asegurar uso de `FrmConfirmacion.Mostrar(...)` y `FrmConfirmacion.MostrarAviso(...)`.

---

### 4. Pruebas Unitarias (`TP_ControlVehicular.Tests`)

#### [MODIFY] `TP_ControlVehicular.Tests/ViewModels/ListadosCrudPaginacionTests.cs`
- Actualizar los mocks de `IClienteRepository`, `IUsuarioRepository`, `IRolRepository`, `IServicioRepository` y `ITallerRepository` para verificar la llamada con el argumento `string? filtroEstado` (`"Activos"` en lugar de `false`).
- Verificar que las pruebas unitarias compilen y pasen al 100%.

---

## Plan de Verificación

### Compilación y Tests Automatizados
```powershell
dotnet build "TP_ControlVehicular.slnx"
dotnet test --filter "FullyQualifiedName~ListadosCrudPaginacionTests"
dotnet test --filter "FullyQualifiedName~RbacTests"
```

### Verificación Manual / Visual
1. Ejecutar la aplicación: `dotnet run --project "TP_ControlVehicular.csproj"`.
2. Navegar a **Clientes**:
   - Verificar presencia del ComboBox `Estado:` con `Todos`, `Activos`, `Inactivos`.
   - Comprobar que no existe la columna `Activo` en la grilla.
   - Filtrar `Todos` o `Inactivos`: constatar que las filas de clientes dados de baja se renderizan con fondo rosa/rojo suave (`#FDECEA`) y texto rojo (`#C0392B`).
   - Verificar la botonera inferior: Editar (Naranja), Eliminar (Rojo) y Reactivar (Verde a la derecha).
3. Repetir la comprobación visual en **Usuarios**, **Roles**, **Servicios** y **Talleres**.
