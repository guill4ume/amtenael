$lines = Get-Content 'C:\OpenDAOC_server\ProjetsAnnexes\LootsAvalon\echangeur_final.txt' -Encoding Unicode
$names = $lines | foreach { $_.Split('|')[0] } | select -Unique
$sql = @()
foreach ($n in $names) {
    if ($n -eq "Nom PNJ") { continue }
    $safeName = $n.Replace("\", "\\").Replace("'", "''")
    $sql += "UPDATE mob SET ClassType = 'DOL.GS.Scripts.AvalonExchangerNPC' WHERE Name = '$safeName' AND Region = 51;"
}
$sql | Set-Content 'C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB\update_npc_classes.sql'
