Taller de Programación II - TP_ControlVehicular

Nombre del proyecto
-------------------
Control Vehicular - Taller PRO

Integrantes
-----------
- Antonio Ledezma <antonioledezma@gmail.com>
- Walter Velazco <walterg20@gmail.com>

(Reemplazar los correos por los reales antes de entregar.)

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
- Entity Framework Core se encarga del mapeo ORM y de las migraciones.

Configurar la conexión a la base de datos
-----------------------------------------
La aplicación lee la cadena de conexión desde appsettings.json (clave: DefaultConnection). Ejemplo de connection string para SQL Server:

- Integrated Security (LocalDB / instancia dev):
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TP_ControlVehicularDB;Trusted_Connection=True;MultipleActiveResultSets=true"

- SQL Server con usuario/clave:
  "DefaultConnection": "Server=SERVIDOR\\INSTANCIA;Database=TP_ControlVehicularDB;User Id=sa;Password=TuPassword;MultipleActiveResultSets=true"

Coloca la sección ConnectionStrings en Presentacion/appsettings.json o en el exe folder. No commitear credenciales en el repo.

Flujo de migraciones (recomendado)
----------------------------------
1) En la máquina de desarrollo (crear y commitear migración):
   dotnet tool install --global dotnet-ef    # si no lo tienes
   dotnet restore
   dotnet build
   dotnet ef migrations add <NombreMigracion> --project Datos --startup-project Presentacion --context CVDbContext
   git add Datos/Migrations
   git commit -m "Add migration <NombreMigracion>"
   git push

2) En otra máquina (tras clonar / pull):
   dotnet restore
   dotnet build
   dotnet ef database update --project Datos --startup-project Presentacion --context CVDbContext




Buenas prácticas y recomendaciones
---------------------------------
- Commitear las migraciones (carpeta Migrations) al repo para que otras máquinas puedan aplicarlas.
- No almacenar credenciales en el repositorio; usar variables de entorno o secret manager.
- En entornos críticos, generar script SQL con dotnet ef migrations script y revisarlo antes de aplicar.
- Mantener README actualizado con instrucciones de arranque y configuración de la BD.

Contacto y notas finales
------------------------
Actualiza la sección Integrantes con los correos reales. Si quieres, puedo:
- Generar un ejemplo de appsettings.json listo para usar (sin credenciales).
- Añadir el fragmento de auto-migrate directamente en App.xaml.cs y crear una migración de ejemplo.
