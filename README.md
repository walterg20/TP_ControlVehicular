Taller de Programación II - TP_ControlVehicular

Nombre del proyecto
-------------------
Control Vehicular - Taller PRO

Integrantes
-----------
- Integrante 1 <integrante1@example.com>
- Integrante 2 <integrante2@example.com>


Stack y versiones
------------------
- .NET: .NET 10
- C#: 14.0
- ORM: Entity Framework Core (Microsoft.EntityFrameworkCore + Microsoft.EntityFrameworkCore.SqlServer)
- Entorno de desarrollo: Visual Studio Community 2026
- Base de datos: SQL Server / LocalDB (provider: Microsoft.EntityFrameworkCore.SqlServer)

Estructura del proyecto (por capas)
-----------------------------------
- Presentacion
  - WPF (MVVM): vistas, windows, UserControls y ViewModels. Contiene la configuración de DI y arranque (App.xaml.cs).
- Negocio
  - Lógica de negocio: DTOs, servicios (handlers), mappers y reglas de negocio.
- Datos
  - Acceso a datos mediante Entity Framework Core: CVDbContext, repositories y migraciones (carpeta Migrations).
- Entidad
  - Clases de entidad (modelos que se mapean a tablas de la BD).

Resumen funcional
-----------------
- La vista (WPF) consume ViewModels que exponen ICommand y propiedades.
- El ViewModel invoca Handlers/Services en la capa Negocio.
- Los servicios usan Repositories en la capa Datos, que exponen DbSet<T> del CVDbContext.
- Entity Framework Core se encarga del mapeo ORM.

Configurar la conexión a la base de datos
-----------------------------------------
La aplicación lee la cadena de conexión desde appsettings.json (clave: DefaultConnection). Ejemplo de connection string para SQL Server:

- Integrated Security (LocalDB / instancia dev):
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ControlVehicular;Trusted_Connection=True;MultipleActiveResultSets=true"

- SQL Server con usuario/clave:
  "DefaultConnection": "Server=SERVIDOR\\INSTANCIA;Database=ControlVehicular;User Id=sa;Password=TuPassword;MultipleActiveResultSets=true"

Coloca la sección ConnectionStrings en Presentacion/appsettings.json o en el exe folder. No commitear credenciales en el repo.




Configuración inicial de la Base de Datos
-----------------------------------------
Para inicializar la base de datos (esquema, procedimientos almacenados, triggers y datos de prueba) ejecutá los scripts SQL canónicos de `bd/`, **en orden** (`01` → `04`). Son idempotentes.

**Ubicación de los scripts:** bd/01_estructura.sql, bd/02_procedimientos.sql, bd/03_triggers.sql, bd/04_datos.sql

**Pasos para usarlos:**
1. Abre **SQL Server Management Studio (SSMS)** y conéctate a tu servidor SQL.
2. Abre y ejecutá, en orden, bd/01_estructura.sql, bd/02_procedimientos.sql, bd/03_triggers.sql y bd/04_datos.sql.
3. Haz clic en el botón **Ejecutar** (Execute) o presiona F5 en cada uno.
4. Los scripts aplican el esquema, los procedimientos, los triggers y los datos semilla (roles y usuarios iniciales) sobre la base ControlVehicular.

**Usuarios de prueba disponibles:**
Una vez ejecutado el script, puedes iniciar sesión en la aplicación utilizando cualquiera de estas credenciales según el rol que quieras probar:

- Administrador:
  DNI: 11111111
  Clave: CHANGE_ME

- Recepcionista:
  DNI: 22222222
  Clave: CHANGE_ME

- Mecánico:
  DNI: 33333333
  Clave: CHANGE_ME
