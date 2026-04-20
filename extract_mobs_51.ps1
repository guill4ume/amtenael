Add-Type -AssemblyName Microsoft.VisualBasic
$inputPath = "C:\OpenDAOC_server\ProjetsAnnexes\Breamor\BDDamte030326.sql"
$outputPath = "C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB\mobs_map51_amte.sql"
$startLine = 1221817
$endLine = 1235605

$header = "INSERT INTO opendaoc.mob (Mob_ID, ClassType, TranslationId, Name, Suffix, Guild, ExamineArticle, MessageArticle, X, Y, Z, Speed, Heading, Region, Model, Size, Strength, Constitution, Dexterity, Quickness, Intelligence, Piety, Empathy, Charisma, Level, Realm, EquipmentTemplateID, ItemsListTemplateID, NPCTemplateID, Race, Flags, AggroLevel, AggroRange, MeleeDamageType, RespawnInterval, FactionID, BodyType, HouseNumber, Brain, PathID, OwnerID, RoamingRange, IsCloakHoodUp, Gender, PackageID, VisibleWeaponSlots, LastTimeRowUpdated) VALUES "
$header | Out-File $outputPath -Encoding utf8

$mobs = @()
Get-Content $inputPath | Select-Object -Skip ($startLine - 1) | Select-Object -First ($endLine - $startLine + 1) | ForEach-Object {
    if ($_ -match "^\s*\((.*)\),?\s*$") {
        $data = $_ -replace "^\s*\((.*)\),?\s*$", '$1'
        $columns = [Microsoft.VisualBasic.FileIO.TextFieldParser]::new([System.IO.StringReader]::new($data))
        $columns.HasFieldsEnclosedInQuotes = $true
        $columns.SetDelimiters(",")
        $fields = $columns.ReadFields()
        
        $region = $fields[8].Trim().Trim("'")
        if ($region -eq "51") {
            $rowIndices = @(42, 0, 43, 1, 44, 2, 45, 46, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 36, 37, 38, 39, 40, 41, 47)
            
            $formattedValues = @()
            foreach ($idx in $rowIndices) {
                $val = $fields[$idx]
                if ($null -ne $val) { 
                    $val = $val.Trim().Trim("'") 
                    # Clean existing backslash escapes to prevent double escaping issues
                    $val = $val -replace "\\'", "'"
                }

                if ($null -eq $val -or $val -eq "NULL" -or $val -eq "") {
                    if ($idx -in @(42, 0, 43, 1, 44, 2, 45, 46, 21, 22, 33, 34, 36, 40)) {
                        if ($idx -in @(21, 22, 33, 34, 40)) { $formattedValues += "NULL" }
                        else { $formattedValues += "''" }
                    }
                    elseif ($idx -eq 47) { $formattedValues += "'2000-01-01 00:00:00'" }
                    else { $formattedValues += "0" }
                }
                else {
                    # Proper SQL escaping (double quotes)
                    $formattedValues += "'" + ($val -replace "'", "''") + "'"
                }
            }
            $mobs += "(" + ($formattedValues -join ", ") + ")"
        }
    }
}

if ($mobs.Count -gt 0) {
    $sqlContent = $mobs -join ",`n"
    $sqlContent + ";" | Out-File $outputPath -Append -Encoding utf8
}

Write-Host "Done! Extracted $($mobs.Count) mobs."
