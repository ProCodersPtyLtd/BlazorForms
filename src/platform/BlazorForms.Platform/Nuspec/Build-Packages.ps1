# PowerShell script to build BlazorForms NuGet packages
# Uses nuget.exe with .nuspec files to ensure all required DLLs are included
param([string]$Version = "1.14.2-preview.3")

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$OutputDir = Join-Path $ScriptDir "Nuget"
$SrcRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $ScriptDir))

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Building BlazorForms NuGet Packages v$Version" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# First, build the entire solution to ensure all dependencies are available
Write-Host "Building BlazorForms solution to ensure all dependencies..." -ForegroundColor Yellow
$solutionPath = Join-Path $SrcRoot "BlazorForms.Framework.sln"
Write-Host "Solution path: $solutionPath" -ForegroundColor Gray
& dotnet build $solutionPath --configuration Release --verbosity minimal
$buildSuccess = ($LASTEXITCODE -eq 0)

if (-not $buildSuccess) {
    Write-Host "✗ Solution build failed. Cannot continue." -ForegroundColor Red
    exit 1
}
Write-Host "✓ Solution built successfully" -ForegroundColor Green
Write-Host ""

# Build the main BlazorForms package using nuspec (includes all DLLs)
Write-Host "Building BlazorForms package (with all DLLs)..." -ForegroundColor Green
$nuspecPath = Join-Path $ScriptDir "BlazorForms.nuspec"
Write-Host "Using nuspec: $nuspecPath" -ForegroundColor Gray
$result1 = & ".\nuget.exe" pack $nuspecPath -OutputDirectory $OutputDir -Version $Version -Verbosity normal
$success1 = ($LASTEXITCODE -eq 0)

# Build the BlazorForms.Cosmos package using nuspec file
Write-Host "Building BlazorForms.Cosmos package..." -ForegroundColor Green  
$cosmosNuspecPath = Join-Path $ScriptDir "BlazorForms.Cosmos.nuspec"
Write-Host "Using nuspec: $cosmosNuspecPath" -ForegroundColor Gray
$result2 = & ".\nuget.exe" pack $cosmosNuspecPath -OutputDirectory $OutputDir -Version $Version -Verbosity normal
$success2 = ($LASTEXITCODE -eq 0)

Write-Host ""
if ($success1 -and $success2) {
    Write-Host "All packages built successfully!" -ForegroundColor Green
} else {
    Write-Host "Some packages failed to build." -ForegroundColor Red
}

Write-Host ""
Write-Host "Generated packages:" -ForegroundColor Yellow
Get-ChildItem -Path $OutputDir -Filter "*.nupkg" | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor White }
