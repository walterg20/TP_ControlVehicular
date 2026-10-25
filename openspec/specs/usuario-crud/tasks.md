# Tareas de Implementación: Usuario CRUD

- [x] 1. **Modificar Entidad Usuario**: Agregar Apellido, Dni, Email, Telefono, Domicilio y FechaNacimiento a `Entidad/Usuario.cs`.
- [x] 2. **Migración EF Core**:
  - [x] Migraciones `AddDatosUsuario` y `AddUniqueConstraintsToUsuario` generadas y aplicadas a SQL Server.
- [x] 3. **Capa de Datos y Negocio**:
  - [x] `IUsuarioRepository` y `UsuarioRepository` actualizados.
  - [x] `UsuarioDto.cs` actualizado con los nuevos campos (Apellido, DNI, Email, Telefono, Domicilio, FechaNacimiento).
  - [x] Handlers `RegistrarUsuarioHandler`, `ModificarUsuarioHandler`, `ListarUsuariosHandler` y `MappingProfile` actualizados.
- [x] 4. **Capa de Presentación & Validaciones**:
  - [x] Validaciones de tiempo real mediante `ValidadorGlobal` (DNI, Email único, Teléfono formato celular, mayor de 18 años, campos obligatorios).
  - [x] `CtlUsuario.xaml` / `CtlUsuario.xaml.cs`: Tabla DataGrid con columnas actualizadas, filtrado por búsqueda y control de estados.
  - [x] `FrmUsuario.xaml` / `FrmUsuario.xaml.cs`: Formulario modal side-by-side con validación al perder el foco y `FrmConfirmacion` en bajas/modificaciones.
