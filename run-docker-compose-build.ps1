$startTime = $(Get-Date)

docker-compose build

$elapsedTime = $(Get-Date) - $startTime

$elapsedTime

# "Beep" from: http://jeffwouters.nl/index.php/2012/03/get-your-geek-on-with-powershell-and-some-music/
[console]::beep(900,400) 
[console]::beep(1000,400) 
[console]::beep(800,400) 
[console]::beep(400,400) 
[console]::beep(600,1600)
