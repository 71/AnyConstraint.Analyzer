param([switch] $Publish, [string] $Configuration = "Release")

# Build all projects
dotnet.exe pack .\AnyConstraint.Analyzer\AnyConstraint.Analyzer.csproj --configuration $Configuration --output $(Get-Location)

if ($Publish) {
    # Optionally publish the package on NuGet
    nuget.exe push $(Get-ChildItem | Sort-Object { -$_.LastWriteTime })[0]
}