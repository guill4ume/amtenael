$inputDir = "C:\Users\Guillaume\Videos"
$outputDir = "C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoEditVideo"
$processedDir = Join-Path $outputDir "processed"
$toolsDir = Join-Path $outputDir "tools"
$tempDir = Join-Path $outputDir "temp"
$ffmpeg = "$toolsDir\ffmpeg.exe"
$ffprobe = "$toolsDir\ffprobe.exe"
$filterFile = "$tempDir\filter.txt"
$logFile = "$tempDir\silence.log"
$padding = 0.5 # New padding requested by user

# Ensure processed directory exists
if (-not (Test-Path $processedDir)) { New-Item -ItemType Directory -Path $processedDir -Force }

# Get all mp4 files in input directory (excluding already treated ones)
$videos = Get-ChildItem -Path $inputDir -Filter "*.mp4" | Where-Object { $_.Name -notmatch "_VocalOnly.mp4" }

foreach ($video in $videos) {
    $inputFile = $video.FullName
    $outputFile = Join-Path $processedDir ($video.BaseName + "_VocalOnly.mp4")
    
    Write-Host "--- Traitement de : $($video.Name) ---"
    
    # 1. Detect silence
    Write-Host "Détection des silences..."
    & $ffmpeg -i $inputFile -af silencedetect=n=-30dB:d=0.5 -f null - 2> $logFile
    
    # 2. Parse silence
    $durationStr = & $ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 $inputFile
    $duration = [double]::Parse($durationStr, [System.Globalization.CultureInfo]::InvariantCulture)

    $content = Get-Content $logFile
    $silenceStarts = New-Object System.Collections.Generic.List[double]
    $silenceEnds = New-Object System.Collections.Generic.List[double]

    foreach ($line in $content) {
        if ($line -match "silence_start: ([\d.]+)") {
            $silenceStarts.Add([double]::Parse($matches[1], [System.Globalization.CultureInfo]::InvariantCulture))
        }
        elseif ($line -match "silence_end: ([\d.]+)") {
            $silenceEnds.Add([double]::Parse($matches[1], [System.Globalization.CultureInfo]::InvariantCulture))
        }
    }

    $segments = New-Object System.Collections.Generic.List[PSObject]
    $lastPos = 0.0

    for ($i = 0; $i -lt $silenceStarts.Count; $i++) {
        $v_start = $lastPos
        $v_end = $silenceStarts[$i]
        
        if ($v_start -gt 0) { $v_start = [math]::Max(0, $v_start - $padding) }
        $v_end = [math]::Min($duration, $v_end + $padding)

        if ($v_end - $v_start -gt 0.1) {
            $segments.Add([PSCustomObject]@{s=$v_start; e=$v_end})
        }
        
        if ($i -lt $silenceEnds.Count) {
            $lastPos = $silenceEnds[$i]
        } else {
            $lastPos = $duration
        }
    }

    if ($lastPos -lt $duration) {
        $v_start = [math]::Max(0, $lastPos - $padding)
        $v_end = $duration
        if ($v_end - $v_start -gt 0.1) {
            $segments.Add([PSCustomObject]@{s=$v_start; e=$v_end})
        }
    }

    Write-Host "Nombre de segments : $($segments.Count)"

    # 3. Create Filter
    $sb = New-Object System.Text.StringBuilder
    for ($i = 0; $i -lt $segments.Count; $i++) {
        $sTime = $segments[$i].s.ToString("F3", [System.Globalization.CultureInfo]::InvariantCulture)
        $eTime = $segments[$i].e.ToString("F3", [System.Globalization.CultureInfo]::InvariantCulture)
        [void]$sb.AppendLine("[0:v]trim=${sTime}:${eTime},setpts=PTS-STARTPTS[v$i];")
        [void]$sb.AppendLine("[0:a]atrim=${sTime}:${eTime},asetpts=PTS-STARTPTS[a$i];")
    }

    for ($i = 0; $i -lt $segments.Count; $i++) {
        [void]$sb.Append("[v$i][a$i]")
    }
    [void]$sb.Append("concat=n=$($segments.Count):v=1:a=1[v][a]")

    $utf8NoBOM = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($filterFile, $sb.ToString(), $utf8NoBOM)

    # 4. ffmpeg
    Write-Host "Exportation..."
    & $ffmpeg -i $inputFile -filter_complex_script $filterFile -map "[v]" -map "[a]" -c:v libx264 -preset ultrafast -crf 23 -c:a aac -b:a 128k $outputFile -y
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Succès ! Suppression de l'original..."
        Remove-Item -Path $inputFile -Force
    } else {
        Write-Host "Erreur lors du traitement de $($video.Name). Fichier original conservé."
    }
}

Write-Host "Batch terminé."
