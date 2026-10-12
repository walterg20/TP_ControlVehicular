# Guía de configuración de respaldos (mini tutorial)

Paso a paso para dejar funcionando la pantalla de **Respaldos**: generar una copia de la base,
probarla (restaurándola en una base temporal) y resguardarla en **Google Drive**.

> Complementa a `docs/referencia/backup.md` (que describe *cómo está hecho*). Este documento
> describe *qué hay que tener* y *cómo se hace* para ponerlo en marcha.

---

## 1. Qué hace el flujo (resumen)

| Acción en la app | Qué ocurre por dentro |
|---|---|
| **💾 Hacer Backup** | Ejecuta `BACKUP DATABASE ... TO DISK` en el **servidor** y luego `RESTORE VERIFYONLY`. Genera `ControlVehicular_YYYYMMDD_HHMMSS_FULL.bak`. |
| **🧪 Probar Backup** | Restaura el `.bak` seleccionado en una base de prueba `ControlVehicular_Test` (online) y cuenta sus tablas. **No toca la base real.** |
| **☁️ Resguardar** | Sube **el último** `.bak` a Google Drive (nombre `ControlVehicular_YYYYMMDD_FULL.bak`, **un archivo por día**; si ya se subió hoy, no repite). |
| **🔄 Actualizar** | Vuelve a listar los `.bak` de la carpeta configurada. |

El permiso **`Configuracion.Backup.Gestionar`** pertenece a **Admin (`IdRol = 1`)**.
Recepcionista (2) y Mecánico (3) **no** ven la opción en el menú.

---

## 2. Requisitos previos (checklist)

- [ ] **SQL Server** central con la base `ControlVehicular` (el objetivo del taller es 1 servidor + 2 PCs). SQL Server 2019 **Express** no tiene Agente → no hay respaldos automáticos.
- [ ] La **cuenta de Windows** con la que la app se conecta (hoy *Windows Auth*) debe tener permisos de **backup** y de **restore** en el servidor (ver §7).
- [ ] Una **carpeta en el servidor** donde el servicio SQL escriba los `.bak` y **compartida (UNC)** para que las PCs la lean (ver §4).
- [ ] Una **cuenta de Google** y un **proyecto en Google Cloud** con credenciales OAuth descargadas como archivo **`credenciales.json`** (ver §5).
- [ ] La app ejecutándose con un **usuario Admin** para ver el menú de Administración.

> La **carpeta de Drive** no se configura a mano: la app busca y, si no existe, **crea automáticamente**
> la carpeta `ControlVehicularBackups` la primera vez que se resguarda (ver §6).

---

## 3. Paso 1 — Crear la tabla y el permiso (scripts SQL)

Ejecutar una sola vez sobre `ControlVehicular` (son **idempotentes**, se pueden repetir):

- `bd/21_ConfiguracionBackup.sql` → crea la tabla singleton `dbo.ConfiguracionBackup` (una fila, `Id = 1`) con rutas vacías.
- `bd/22_PermisosBackup.sql` → crea el permiso `Configuracion.Backup.Gestionar` y lo asigna a Admin (`IdRol = 1`).

Desde SQL Server Management Studio o `sqlcmd`:

```sql
USE [ControlVehicular];
GO
-- (pegar el contenido de bd/21_ConfiguracionBackup.sql)
-- (pegar el contenido de bd/22_PermisosBackup.sql)
```

Resultado esperado: tabla `dbo.ConfiguracionBackup` con 1 fila y, en `Permisos`, la fila
`Configuracion.Backup.Gestionar` ligada al rol 1 en `RolPermisos`.

---

## 4. Paso 2 — Carpeta de backups en el servidor (y ruta UNC)

El `BACKUP` lo escribe **el servicio SQL**, por lo que la ruta es **del servidor** (absoluta, local
al servidor). La app necesita además una ruta **UNC** (`\\SERVIDOR\...`) para listar y subir los `.bak`.

> ⚠️ **Primero crear la carpeta, después otorgar permisos.** Si se ejecuta `icacls` sobre una ruta que
> no existe, falla con un mensaje engañoso — `C:\$Recycle.Bin\S-1-5-18\*: Acceso denegado` — que **no**
> describe el problema real: la ruta es incorrecta o todavía no fue creada.

### 4.1 Identificar la cuenta del servicio SQL

La cuenta que escribe el `.bak` es la del **servicio** de la instancia (no la del usuario de la app).
Consultarla con PowerShell **como Administrador**:

