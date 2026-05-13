param(
    [Parameter(Mandatory = $false)]
    [string]$RepoPath = "c:\Users\ADMIN\source\repos\real\RealEstate"
)

$ErrorActionPreference = "Stop"

$snap = Join-Path $RepoPath "Migrations\ApplicationDBContextModelSnapshot.cs"
if (!(Test-Path $snap)) {
    throw "Snapshot file not found: $snap"
}

$outCsv = Join-Path $RepoPath "DataDictionary.csv"
$outMd  = Join-Path $RepoPath "DataDictionary.md"

$lines = Get-Content -Path $snap

$rows = New-Object System.Collections.Generic.List[object]

$currentEntity = ""
$currentTable = ""
$currentPk = ""
$fkPrincipalEntity = ""

function Set-TableForEntityRows([string]$entity, [string]$table) {
    foreach ($r in $rows) {
        if ($r.Entity -eq $entity -and ([string]::IsNullOrWhiteSpace($r.Table))) {
            $r.Table = $table
        }
    }
}

function Set-PkForColumn([string]$entity, [string]$column) {
    foreach ($r in $rows) {
        if ($r.Entity -eq $entity -and $r.Column -eq $column) {
            $r.IsPrimaryKey = $true
        }
    }
}

function Set-FkForColumn([string]$entity, [string]$column, [string]$principalEntity) {
    foreach ($r in $rows) {
        if ($r.Entity -eq $entity -and $r.Column -eq $column) {
            $r.ForeignKeyEntity = $principalEntity
        }
    }
}

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]

    # IMPORTANT: In the snapshot, the opening "{" is commonly on the next line.
    # So we detect the entity start without requiring "{"
    if ($line -match 'modelBuilder\.Entity\("([^"]+)",\s*b\s*=>') {
        $currentEntity = $Matches[1]
        $currentTable = ""
        $currentPk = ""
        $fkPrincipalEntity = ""
        continue
    }

    if ($line -match 'b\.ToTable\("([^"]+)"\)') {
        $currentTable = $Matches[1]
        Set-TableForEntityRows -entity $currentEntity -table $currentTable
        continue
    }

    if ($line -match 'b\.HasKey\("([^"]+)"\)') {
        $currentPk = $Matches[1]
        Set-PkForColumn -entity $currentEntity -column $currentPk
        continue
    }

    # Track principal entity for HasForeignKey(...) parsing
    if ($line -match 'b\.HasOne\("([^"]+)"') {
        $fkPrincipalEntity = $Matches[1]
        continue
    }

    if ($line -match 'HasForeignKey\("([^"]+)"\)') {
        $fkCol = $Matches[1]
        if (-not [string]::IsNullOrWhiteSpace($fkPrincipalEntity)) {
            Set-FkForColumn -entity $currentEntity -column $fkCol -principalEntity $fkPrincipalEntity
        }
        continue
    }

    if ($line -match 'b\.Property<([^>]+)>\("([^"]+)"\)') {
        $clr = ($Matches[1]).Trim()
        $col = $Matches[2]

        $required = $false
        $maxLen = ""
        $dbType = ""

        # Look ahead for IsRequired / HasMaxLength / HasColumnType in the chain
        $j = $i + 1
        while ($j -lt $lines.Count) {
            $l2 = $lines[$j]

            # Stop if next property/entity starts
            if ($l2 -match 'b\.Property<' -or $l2 -match 'modelBuilder\.Entity\("') { break }
            if ($l2 -match 'b\.HasKey\("') { break }
            if ($l2 -match 'b\.ToTable\("') { break }

            if ($l2 -match 'IsRequired\(\)') { $required = $true }
            if ($l2 -match 'HasMaxLength\((\d+)' ) { $maxLen = $Matches[1] }
            if ($l2 -match 'HasColumnType\("([^"]+)"\)') { $dbType = $Matches[1]; $i = $j; break }

            # Fallback if HasColumnType uses a semicolon on the same line
            if ($l2 -match 'HasColumnType\("([^"]+)"\);') { $dbType = $Matches[1]; $i = $j; break }

            $j++
        }

        # Some snapshots might keep HasColumnType on the same line
        if ([string]::IsNullOrWhiteSpace($dbType) -and $line -match 'HasColumnType\("([^"]+)"\)') {
            $dbType = $Matches[1]
        }

        $isNullable = -not $required

        $rows.Add([pscustomobject]@{
            Entity = $currentEntity
            Table  = $currentTable # likely empty until ToTable() appears later in the block
            Column = $col
            CLRType = $clr
            DbType = $dbType
            MaxLength = $maxLen
            IsNullable = $isNullable
            IsPrimaryKey = $false
            ForeignKeyEntity = ""
        })

        continue
    }
}

# Ensure any rows that still lack Table get it as Entity (best-effort)
foreach ($r in $rows) {
    if ([string]::IsNullOrWhiteSpace($r.Table)) {
        $r.Table = $r.Entity
    }
}

$dedup = $rows |
    Select-Object Table, Column, CLRType, DbType, MaxLength, IsNullable, IsPrimaryKey, ForeignKeyEntity, Entity |
    Sort-Object Table, Column, CLRType, DbType, IsPrimaryKey -Descending |
    Group-Object { "$($_.Table)|$($_.Column)" } |
    ForEach-Object { $_.Group | Select-Object -First 1 }

$export = $dedup |
    Select-Object Table, Column, CLRType, DbType, MaxLength, IsNullable, IsPrimaryKey, ForeignKeyEntity |
    Sort-Object Table, Column

$export | Export-Csv -NoTypeInformation -Encoding UTF8 -Path $outCsv

"# Data Dictionary`n`nGenerated from `"$snap`" (EF Core model snapshot).`n`nCSV: `"$outCsv`"`n`nRows: $($export.Count)`n" |
    Out-File -Encoding UTF8 -FilePath $outMd

Write-Output "Wrote: $outCsv"
Write-Output "Wrote: $outMd"

