# TP_ControlVehicular — Taller Pro

Aplicación de escritorio WPF (.NET 10) con patrón MVVM para la gestión integral
de un taller mecánico: clientes, vehículos, órdenes de servicio, tareas por
mecánico, facturación/cobro, reportes (QuestPDF) y respaldos en Google Drive.
Arquitectura en capas: `Entidad`, `Datos`, `Negocio`, `Presentacion` (+ `Migracion`).

## Stack y versiones
- .NET 10.0 (`net10.0-windows`), C# 13, WinExe, WPF (`UseWPF=true`)
- EF Core 10.0.11 + SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
- AutoMapper 16.2.0, QuestPDF 2026.9.1, Dapper 2.1.89
- Microsoft.Extensions (DI/Configuration/Hosting) 10.0.11
- Tests: xUnit + Moq (`TP_ControlVehicular.Tests`)

## Requisitos previos
- Windows 10/11 (64-bit)
- .NET SDK 10.0 (desarrollo) / .NET Desktop Runtime 10.0 (x64) (ejecución)
- Microsoft SQL Server 2019/2022 (Express, Standard o superior). Recomendado: instancia nombrada.
- (Opcional) `sqlcmd` en PATH para usar `bd/instalar.ps1`. SSMS para ejecutar los scripts manualmente.
- Visual Studio 2022/2026 Community (o `dotnet` CLI).

## 1. Clonar y restaurar
```powershell
git clone <repo> TP_ControlVehicular
cd "TP_ControlVehicular"
dotnet build "TP_ControlVehicular.slnx"
```
> La ruta contiene espacios: usar siempre comillas.

## 2. Configurar la conexión a SQL Server
La app resuelve la cadena de conexión `DefaultConnection` con esta precedencia
(de mayor a menor):

1. Variables de entorno con prefijo `TPCV_` (ej. `TPCV_ConnectionStrings__DefaultConnection`).
2. Archivo `.env` (raíz del proyecto o junto al ejecutable), si existe.
3. `appsettings.json` (`ConnectionStrings:DefaultConnection`).

### Opción recomendada para desarrollo: `.env`
```powershell
Copy-Item .env.example .env
```
Editar `.env` con los datos de tu servidor:
```env
TPCV_ConnectionStrings__DefaultConnection=Server=localhost\SQLEXPRESS;Database=ControlVehicular;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```
`.env` está en `.gitignore`: cada desarrollador mantiene el suyo sin tocar
archivos versionados.

### Opción alternativa: editar `appsettings.json`
`appsettings.json` está ignorado por git. Si no usas `.env`, edita la copia local:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<SERVIDOR>\\<INSTANCIA>;Database=ControlVehicular;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Parámetros de la cadena
- `Server` — equipo + instancia (ej. `localhost\SQLEXPRESS`, `WALTERG20\SQLEXPRESS2019`).
  Para un puerto fijo: `Server=<SERVIDOR>,1433`.
- `Database` — `ControlVehicular` (la BD productiva).
- `Trusted_Connection=True` — autenticación integrada de Windows (recomendada).
  La cuenta de Windows del desarrollador debe ser login en SQL Server.
- `TrustServerCertificate=True` — necesario con certificados autofirmados en LAN.
- `MultipleActiveResultSets=true` — requerido para grids con relaciones.
- Autenticación SQL (alternativa): reemplazar `Trusted_Connection=True` por
  `User Id=...;Password=...;` y habilitar Mixed Mode en el servidor.

> La app falla al arrancar si `appsettings.json` no existe o `DefaultConnection`
> está vacío (`optional: false` en `App.xaml.cs`).

## 3. Habilitar red en SQL Server (solo si conectas a otra máquina)
Por defecto SQL Server Express puede tener TCP/IP deshabilitado y puertos
dinámicos. En el servidor:
1. Abrir **SQL Server Configuration Manager**.
2. `SQL Server Network Configuration → Protocols for <INSTANCIA>` → habilitar **TCP/IP**.
3. En `IP Addresses → IPAll`: `TCP Dynamic Ports` vacío, `TCP Port = 1433`.
4. Reiniciar el servicio `SQL Server (<INSTANCIA>)`.
5. (Opcional) iniciar **SQL Server Browser** (UDP 1434) para resolver instancias nombradas.
6. Firewall: abrir TCP 1433 (y UDP 1434 si usas Browser):
   ```powershell
   New-NetFirewallRule -DisplayName "SQL Server TCP 1433" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
   ```
7. Verificar: `Test-NetConnection -ComputerName "<SERVIDOR>" -Port 1433`.

## 4. Crear e inicializar la base de datos
Los scripts de `bd/` son idempotentes. Orden estricto: `00` → `01` → `02` → `04` → `03`.
(`05` y `06` son opcionales: utilidades de test y población demo.)

