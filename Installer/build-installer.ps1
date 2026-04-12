# VortexTrade Installer Build Script
# Requires: .NET 10 SDK, InnoSetup 6+
# Usage: .\build-installer.ps1

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$CsprojPath = Join-Path $ProjectRoot "VortexTrade.csproj"
$PublishDir = Join-Path $ScriptDir "publish"
$OutputDir = Join-Path $ScriptDir "Output"
$SetupScript = Join-Path $ScriptDir "setup.iss"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  VortexTrade Installer Builder" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

# Extract version from .csproj
[xml]$csproj = Get-Content $CsprojPath
$version = $csproj.Project.PropertyGroup.Version
if (-not $version) {
    Write-Host "ERROR: Version not found in csproj" -ForegroundColor Red
    exit 1
}
Write-Host "Version: $version" -ForegroundColor Cyan

# Clean previous builds
Write-Host "`n[1/4] Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $PublishDir) { Remove-Item -Recurse -Force $PublishDir }
if (Test-Path $OutputDir) { Remove-Item -Recurse -Force $OutputDir }

# Publish self-contained single-file
Write-Host "[2/4] Publishing self-contained app ($Runtime)..." -ForegroundColor Yellow
dotnet publish $CsprojPath `
    --configuration $Configuration `
    --runtime $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    --output $PublishDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet publish failed" -ForegroundColor Red
    exit 1
}

Write-Host "Published to: $PublishDir" -ForegroundColor Cyan

# Find InnoSetup compiler
Write-Host "[3/4] Locating InnoSetup compiler..." -ForegroundColor Yellow
$isccPaths = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe"
)

$iscc = $null
foreach ($path in $isccPaths) {
    if (Test-Path $path) {
        $iscc = $path
        break
    }
}

if (-not $iscc) {
    Write-Host "ERROR: InnoSetup 6 ISCC.exe not found!" -ForegroundColor Red
    Write-Host "Please install InnoSetup 6 from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host "Published files are available at: $PublishDir" -ForegroundColor Yellow
    exit 1
}

Write-Host "Found ISCC: $iscc" -ForegroundColor Cyan

# Compile installer
Write-Host "[4/4] Compiling installer..." -ForegroundColor Yellow
& $iscc "/DMyAppVersion=$version" $SetupScript

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: InnoSetup compilation failed" -ForegroundColor Red
    exit 1
}

$setupExe = Join-Path $OutputDir "VortexTrade_Setup_v${version}.exe"
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  Build Complete!" -ForegroundColor Green
Write-Host "  Installer: $setupExe" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Green
