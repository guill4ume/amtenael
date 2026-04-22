# Configuration
$CORE_PATH = "C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB"
$GAMESERVER_PATH = Join-Path $CORE_PATH "GameServer"
$COMMANDS_PATH = Join-Path $GAMESERVER_PATH "commands"
$SCRIPTS_PATH = Join-Path $GAMESERVER_PATH "scripts"
$LANG_FR_PATH = Join-Path $GAMESERVER_PATH "language\FR"
$LANG_EN_PATH = Join-Path $GAMESERVER_PATH "language\EN"

# Dicts for translations
$translationsFR = @{}
$translationsEN = @{}
$fallbackLog = @()

# Helper to load translations from a directory
function Load-Translations($path, $dict) {
    if (-not (Test-Path $path)) { return }
    Get-ChildItem -Path $path -Filter "*.txt" -Recurse | ForEach-Object {
        $content = Get-Content $_.FullName -Encoding UTF8
        foreach ($line in $content) {
            $line = $line.Trim()
            if ($line -eq "" -or $line.StartsWith("#")) { continue }
            if ($line -match '^([^:#\s]+)\s*:\s*(.*)$') {
                $key = $matches[1].Trim()
                $val = $matches[2].Trim()
                if (-not $dict.ContainsKey($key)) {
                    $dict[$key] = $val
                }
            }
        }
    }
}

Write-Host "Loading translations..."
Load-Translations $LANG_FR_PATH $translationsFR
Load-Translations $LANG_EN_PATH $translationsEN

function Cleanup-String($str) {
    if ($null -eq $str) { return "" }
    # Remove C# comments // ...
    $str = $str -replace '//.*$', ''
    # Remove concatenation " + " and extra quotes
    $str = $str -replace '"\s*\+\s*"', ''
    $str = $str.Trim().Trim('"')
    return $str
}

function Resolve-Text($text, $cmdName) {
    $text = Cleanup-String $text
    if ([string]::IsNullOrWhiteSpace($text)) { return "" }
    
    # If it's a known key in French
    if ($translationsFR.ContainsKey($text)) {
        return $translationsFR[$text]
    }
    
    # If it's a known key in English
    if ($translationsEN.ContainsKey($text)) {
        $script:fallbackLog += "Commande [$cmdName] : Clé [$text] traduite via l'Anglais"
        return $translationsEN[$text]
    }
    
    # If it's not a key, return it as is (literal string)
    return $text
}

# Privilege Level mapping
$PRIV_LEVELS = @{
    "1" = "Joueur"
    "2" = "GM"
    "3" = "Admin"
    "ePrivLevel.Player" = "Joueur"
    "ePrivLevel.GM" = "GM"
    "ePrivLevel.Admin" = "Admin"
}

$allCommands = @{}

function Parse-CSFile($file) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $matches = [regex]::Matches($content, '(?s)\[Cmd(?:Attribute)?\s*\((.*?)\)\]')
    foreach ($m in $matches) {
        $argsStr = $m.Groups[1].Value.Trim()
        
        $parts = @()
        $current = ""
        $inQuotes = $false
        $bracketLevel = 0
        $chars = $argsStr.ToCharArray()
        for ($i = 0; $i -lt $chars.Length; $i++) {
            $char = $chars[$i]
            if ($char -eq '"') { $inQuotes = -not $inQuotes }
            elseif ($char -eq '{' -or $char -eq '[') { if (-not $inQuotes) { $bracketLevel++ } }
            elseif ($char -eq '}' -or $char -eq ']') { if (-not $inQuotes) { $bracketLevel-- } }
            
            if ($char -eq ',' -and -not $inQuotes -and $bracketLevel -eq 0) {
                $parts += $current.Trim()
                $current = ""
            } else {
                $current += $char
            }
        }
        $parts += $current.Trim()
        
        if ($parts.Count -lt 1) { continue }
        
        $cmdNameRaw = Cleanup-String $parts[0]
        $cmdNameRaw = $cmdNameRaw.Replace("&", "/")
        
        $idx = 1
        $aliases = @()
        
        if ($idx -lt $parts.Count -and ($parts[$idx] -match 'new\s*(string)?\s*\[\]' -or $parts[$idx].StartsWith("{"))) {
            if ($parts[$idx] -match '{(.*?)}') {
                $aliasList = $matches[1].Split(",")
                foreach ($a in $aliasList) {
                    $aliases += (Cleanup-String $a).Replace("&", "/")
                }
            }
            $idx++
        }
        
        if ($idx -lt $parts.Count -and $parts[$idx].StartsWith('"') -and -not ($parts[$idx] -match "ePrivLevel|\d+")) {
            $idx++
        }
        
        $level = "Unknown"
        if ($idx -lt $parts.Count) {
            $lvlRaw = $parts[$idx].Trim()
            if ($PRIV_LEVELS.ContainsKey($lvlRaw)) {
                $level = $PRIV_LEVELS[$lvlRaw]
            } else {
                $level = $lvlRaw
            }
            $idx++
        }
        
        $desc = ""
        if ($idx -lt $parts.Count) {
            $desc = Resolve-Text $parts[$idx] $cmdNameRaw
            $idx++
        }
        
        $usages = @()
        while ($idx -lt $parts.Count) {
            $val = Resolve-Text $parts[$idx] $cmdNameRaw
            if ($val -ne "") { $usages += $val }
            $idx++
        }
        
        $allCommands[$cmdNameRaw] = @{
            Name = $cmdNameRaw
            Aliases = $aliases
            Level = $level
            Description = $desc
            Usages = $usages
        }
    }
}

Write-Host "Parsing Core commands..."
if (Test-Path $COMMANDS_PATH) {
    Get-ChildItem -Path $COMMANDS_PATH -Filter "*.cs" -Recurse | ForEach-Object { Parse-CSFile $_ }
}

Write-Host "Parsing Script commands (priority)..."
if (Test-Path $SCRIPTS_PATH) {
    Get-ChildItem -Path $SCRIPTS_PATH -Filter "*.cs" -Recurse | ForEach-Object { Parse-CSFile $_ }
}

$groups = @{
    "Joueur" = @()
    "GM" = @()
    "Admin" = @()
}

foreach ($cmd in $allCommands.Values) {
    $lvl = $cmd.Level
    if ($groups.ContainsKey($lvl)) {
        $groups[$lvl] += $cmd
    }
}

$levelFiles = @{
    "Joueur" = "Commandes_Joueur.txt"
    "GM" = "Commandes_GM.txt"
    "Admin" = "Commandes_Admin.txt"
}

Write-Host "Generating files..."
$utf8NoBOM = New-Object System.Text.UTF8Encoding $false
foreach ($lvl in $levelFiles.Keys) {
    $filename = $levelFiles[$lvl]
    $cmds = $groups[$lvl] | Sort-Object Name
    
    $output = "====== Commandes $lvl ======`n`n"
    $output += "^ Commande ^ Alias ^ Description ^ Utilisation ^`n"
    
    foreach ($cmd in $cmds) {
        $aliasStr = $cmd.Aliases -join ", "
        $usageStr = $cmd.Usages -join " \\ "
        $output += "| **$($cmd.Name)** | $aliasStr | $($cmd.Description) | $usageStr |`n"
    }
    
    [System.IO.File]::WriteAllText((Join-Path $PSScriptRoot $filename), $output, $utf8NoBOM)
}

$logOutput = $fallbackLog | Sort-Object | Out-String
[System.IO.File]::WriteAllText((Join-Path $PSScriptRoot "TODO_Traductions.txt"), $logOutput, $utf8NoBOM)

Write-Host "Documents générés dans $PSScriptRoot"