### Opción A — Orquestador PowerShell (recomendado)
```powershell
cd "bd"
.\instalar.ps1 -Servidor "localhost\SQLEXPRESS" -BaseDatos "ControlVehicular"
```
Ajusta `-Servidor` a tu instancia. Requiere `sqlcmd`.

### Opción B — Manual (SSMS)
1. Conectarse a `master` y ejecutar `bd/00_bootstrap.sql` (crea la BD si no existe).
2. Conectarse a `ControlVehicular` y ejecutar en orden:
   `01_estructura.sql` → `02_datos.sql` → `04_triggers.sql` → `03_procedimientos.sql`.
3. (Opcional) `06_poblacion.sql` para datos demo.

### Verificación
```sql
USE [ControlVehicular];
SELECT name FROM sys.tables ORDER BY name;
SELECT name FROM sys.procedures
  WHERE name IN ('sp_ReporteIngresos','sp_ReporteIngresosAdmin',
                 'sp_ReporteProductividadMecanicos','sp_ReporteServiciosPorRango',
                 'sp_GenerarComprobanteOrden','sp_GenerarComprobantePago');
```

## 5. Credenciales de prueba (seed de bd/02_datos.sql)
| Rol | DNI | Contraseña inicial |
|---|---|---|
| Administrador | `10000001` | `CHANGE_ME` |
| Recepcionista | `10000003` | `CHANGE_ME` |
| Mecánico | `10000002` | `CHANGE_ME` |

> Cambiar las contraseñas semilla en cualquier entorno no de desarrollo.

## 6. Google Drive (módulo de respaldos, opcional)
El módulo de respaldos usa OAuth de Google. Las credenciales se resuelven con
esta precedencia:
1. Variables de entorno `TPCV_Google__ClientId` y `TPCV_Google__ClientSecret`.
2. Archivo `credenciales.json` (tipo Desktop app) junto al ejecutable o en la raíz
   del proyecto (copiado a la salida si existe).

Si ninguna está presente, la operación falla con un mensaje claro indicando
dónde colocar `credenciales.json`. El token de refresco se guarda por-PC cifrado
con DPAPI en `%LocalAppData%\TP_ControlVehicular\Drive` (nunca en la BD ni en el repo).

Para obtener `credenciales.json`: Google Cloud Console → crear OAuth 2.0 Client ID
(tipo Desktop app) → descargar JSON → renombrarlo a `credenciales.json` y
colocarlo junto al ejecutable (o en la raíz del proyecto).

## 7. Ejecutar la aplicación
```powershell
dotnet run --project "TP_ControlVehicular.csproj"
```
O desde Visual Studio: establecer `TP_ControlVehicular` como proyecto de inicio y F5.
El primer arranque detecta si la BD está vacía (o el admin sigue con la clave
bootstrap) y abre el modal de configuración inicial (`FrmSetupInicial`).

## 8. Tests
```powershell
dotnet test "TP_ControlVehicular.slnx"
```
- Las pruebas E2E usan una BD dedicada `ControlVehicular_Test` (nunca la productiva).
- Conexión de test: variable de entorno `TPCV_TEST_CONNECTION`, si no existe se
  deriva de `appsettings.json` reemplazando el catálogo por `ControlVehicular_Test`.
- Provisioning idempotente de los scripts `bd/01`–`bd/04` sobre la BD de test.

## 9. Estructura del proyecto (por capas)
- `Presentacion/` — WPF + MVVM: vistas, UserControls, ViewModels. DI y arranque en `App.xaml.cs`. Helpers (`EnvLoader`, `Debouncer`).
- `Negocio/` — handlers, DTOs, mappers, reportes QuestPDF, servicios (incl. Drive).
- `Datos/` — `CVDbContext`, repositorios, EF Core.
- `Entidad/` — clases POCO del dominio.
- `Migracion/` — migraciones EF Core y snapshot del modelo.
- `bd/` — scripts SQL canónicos idempotentes (`00_bootstrap`, `01_estructura`, `02_datos`, `03_procedimientos`, `04_triggers`, `05_utilidades_test`, `06_poblacion`).
- `docs/` — documentación y manuales (`docs/manuales/manual_tecnico_administracion.md`).
- `openspec/` — especificaciones SDD.

## Notas importantes
- `appsettings.json` se copia a la salida (`CopyToOutputDirectory=Always`) y la
  app lo lee con `SetBasePath(AppContext.BaseDirectory)`. En desarrollo, edita el
  de la raíz; en ejecución, el de `bin/Debug/net10.0-windows/`.
- `credenciales.json` y `.env` son opcionales y gitignored; colócalos junto al
  ejecutable (o en la raíz del proyecto) si los necesitas.
- Para más detalle operativo (TCP/IP, firewall, respaldos, DR, RBAC), ver
  `docs/manuales/manual_tecnico_administracion.md`.

## Equipo
- (completar)