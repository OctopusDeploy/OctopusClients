$host.ui.rawui.WindowTitle = "Octopus Tools Prompt"

function Prompt
{
    Write-Host ("OctopusTools>") -nonewline -foregroundcolor White
    return " "
}

Import-Module .\Tools\Build.psm1

clear

Write-Host "Welcome! This is a PowerShell console with commands for building OctopusTools." -foregroundcolor Green
Write-Host
Write-Host "Try one of these to get started: "
Write-Host "  Build-Compile            # Compiles the code"
Write-Host "  Build-Test               # Compiles and runs unit tests"
Write-Host "  Build-Package            # Does a complete build, producing MSIs"
Write-Host "  Build-Release            # Does a complete build, producing MSIs, versioned"
Write-Host
