# instalar.ps1
# Orquesta la creacion de la base de datos ControlVehicular en SQL Server LocalDB/Express.
# Uso: .\instalar.ps1 [-Servidor "(localdb)\MSSQLLocalDB"] [-BaseDatos "ControlVehicular"]
param(
    [string]$Servidor = "(localdb)\MSSQLLocalDB",
    [string]$BaseDatos = "ControlVehicular"
)

$ErrorActionPreference = "Stop"
$dir = Split-Path -Parent $MyInvocation.MyCommand.Path

function Invoke-SqlFile {
    param([string]$Archivo)
    Write-Host ">> Ejecutando $Archivo ..." -ForegroundColor Cyan
    sqlcmd -S $Servidor -d master -i (Join-Path $dir $Archivo) -b
    if ($LASTEXITCODE -ne 0) { throw "Fallo al ejecutar $Archivo (exit $LASTEXITCODE)" }
}

# 0. Bootstrap (crea la BD)
Invoke-SqlFile "00_bootstrap.sql"

# 1. Estructura
sqlcmd -S $Servidor -d $BaseDatos -i (Join-Path $dir "01_estructura.sql") -b
if ($LASTEXITCODE -ne 0) { throw "Fallo 01_estructura.sql" }

# 2. Seed base (roles, permisos, matriz, taller, configs, admin, servicios, marcas, modelos)
sqlcmd -S $Servidor -d $BaseDatos -i (Join-Path $dir "02_datos.sql") -b
if ($LASTEXITCODE -ne 0) { throw "Fallo 02_datos.sql" }

# 3. Triggers
sqlcmd -S $Servidor -d $BaseDatos -i (Join-Path $dir "04_triggers.sql") -b
if ($LASTEXITCODE -ne 0) { throw "Fallo 04_triggers.sql" }

# 4. Procedimientos
sqlcmd -S $Servidor -d $BaseDatos -i (Join-Path $dir "03_procedimientos.sql") -b
if ($LASTEXITCODE -ne 0) { throw "Fallo 03_procedimientos.sql" }

Write-Host ""
Write-Host "Instalacion de base de datos completada." -ForegroundColor Green
Write-Host "Base: $BaseDatos en $Servidor"
Write-Host "NOTA: 05_utilidades_test.sql y 06_poblacion.sql son opcionales (solo dev/demo)."