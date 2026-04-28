$dumpPath = "amtedump04.sql"
$itemListPath = "itemlistids_region51.txt"
$outputPath = "migration_merchants_51.sql"

$ids = Get-Content $itemListPath | Where-Object { $_ -and $_ -ne "ItemsListTemplateID" }
$ids = $ids.Trim()

Write-Host "Extracting Merchant Items..."
$merchantItems = New-Object System.Collections.Generic.List[string]
$itemTemplateIds = New-Object System.Collections.Generic.HashSet[string]

$reader = New-Object System.IO.StreamReader($dumpPath)
while ($null -ne ($line = $reader.ReadLine())) {
    if ($line.Contains("INSERT INTO `merchantitem`")) {
        foreach ($id in $ids) {
            if ($line.Contains("', '$id', '")) {
                $merchantItems.Add($line.Trim())
                # Extract ItemTemplateID (it is the 3rd field)
                # Format: ('ID', 'ItemListID', 'ItemTemplateID', ...)
                $parts = $line.Split(",")
                if ($parts.Count -gt 2) {
                    $itid = $parts[2].Trim().Trim("'")
                    [void]$itemTemplateIds.Add($itid)
                }
                break
            }
        }
    }
}
$reader.Close()

Write-Host "Found $($merchantItems.Count) merchant item chunks."
Write-Host "Referencing $($itemTemplateIds.Count) item templates."

Write-Host "Second pass for item templates..."
$extractedTemplates = New-Object System.Collections.Generic.List[string]
$reader = New-Object System.IO.StreamReader($dumpPath)
while ($null -ne ($line = $reader.ReadLine())) {
    if ($line.Contains("INSERT INTO `itemtemplate`")) {
        foreach ($itid in $itemTemplateIds) {
            if ($line.Contains("VALUES ('$itid'")) {
                $extractedTemplates.Add($line.Trim())
                break
            }
        }
    }
}
$reader.Close()

Write-Host "Extracted $($extractedTemplates.Count) item templates."

$sw = New-Object System.IO.StreamWriter($outputPath)
$sw.WriteLine("SET FOREIGN_KEY_CHECKS=0;")
foreach($t in $extractedTemplates) { $sw.WriteLine($t) }
foreach($mi in $merchantItems) { $sw.WriteLine($mi) }
$sw.WriteLine("SET FOREIGN_KEY_CHECKS=1;")
$sw.Close()
Write-Host "Done!"
