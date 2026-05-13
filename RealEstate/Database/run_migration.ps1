# ═══════════════════════════════════════════════════════════════
# EstateFlow Database Migration Script
# This script will run the comprehensive SQL migration
# ═══════════════════════════════════════════════════════════════

Write-Host "══════════════════════════════════════════════════════════=" -ForegroundColor Cyan
Write-Host "  EstateFlow Database Migration" -ForegroundColor Cyan
Write-Host "══════════════════════════════════════════════════════════=" -ForegroundColor Cyan
Write-Host ""

# Configuration
$sqlServer = "LAPTOP-GBP34Q5H\SQLEXPRESS"
$databaseName = "DB_Real_Estate"
$sqlScriptPath = "c:\Users\ADMIN\source\repos\real\RealEstate\Database\COMPREHENSIVE_SCHEMA_MIGRATION.sql"

Write-Host "Database Server: $sqlServer" -ForegroundColor Yellow
Write-Host "Database Name:   $databaseName" -ForegroundColor Yellow
Write-Host "SQL Script:      $sqlScriptPath" -ForegroundColor Yellow
Write-Host ""

# Check if SQL script exists
if (-not (Test-Path $sqlScriptPath)) {
    Write-Host "❌ ERROR: SQL script not found at: $sqlScriptPath" -ForegroundColor Red
    exit 1
}

# Read and update the SQL script
Write-Host "📝 Preparing SQL script..." -ForegroundColor Green
$sqlContent = Get-Content $sqlScriptPath -Raw
$sqlContent = $sqlContent -replace "USE \[YourDatabaseName\];", "USE [$databaseName];"

# Create temporary script with correct database name
$tempScriptPath = [System.IO.Path]::GetTempFileName() + ".sql"
$sqlContent | Out-File -FilePath $tempScriptPath -Encoding UTF8

Write-Host "✅ SQL script prepared" -ForegroundColor Green
Write-Host ""

# Try to run the SQL script using sqlcmd
Write-Host "🚀 Running migration..." -ForegroundColor Green
Write-Host ""

try {
    $process = Start-Process -FilePath "sqlcmd" -ArgumentList "-S `"$sqlServer`"", "-d `"$databaseName`"", "-E", "-i `"$tempScriptPath`"", "-b" -NoNewWindow -Wait -PassThru
    
    if ($process.ExitCode -eq 0) {
        Write-Host ""
        Write-Host "══════════════════════════════════════════════════════════=" -ForegroundColor Green
        Write-Host "  ✅ Migration completed successfully!" -ForegroundColor Green
        Write-Host "══════════════════════════════════════════════════════════=" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "⚠️  sqlcmd exited with code: $($process.ExitCode)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "This might mean:" -ForegroundColor Yellow
        Write-Host "  1. sqlcmd is not installed (SQL Server tools needed)" -ForegroundColor Yellow
        Write-Host "  2. Database connection failed" -ForegroundColor Yellow
        Write-Host "  3. Some tables already exist (this is OK)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Alternative: Open SSMS and run the script manually:" -ForegroundColor Cyan
        Write-Host "  1. Open SQL Server Management Studio" -ForegroundColor White
        Write-Host "  2. Connect to: $sqlServer" -ForegroundColor White
        Write-Host "  3. Open file: $sqlScriptPath" -ForegroundColor White
        Write-Host "  4. Press F5 to execute" -ForegroundColor White
    }
} catch {
    Write-Host ""
    Write-Host "❌ Error running sqlcmd: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Open SSMS and run the script manually:" -ForegroundColor Cyan
    Write-Host "  1. Open SQL Server Management Studio" -ForegroundColor White
    Write-Host "  2. Connect to: $sqlServer" -ForegroundColor White
    Write-Host "  3. Open file: $sqlScriptPath" -ForegroundColor White
    Write-Host "  4. Press F5 to execute" -ForegroundColor White
}

# Cleanup temp file
if (Test-Path $tempScriptPath) {
    Remove-Item $tempScriptPath -Force
}

Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
