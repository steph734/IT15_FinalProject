@echo off
echo ============================================================
echo   EstateFlow Database Migration
echo ============================================================
echo.

echo Database: DB_Real_Estate
echo Server: LAPTOP-GBP34Q5H\SQLEXPRESS
echo.

echo Running SQL migration script...
echo.

sqlcmd -S "LAPTOP-GBP34Q5H\SQLEXPRESS" -d "DB_Real_Estate" -E -i "COMPREHENSIVE_SCHEMA_MIGRATION.sql" -b

if %errorlevel% equ 0 (
    echo.
    echo ============================================================
    echo   SUCCESS! Migration completed!
    echo ============================================================
) else (
    echo.
    echo ============================================================
    echo   Note: Some tables may already exist (this is OK)
    echo   Or sqlcmd is not installed - use SSMS instead
    echo ============================================================
    echo.
    echo To run manually:
    echo   1. Open SQL Server Management Studio
    echo   2. Connect to: LAPTOP-GBP34Q5H\SQLEXPRESS
    echo   3. Open: COMPREHENSIVE_SCHEMA_MIGRATION.sql
    echo   4. Press F5
    echo ============================================================
)

echo.
pause
