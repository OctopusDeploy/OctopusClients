$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

function Build-Release ([string]$BuildNumber=1)
{
	& $msbuild "Tools\Build.proj" /p:build_number=$BuildNumber /t:BuildAndPublish
	Revert-Changes
}

function Build-Package
{
	& $msbuild "Tools\Build.proj" /t:BuildAndPackage /v:q
	Revert-Changes
}

function Build-Compile
{
	& $msbuild "Tools\Build.proj" /t:Compile /v:q
}

function Build-Test
{
	& $msbuild "Tools\Build.proj" /t:Test /v:q
}

function Revert-Changes 
{
}

Export-ModuleMember -function * -alias *
