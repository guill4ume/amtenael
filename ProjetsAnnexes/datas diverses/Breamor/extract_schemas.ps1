$file = 'C:\OpenDAOC_server\ProjetsAnnexes\Breamor\BDDamte030326.sql'
$reader = [System.IO.File]::OpenText($file)
while($null -ne ($line = $reader.ReadLine())){
    if($line -like '*CREATE TABLE `worldobject`*' -or $line -like '*CREATE TABLE `door`*'){
        Write-Host "--- Schema found ---"
        Write-Host $line
        for($i=0; $i -lt 40; $i++){
            $l = $reader.ReadLine()
            if ($null -ne $l) { Write-Host $l }
            if ($l -like '*CHARSET=*') { break }
        }
        Write-Host "--------------------"
    }
}
$reader.Close()
