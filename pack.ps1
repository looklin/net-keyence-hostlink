# NuGet One-Click Pack Script
# Usage: .\pack.ps1

param(
    [switch]$SkipPack,
    [switch]$Clean
)

$configPath = Join-Path $PSScriptRoot "pack-config.props"
[xml]$config = Get-Content $configPath

$stableVersion = $config.Project.PropertyGroup.StableVersion
$enhancedVersion = $config.Project.PropertyGroup.EnhancedVersion
$packageId = $config.Project.PropertyGroup.PackageId

$projectPath = Join-Path $PSScriptRoot "src\Keyence.HostLink\Keyence.HostLink.csproj"
$outputDir = Join-Path $PSScriptRoot "nupkgs"

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  NuGet Pack Tool" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Package: $packageId" -ForegroundColor Yellow
Write-Host "  Stable:  $stableVersion" -ForegroundColor Green
Write-Host "  Enhanced: $enhancedVersion" -ForegroundColor Magenta
Write-Host ""

if ($Clean -or (Test-Path $outputDir)) {
    if (Test-Path $outputDir) {
        Remove-Item $outputDir -Recurse -Force
        Write-Host "[Clean] Removed old output directory" -ForegroundColor DarkGray
    }
}
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

Write-Host "[1/3] Building project..." -ForegroundColor Yellow
& dotnet build $projectPath -c Release --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "[Error] Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "[1/3] Build completed" -ForegroundColor Green
Write-Host ""

if ($SkipPack) {
    Write-Host "[Skip] Pack step skipped" -ForegroundColor DarkGray
    exit 0
}

Write-Host "[2/3] Packing Stable v$stableVersion ..." -ForegroundColor Green
& dotnet pack $projectPath -c Release /p:PackageVersion=$stableVersion /p:Version=$stableVersion /p:PackageReleaseNotes="Stable release recommended for production use." --no-build --nologo -o $outputDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "[Error] Stable pack failed!" -ForegroundColor Red
    exit 1
}
Write-Host "[2/3] Stable pack completed" -ForegroundColor Green
Write-Host ""

Write-Host "[3/3] Packing Enhanced v$enhancedVersion ..." -ForegroundColor Magenta
& dotnet pack $projectPath -c Release /p:PackageVersion=$enhancedVersion /p:Version=$enhancedVersion /p:PackageReleaseNotes="Enhanced version with latest features and optimizations for .NET 6+." --no-build --nologo -o $outputDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "[Error] Enhanced pack failed!" -ForegroundColor Red
    exit 1
}
Write-Host "[3/3] Enhanced pack completed" -ForegroundColor Magenta
Write-Host ""

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  Pack Complete!" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem $outputDir -Filter "*.nupkg"
foreach ($file in $files) {
    $sizeKB = [math]::Round($file.Length / 1KB, 1)
    Write-Host "  [$($file.Name)]" -ForegroundColor White
    Write-Host "    Path: $($file.FullName)" -ForegroundColor DarkGray
    Write-Host "    Size: ${sizeKB} KB" -ForegroundColor DarkGray
    Write-Host ""
}

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  Publish Commands:" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  dotnet nuget push `"$outputDir\$packageId.$stableVersion.nupkg`" -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json" -ForegroundColor DarkGray
Write-Host "  dotnet nuget push `"$outputDir\$packageId.$enhancedVersion.nupkg`" -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json" -ForegroundColor DarkGray
Write-Host ""
