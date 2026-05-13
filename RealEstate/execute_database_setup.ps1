# PowerShell script to execute SQL on remote database
# Database connection details from appsettings.json

$server = "db49649.public.databaseasp.net"
$database = "db49649"
$username = "db49649"
$password = "eP_72hC-=9wK"

# SQL script file path
$sqlFile = "c:\Users\ADMIN\source\repos\real\RealEstate\reset_and_setup_database.sql"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Database Setup Script Executor" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Server: $server" -ForegroundColor Yellow
Write-Host "Database: $database" -ForegroundColor Yellow
Write-Host "User: $username" -ForegroundColor Yellow
Write-Host ""
Write-Host "Reading SQL script..." -ForegroundColor Green

# Read the SQL script
$sqlScript = Get-Content -Path $sqlFile -Raw

Write-Host "SQL script loaded successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Attempting to connect to database..." -ForegroundColor Yellow
Write-Host ""

# Create connection string
$connectionString = "Server=$server;Database=$database;User Id=$username;Password=$password;Encrypt=True;TrustServerCertificate=True;"

try {
    # Load SQL Server assembly
    Add-Type -AssemblyName "Microsoft.Data.SqlServer" | Out-Null
    
    # Create connection
    $connection = New-Object Microsoft.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Executing SQL script..." -ForegroundColor Yellow
    Write-Host ""
    
    # Create command
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.CommandTimeout = 120
    
    # Execute and capture output
    $reader = $command.ExecuteReader()
    
    # Process results
    while ($reader.Read()) {
        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
            Write-Host "$($reader.GetName($i)): $($reader[$i])" -ForegroundColor White
        }
        Write-Host ""
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host "Script executed successfully!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Cyan
    
} catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Please run the SQL script manually using:" -ForegroundColor Yellow
    Write-Host "1. SQL Server Management Studio (SSMS)" -ForegroundColor Yellow
    Write-Host "2. Azure Data Studio" -ForegroundColor Yellow
    Write-Host "3. Your hosting provider's web interface" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Script location: $sqlFile" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
