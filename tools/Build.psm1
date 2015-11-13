$msbuild = "MSBuild.exe"

function Create-OctopusToolsRelease ([string]$BuildNumber=0.0.0)
{
    <#
    .Synopsis
     Builds a complete release (compile, test, ILMerge, NuGet package, version stamp)
    #>
	& $msbuild "Tools\Build.proj" /p:build_number=$BuildNumber /t:BuildAndPublish /verbosity:detailed
}

function Create-OctopusToolsPackage
{
    <#
    .Synopsis
        Compiles, tests, runs ILMerge
    #>
	& $msbuild "Tools\Build.proj" /t:BuildAndPackage /v:q
}

Export-ModuleMember -function * -alias *
