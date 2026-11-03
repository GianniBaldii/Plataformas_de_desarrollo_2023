$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
Set-Location $repo

# Carga opcional de credenciales locales de SMTP. El archivo está ignorado por
# Git para que nunca se publique una contraseña.
$smtpFile = Join-Path $repo '.env.local'
if (Test-Path $smtpFile) {
    Get-Content $smtpFile | ForEach-Object {
        $line = $_.Trim()
        if ($line -and -not $line.StartsWith('#') -and $line.Contains('=')) {
            $parts = $line.Split('=', 2)
            [Environment]::SetEnvironmentVariable($parts[0].Trim(), $parts[1].Trim(), 'Process')
        }
    }
    Write-Host 'Configuración SMTP local cargada.' -ForegroundColor Green
}

Write-Host '== Anirok: iniciando base y entorno ==' -ForegroundColor Cyan

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw 'Docker no está instalado o no está disponible en PATH.'
}

docker info *> $null
if ($LASTEXITCODE -ne 0) {
    throw 'Docker Desktop no está iniciado. Abrilo y ejecutá nuevamente este script.'
}

docker compose up -d sqlserver
if ($LASTEXITCODE -ne 0) {
    throw 'No se pudo iniciar Docker Compose.'
}

$deadline = (Get-Date).AddMinutes(2)
do {
    $status = docker inspect --format '{{.State.Health.Status}}' anirok-sqlserver 2>$null
    if ($status -eq 'healthy') { break }
    if ((Get-Date) -gt $deadline) {
        docker compose ps
        throw 'SQL Server no llegó al estado healthy dentro del tiempo esperado.'
    }
    Start-Sleep -Seconds 3
} while ($true)

# db-init es un servicio de una sola ejecución. Se fuerza su recreación para
# que las migraciones nuevas también se apliquen cuando la base ya existe.
docker compose up --force-recreate --abort-on-container-exit db-init
if ($LASTEXITCODE -ne 0) {
    throw 'No se pudieron aplicar las migraciones de la base.'
}

Write-Host 'SQL Server está listo en localhost:1433.' -ForegroundColor Green
docker compose ps

# Evita que Visual Studio herede una cadena antigua de LocalDB o una contraseña
# anterior. Ambas aplicaciones usan esta misma conexión de desarrollo.
$env:ANIROK_DB_CONNECTION = 'Server=localhost,1433;Database=anirok2;User Id=sa;Password=Anirok_dev_2026!;TrustServerCertificate=True'

$solution = Join-Path $repo 'Anirok\EjemploABM.sln'
$devenv = 'C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe'
if (-not (Test-Path $devenv)) {
    throw "No se encontró Visual Studio Community 2022 en: $devenv"
}

Write-Host 'Abriendo Visual Studio...' -ForegroundColor Cyan
Start-Process -FilePath $devenv -ArgumentList "`"$solution`"" -Verb RunAs
Write-Host ''
Write-Host 'En Visual Studio: seleccioná AnirokWeb/IIS Express y presioná Ctrl+F5.' -ForegroundColor Yellow
Write-Host 'Web: http://localhost:64726/' -ForegroundColor Yellow
Write-Host 'Desktop: ejecutá EjemploABM desde la misma solución.' -ForegroundColor Yellow
