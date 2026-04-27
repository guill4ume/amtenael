$results = @()
$realms = "Albion", "Hibernia", "Midgard"

foreach ($realm in $realms) {
    $files = Get-ChildItem -Path "./$realm/*.md"
    foreach ($file in $files) {
        $className = $file.BaseName
        $content = Get-Content $file.FullName
        
        $inTable = $false
        $headers = @()
        
        foreach ($line in $content) {
            if ($line -match "^\|") {
                $cells = $line.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }
                
                if ($line -match "Level" -and $line -match "Name") {
                    $headers = $cells
                    $inTable = $true
                    continue
                }
                
                if ($inTable -and $line -match "---") { continue }
                
                if ($inTable -and $cells.Count -ge $headers.Count) {
                    $prereqIndex = [array]::IndexOf($headers, "Prerequisite")
                    if ($prereqIndex -ge 0) {
                        $prereq = $cells[$prereqIndex]
                        if ($prereq -match "parry|block|evade") {
                            $styleName = $cells[[array]::IndexOf($headers, "Name")]
                            $results += [PSCustomObject]@{
                                Realm = $realm
                                Class = $className
                                Style = $styleName
                                Requirement = $prereq
                            }
                        }
                    }
                }
            } else {
                $inTable = $false
            }
        }
    }
}

$results | Export-Csv -Path "requirements_audit.csv" -NoTypeInformation
$results | Format-Table -AutoSize
