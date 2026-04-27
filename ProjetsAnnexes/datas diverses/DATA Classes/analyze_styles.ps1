$data = Import-Csv requirements_audit.csv
$grouped = $data | Group-Object Class
$results = @()

foreach ($g in $grouped) {
    $reqs = $g.Group.Requirement | Select-Object -Unique
    $parry = $reqs -contains "You parry"
    $block = $reqs -contains "You block"
    $evade = $reqs -contains "You evade"
    
    $results += [PSCustomObject]@{
        Class = $g.Name
        Realm = $g.Group[0].Realm
        NeedsParry = $parry
        NeedsBlock = $block
        NeedsEvade = $evade
    }
}

$results | Out-File -FilePath "style_matrix.txt"
$results | Format-Table -AutoSize
