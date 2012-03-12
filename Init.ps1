Import-Module .\Tools\Build.psm1

clear
Write-Host "Loaded OctopusTools commands:" -foregroundcolor Green
Get-Command | where-object { $_.Name -like "*OctopusTools*" }
