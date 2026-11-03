param(
    [string]$Server = "localhost,1433",
    [string]$Database = "anirok2",
    [string]$User = "sa",
    [string]$Password = "Anirok_dev_2026!"
)

$ErrorActionPreference = "Stop"
$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd) { throw "sqlcmd no está instalado o no está en PATH." }

function Invoke-Scalar([string]$Query) {
    $value = & $sqlcmd.Source -S $Server -U $User -P $Password -d $Database -C -h -1 -W -b -Q $Query
    if ($LASTEXITCODE -ne 0) { throw "Falló la consulta: $Query" }
    return (($value | Out-String).Trim())
}

function Assert-Equal([string]$Name, [string]$Expected, [string]$Actual) {
    if ($Expected -ne $Actual) { throw "${Name}: esperado '$Expected', obtenido '$Actual'" }
    Write-Host "OK - $Name"
}

Assert-Equal "Base accesible" "1" (Invoke-Scalar "SELECT 1")
Assert-Equal "Tabla contacto" "1" (Invoke-Scalar "SELECT CASE WHEN OBJECT_ID('dbo.contacto','U') IS NULL THEN 0 ELSE 1 END")
Assert-Equal "Tabla email_envio" "1" (Invoke-Scalar "SELECT CASE WHEN OBJECT_ID('dbo.email_envio','U') IS NULL THEN 0 ELSE 1 END")
Assert-Equal "Tabla proveedor" "1" (Invoke-Scalar "SELECT CASE WHEN OBJECT_ID('dbo.proveedor','U') IS NULL THEN 0 ELSE 1 END")
Assert-Equal "Precio decimal" "decimal" (Invoke-Scalar "SELECT TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.producto') AND name='precio'")
Assert-Equal "Detalle precio decimal" "decimal" (Invoke-Scalar "SELECT TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.detalle_venta') AND name='precio_unitario'")
Assert-Equal "Usuarios de prueba" "1" (Invoke-Scalar "SELECT CASE WHEN EXISTS(SELECT 1 FROM dbo.usuario WHERE nombre='Gianni') THEN 1 ELSE 0 END")
Assert-Equal "Sin claves planas de prueba" "0" (Invoke-Scalar "SELECT COUNT(*) FROM dbo.usuario WHERE contraseña IN ('gianni123','vic345')")
Write-Host "Smoke tests completados correctamente."
