$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

function Create-OctopusToolsRelease
{
    <#
    .Synopsis
     Builds a complete release (compile, test, ILMerge, NuGet package, version stamp)
    #>

    $output = . "Tools\GitVersion\gitversion.exe"

    $formattedOutput = $output -join "`n"
    Write-Host "Output from gitversion.exe"
    Write-Host $formattedOutput

    $versionInfo = $formattedOutput | ConvertFrom-Json
    $script:package_version = $versionInfo.NuGetVersion
    write-host "Package version:    $script:package_version"

	& $msbuild "Tools\Build.proj" /p:build_number=$package_version /t:BuildAndPublish /v:d
}

Export-ModuleMember -function * -alias *
