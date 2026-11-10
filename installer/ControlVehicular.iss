; ControlVehicular - Instalador (Inno Setup)
; Requiere: .NET 10 Desktop Runtime, SQL Server Express LocalDB.
; Compilar con Inno Setup 6.x.

#define MyAppName "Control Vehicular"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "TP_ControlVehicular"
#define MyAppExeName "TP_ControlVehicular.exe"
#define MyAppSource "E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\bin\Release\net10.0-windows"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=.\Output
OutputBaseFilename=ControlVehicular_Setup_{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Files]
; Aplicacion
Source: "{#MyAppSource}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Scripts de base de datos
Source: "..\bd\*"; DestDir: "{app}\bd"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Crear acceso directo en el escritorio"; GroupDescription: "Accesos directos:"

[Run]
; 1. Instalar SQL Server LocalDB si no esta presente (silencioso)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""if (-not (Get-Command sqllocaldb -ErrorAction SilentlyContinue)) { Write-Host 'LocalDB no encontrado. Instale SQL Server Express LocalDB manualmente.' }"""; Flags: runhidden waituntilterminated
; 2. Crear/actualizar la base de datos (bootstrap + seed + SPs + triggers)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\bd\instalar.ps1"" -Servidor ""(localdb)\MSSQLLocalDB"" -BaseDatos ""ControlVehicular"""; Flags: runhidden waituntilterminated
; 3. Lanzar la aplicacion
Filename: "{app}\{#MyAppExeName}"; Description: "Lanzar {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"