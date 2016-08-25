param($installPath, $toolsPath, $package, $project)

$path = $env:PATH
if (!$path.Contains($toolsPath)) {
    $env:PATH += ";$toolsPath"
}