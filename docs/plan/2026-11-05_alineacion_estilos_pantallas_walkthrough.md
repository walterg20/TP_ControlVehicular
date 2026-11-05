# Walkthrough: Alineación de Estilos y Estados CRUD con CtlModelo

## Resumen de Cambios
Se completó la alineación y estandarización visual y funcional de los UserControls CRUD principales tomando como modelo de referencia `Presentacion/Pantalla/Modelo/CtlModelo.xaml`.

Las pantallas modificadas fueron:
1. `CtlCliente` (`Presentacion/Pantalla/Cliente/`)
2. `CtlUsuario` (`Presentacion/Pantalla/Usuario/`)
3. `CtlRol` (`Presentacion/Pantalla/Rol/`)
4. `CtlServicio` (`Presentacion/Pantalla/Servicio/`)
5. `CtlTaller` (`Presentacion/Pantalla/Taller/`)

(Nota: `CtlModelo`, `CtlMarca` y `CtlVehiculo` ya contaban con esta estructura previa de referencia).

---

## Modificaciones Implementadas

### 1. Barra de Búsqueda y Filtro de Estado
- Se removió el checkbox "Mostrar inactivos" (o su ausencia).
- Se incorporó un ComboBox `Estado:` con las opciones `Todos`, `Activos`, `Inactivos`, enlazado a la propiedad `FiltroEstado` de cada ViewModel.
- La visibilidad del filtro está ligada a `PuedeVerFiltroEstado` (`PuedeEliminar` del módulo), permitiendo sólo a usuarios con permisos de baja/reactivación alternar entre estados mientras que los demás usuarios visualizan únicamente registros activos.

### 2. Grilla DataGrid
- Se eliminó la columna `Activo` (checkbox) de cada grilla.
- Se agregó el estilo `DataGrid.RowStyle` con trigger para resaltar registros inactivos con fondo y texto en tonos rojizos:
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

### 3. Botonera Inferior
- Se estandarizó la botonera inferior en la fila 4 (`Row="4"`), alineada a la derecha con botones de estilo moderno y bordes redondeados (`CornerRadius="6"`):
  - ✏️ **Editar**: `#F39C12` (Naranja)
  - 🗑️ **Eliminar**: `#C0392B` (Rojo)
  - 🔄 **Reactivar**: `#27AE60` (Verde, visible según `PuedeEliminar` y habilitado al seleccionar un registro inactivo)
  - (En `CtlRol` se preserva el botón `Administrar Permisos`).

### 4. Capas de Negocio y Datos
- Repositorios `IClienteRepository`, `IUsuarioRepository`, `IRolRepository`, `IServicioRepository`, `ITallerRepository` y sus implementaciones en `Datos/Repositories/` adaptados para recibir `string? filtroEstado`.
- Handlers `ListarUsuariosHandler`, `ListarRolHandler`, `ListarServiciosHandler`, `ListarTallerHandler` actualizados.
- ViewModels con propiedades `FiltroEstado`, `EstadosDisponibles` y `PuedeVerFiltroEstado`.

### 5. Tests Unitarios
- `TestCollectionBehavior.cs`: deshabilitó la paralelización conflictiva en xUnit para pruebas que interactúan con la sesión estática `UserSession.CurrentUser`.
- Actualización de firmas y aserciones de mock en `ClientePaginacionTests.cs`, `ListadosCrudPaginacionTests.cs` y `UsuarioViewModelTests.cs`.
- Todos los 147 tests unitarios pasan exitosamente (100% de éxito).

---

## Verificación de Compilación y Pruebas
- `dotnet build "TP_ControlVehicular.slnx"`: **0 Errores**, 74 advertencias previas no bloqueantes.
- `dotnet test --filter "FullyQualifiedName!~EndToEnd"`: **147 pruebas superadas, 0 con error**.