```powershell
Get-CimInstance Win32_Service -Filter "Name='MSSQL`$SQLEXPRESS2019'" |
    Select-Object Name, State, StartName
```

Resultado típico (cuenta virtual por instancia):

```
Name                    State   StartName
----                    -----   ---------
MSSQL$SQLEXPRESS2019    Running NT Service\MSSQL$SQLEXPRESS2019
```

> Si el servicio corre con otra cuenta (p. ej. `NT AUTHORITY\NETWORK SERVICE`, `LocalSystem` o un
> usuario de dominio), otorgar el permiso **a esa** cuenta. Alternativa equivalente: el grupo
> `SQLServerMSSQLUser$<EQUIPO>$SQLEXPRESS2019` que crea el Setup.

### 4.2 Crear la carpeta y darle permiso de escritura al servicio SQL

1. En el servidor, crear la carpeta de backups (por ejemplo `D:\Backups`):

   ```powershell
   New-Item -ItemType Directory -Path "D:\Backups" -Force
   ```

2. Otorgar permiso **NTFS Modify** (lectura + escritura) a la cuenta del servicio SQL:

   ```powershell
   icacls "D:\Backups" /grant 'NT SERVICE\MSSQL$SQLEXPRESS2019:(OI)(CI)M'
   ```

   - `(OI)(CI)` = hereda a archivos y subcarpetas · `M` = **Modify**.
   - Usar **comillas simples** para que PowerShell no interprete `$SQLEXPRESS2019` como variable.
   - Agregar `/T` **solo** si la carpeta ya contiene `.bak` con permisos restrictivos (recursivo).

3. Verificar:

   ```powershell
   icacls "D:\Backups"
   ```

   Debe figurar una línea con `NT SERVICE\MSSQL$SQLEXPRESS2019:(OI)(CI)(M)`.

> **NTFS ≠ Share.** Son dos permisos distintos y pueden fallar por separado:
> - **NTFS** → permite que el *servicio SQL* **escriba** el `.bak`.
> - **Share (SMB)** → permite que las *PCs* **lean** por la ruta UNC.
>
> El error típico `Operating system error 5 (Access denied)` en el `BACKUP` es falta de permiso NTFS.

### 4.3 Compartir la carpeta en red (ruta UNC)

1. **Compartir** la carpeta en red con permiso de lectura:

   ```powershell
   New-SmbShare -Name "Backups" -Path "D:\Backups" -ReadAccess "WALTERG20\Usuarios"
   ```

2. Verificar que las PCs la alcanzan: `\\NOMBRESERVIDOR\Backups`
   - La cuenta de Windows que usa la app en cada PC debe poder **leer** esa compartición.

> **Rutas de ejemplo**
> - Ruta en el servidor (para el `BACKUP`): `D:\Backups`
> - Ruta compartida UNC (que lee la app): `\\NOMBRESERVIDOR\Backups`

---

## 5. Paso 3 — Credenciales de Google (Cloud + OAuth) ⭐

El servicio de Drive (`DriveBackupService.cs`) usa Google Drive **API directa** y lee las credenciales
OAuth de un archivo **`credenciales.json`** (tipo *Desktop app*) que se descarga desde Google Cloud y
se **copia junto al ejecutable** (junto a `appsettings.json`). El archivo **no se versiona**
(está en `.gitignore`): cada PC necesita poner el suyo.

Sin ese archivo, la conexión con Drive **no funciona**: al pulsar *Conectar con Google Drive* la app
avisa que no encontró `credenciales.json`.

### 5.1 Crear el proyecto y habilitar la API

1. Entrar a <https://console.cloud.google.com/> y crear (o elegir) un proyecto, p. ej. `TP_ControlVehicular`.
2. Ir a **APIs y servicios → Biblioteca**, buscar **Google Drive API** y pulsar **Habilitar**.

### 5.2 Pantalla de consentimiento OAuth

1. Ir a **APIs y servicios → Pantalla de consentimiento de OAuth**.
2. Tipo de usuario: **Externo**.
3. Completar nombre de la app y correo de soporte.
4. En **Permisos/Scopes**, el único que necesita la app es:
   `https://www.googleapis.com/auth/drive.file`
   (permite ver y administrar **solo los archivos que crea la propia app**).
