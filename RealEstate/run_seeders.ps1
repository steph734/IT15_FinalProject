# PowerShell script to apply migrations and seed database
# Run this to set up your database with seed data

param(
    [string]$ConnectionString = "",
    [switch]$ResetDatabase = $false
)

Write-Host "=== EstateFlow Database Seeder ===" -ForegroundColor Cyan

# Check if dotnet ef is installed
$efTools = dotnet ef --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

# Update connection string if provided
if ($ConnectionString -ne "") {
    Write-Host "Updating connection string..." -ForegroundColor Yellow
    $appSettings = Get-Content appsettings.json | ConvertFrom-Json
    $appSettings.ConnectionStrings.DefaultConnection = $ConnectionString
    $appSettings | ConvertTo-Json -Depth 10 | Set-Content appsettings.json
    Write-Host "Connection string updated!" -ForegroundColor Green
}

# Reset database if requested
if ($ResetDatabase) {
    Write-Host "Resetting database..." -ForegroundColor Red
    dotnet ef database drop --force --yes
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to drop database. It may not exist yet." -ForegroundColor Yellow
    }
}

# Apply migrations (this also runs seeders)
Write-Host "Applying migrations and seeders..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "" 
    Write-Host "=== SUCCESS! ===" -ForegroundColor Green
    Write-Host "Database seeded with the following users:" -ForegroundColor Cyan
    Write-Host "  - superadmin / Admin@123" -ForegroundColor White
    Write-Host "  - manager / Manager@123" -ForegroundColor White  
    Write-Host "  - broker / Broker@123" -ForegroundColor White
    Write-Host "  - seller / Seller@123" -ForegroundColor White
    Write-Host "  - accounting / Accounting@123" -ForegroundColor White
    Write-Host ""
    Write-Host "You can now log in to the application." -ForegroundColor Green
} else {
    Write-Host "=== FAILED ===" -ForegroundColor Red
    Write-Host "Migration failed. Check the error messages above." -ForegroundColor Red
}
