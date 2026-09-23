Taller de Programación II - TP_ControlVehicular

Nombre del proyecto
-------------------
Control Vehicular - Taller PRO

Integrantes
-----------
- Antonio Ledezma <antonioledezma@gmail.com>
- Walter Velazco <walterg20@gmail.com>


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
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TP_ControlVehicularDB;Trusted_Connection=True;MultipleActiveResultSets=true"

- SQL Server con usuario/clave:
  "DefaultConnection": "Server=SERVIDOR\\INSTANCIA;Database=TP_ControlVehicularDB;User Id=sa;Password=TuPassword;MultipleActiveResultSets=true"

Coloca la sección ConnectionStrings en Presentacion/appsettings.json o en el exe folder. No commitear credenciales en el repo.




Configuración inicial de la Base de Datos
-----------------------------------------
Para inicializar la base de datos con todas las tablas necesarias y cargar los datos de prueba (roles y usuarios), debes ejecutar el script SQL que se encuentra en la carpeta docs:

**Ubicación del script:** docs/BD_role_usuarios_script.sql

**Pasos para usarlo:**
1. Abre **SQL Server Management Studio (SSMS)** y conéctate a tu servidor SQL.
2. Abre el archivo docs/BD_role_usuarios_script.sql.
3. Haz clic en el botón **Ejecutar** (Execute) o presiona F5.
4. El script creará automáticamente la base de datos TP_ControlVehicular, sus tablas y los usuarios iniciales.

**Usuarios de prueba disponibles:**
Una vez ejecutado el script, puedes iniciar sesión en la aplicación utilizando cualquiera de estas credenciales según el rol que quieras probar:

- Administrador:
  DNI: 11111111
  Clave: admin123

- Recepcionista:
  DNI: 22222222
  Clave: recep123

- Mecánico:
  DNI: 33333333
  Clave: meca123