5. Publicar la app (**Publishing status → In production**). El scope `drive.file` **no es sensible**,
   así que no requiere verificación de Google.
   > Si se deja en **Testing**, solo funcionan las cuentas de "usuarios de prueba" y los tokens
   > caducan a los **7 días** (habría que reconectar seguido).

### 5.3 Descargar el archivo de credenciales

1. Ir a **APIs y servicios → Credenciales → Crear credenciales → Id. de cliente de OAuth**.
2. Tipo de aplicación: **Aplicación de escritorio (Desktop app)**.
3. Pulsar **Descargar JSON**. El archivo se llama algo como `client_secret_XXXX.apps.googleusercontent.com.json`.
4. **Renombrarlo a `credenciales.json`** y colocarlo en la **misma carpeta que el ejecutable** de la app
   (`bin\Debug\net10.0-windows\` en desarrollo, o la carpeta de instalación en producción).

   > Como alternativa, dejarlo en la **raíz del proyecto** (`TP_ControlVehicular\credenciales.json`):
   > el `.csproj` lo copia automáticamente a la carpeta de salida al compilar.

5. Recompilar (si se puso en la raíz) y volver a ejecutar la app:

   ```powershell
   dotnet build "TP_ControlVehicular.slnx"
   ```

> El contenido del JSON incluye el `client_id` y el `client_secret`. Para una app de escritorio el
> secreto es público por definición, pero **no se commitea**: `credenciales.json` está ignorado por git.
> Lo que **no** viaja al repositorio es el **token de refresco**, que se guarda **por-PC y cifrado con DPAPI**
> en `%LocalAppData%\TP_ControlVehicular\Drive`.

---

## 6. Paso 4 — Configurar la app (pantalla "Configuración de Respaldos")

1. Iniciar sesión en la app con un usuario **Admin**.
2. En el menú **Administración**, abrir **🗄️ Configuración de Respaldos**.
3. Completar los campos:

   | Campo | Qué poner | Ejemplo |
   |---|---|---|
   | **Base de datos** | Nombre de la base a respaldar. | `ControlVehicular` |
   | **Ruta en el servidor** | Ruta **absoluta del servidor** donde el servicio SQL escribe el `.bak`. | `D:\Backups` |
   | **Ruta compartida** | Ruta **UNC** que lee la app para listar/subir. | `\\NOMBRESERVIDOR\Backups` |
   | **Carpeta en Drive (opcional)** | **Dejar vacío**: la app crea/busca sola la carpeta `ControlVehicularBackups`. Solo poner un Id si se quiere forzar otra carpeta. | *(vacío)* |
   | **Google Drive** | Botón **🔗 Conectar con Google Drive**. | — |

   > **Carpeta en Drive**: con el campo vacío, la primera vez que se resguarda la app busca en el Drive
   > del usuario la carpeta `ControlVehicularBackups` y, si no existe, **la crea** y guarda su Id en la
   > configuración (queda visible la próxima vez que se abra la pantalla).
   >
   > Para **forzar otra carpeta**, abrirla en el navegador: la URL es
   > `https://drive.google.com/drive/folders/<ID>` → pegar el `<ID>` en el campo.
   > (Con el scope `drive.file` la carpeta debe ser accesible por la cuenta conectada.)

4. Pulsar **🔗 Conectar con Google Drive**: se abre el navegador para iniciar sesión y autorizar.
   Al volver, debe verse **"✅ Conectado con Google Drive en esta PC."**
5. Pulsar **💾 Guardar**.

`Base de datos`, `Ruta en el servidor` y `Ruta compartida` son **obligatorias** (la app valida).

---

## 7. Permisos de SQL del login

Los comandos `BACKUP` y `RESTORE` los ejecuta la app **con la cuenta de conexión** (hoy Windows Auth
según `appsettings.json`). Esa cuenta necesita:

- **Para hacer backup**: permiso `BACKUP DATABASE` (p. ej. rol `db_backupoperator` o `db_owner`).
- **Para probar (restore)**: permiso para **crear bases** (`dbcreator`) y para ejecutar `RESTORE`.

> Los permisos exactos a otorgar en producción se definen en el **despliegue** (ver `docs/referencia/backup.md`,
> sección *Fuera de alcance*). Opciones posibles: rol `db_backupoperator` + `dbcreator`, o SPs firmados por
> certificado, o Windows Auth. **Coordinar con quien administra el servidor** antes de un despliegue real.

### Cadena de conexión

La app lee la conexión de `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NOMBRESERVIDOR\\SQLEXPRESS2019;Database=ControlVehicular;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

En el despliegue, apuntar `Server=` al **servidor central** del taller. El archivo se lee desde la
carpeta de salida del ejecutable (`AppContext.BaseDirectory`).

---

## 8. Paso 5 — Probar el flujo completo

1. **Respaldos** (pantalla 💾) → botón **Hacer Backup** → confirmar.
   Debe aparecer el aviso con el nombre del `.bak` generado y verse en el listado.
2. Seleccionar ese respaldo → **Probar Backup** → confirmar.
   Debe informar **"Prueba exitosa"** con la base `ControlVehicular_Test` en estado **ONLINE** y el
   número de tablas detectadas.
3. Botón **Resguardar** → confirmar.
   Debe subir el último `.bak` a Drive como `ControlVehicular_YYYYMMDD_FULL.bak`.
   Si ya se subió hoy, avisa que no se vuelve a subir.
4. Verificar en Google Drive que el archivo aparece dentro de la carpeta **`ControlVehicularBackups`**
   (creada automáticamente la primera vez).

---

## 9. Solución de problemas

| Síntoma | Causa probable / solución |
|---|---|
| No aparece el menú de Administración | El usuario no es **Admin**; falta ejecutar `bd/22_PermisosBackup.sql`. |
| "Debe configurar la Ruta de Backup del servidor..." | El campo **Ruta en el servidor** está vacío: guardar la configuración. |
| El listado está vacío | La **Ruta compartida (UNC)** no existe o la PC no tiene permiso de lectura sobre la compartición. |
| Error al hacer backup | La cuenta de conexión no tiene permiso `BACKUP DATABASE`, o el servicio SQL no puede escribir en la carpeta (permiso NTFS). |
| *"BACKUP DATABASE WITH COMPRESSION is not supported on Express Edition (64-bit)"* | La opción `COMPRESSION` no existe en **SQL Server Express** (solo Standard/Enterprise). La app ya no la usa; si el error aparece, la base es otra versión de la app o se copió la sentencia a mano. Ver §4 en `docs/referencia/backup.md`. |
| `icacls` responde `C:\$Recycle.Bin\S-1-5-18\*: Acceso denegado` | La ruta pasada a `icacls` **no existe** (o está mal escrita). Crear primero la carpeta y recién después otorgar el permiso (ver §4.2). |
| La prueba (restore) falla | Falta permiso `dbcreator`/creación de base, o el `.bak` no es accesible desde el servidor. |
| "Debe conectar con Google Drive antes de resguardar" | No se hizo **Conectar con Google Drive** (falta §6), o no existe `credenciales.json` junto al ejecutable (falta §5.3). |
| "No se encontró el archivo de credenciales... credenciales.json" | El JSON de Google Cloud no está junto al ejecutable (o con el nombre correcto). Ver §5.3. |
| Se pide reconectar a Google cada semana | La pantalla de consentimiento está en **Testing**; publicarla (**In production**) para tokens duraderos. |
| Error de tipo "acceso denegado" al subir a Drive | El scope `drive.file` solo permite ver/administrar archivos creados por la app. La carpeta destino debe ser accesible por la cuenta conectada (dejar el campo vacío para que la app cree `ControlVehicularBackups`). |

---

## 10. Lo que todavía NO incluye (fases posteriores)

- **Cifrado** del archivo `.bak`.
- **Agendado / automatización** del respaldo (Task Scheduler + `sqlcmd`): hoy el backup es **manual** desde la app.
- **Permisos de SQL definitivos** para el login de backup/restore (se resuelven en el despliegue).
- Backup **diferencial / logarítmico** y **retención** (GFS).
- Instalador y alta automática de base/datos por defecto.

---

## Referencias rápidas

- Dominio y diseño: `docs/referencia/backup.md`
- Scripts de base de datos: `bd/21_ConfiguracionBackup.sql`, `bd/22_PermisosBackup.sql`
- Servicios: `Negocio/Services/DatabaseBackupService.cs`, `Negocio/Services/DriveBackupService.cs`, `Negocio/Services/BackupConfiguracionService.cs`
- Pantallas: `Presentacion/Pantalla/Configuracion/CtlBackup.xaml(.cs)`, `Presentacion/Pantalla/Configuracion/CtlBackupConfiguracion.xaml(.cs)`
- Plan/bitácora de la spec: `docs/plan/2026-10-11_backup-drive.md`
