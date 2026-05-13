function Get-SHA256Hash {
    param([string]$text)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($text)
    $hash = $sha256.ComputeHash($bytes)
    return [Convert]::ToBase64String($hash)
}

$h1 = Get-SHA256Hash 'Manager@123'
$h2 = Get-SHA256Hash 'Broker@123'
$h3 = Get-SHA256Hash 'Seller@123'
$h4 = Get-SHA256Hash 'Accounting@123'

Write-Host "Manager@123    = $h1"
Write-Host "Broker@123     = $h2"
Write-Host "Seller@123     = $h3"
Write-Host "Accounting@123 = $h4"
