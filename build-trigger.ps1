Param(
  [string]$Target,
  [string]$ArtifactsDir,
  [string]$DeploymentDir
)

$psakeParameters = @{"deploymentDir" = $DeploymentDir;};

if ($ArtifactsDir -ne $null -and $ArtifactsDir -ne "") {
	$psakeParameters["artifactsDir"] = $ArtifactsDir
}

nuget install psake -Version 4.2.0.1 -OutputDirectory "Packages"

Import-Module '.\Packages\psake.4.2.0.1\tools\psake.psm1';
invoke-psake .\build.ps1 $Target -properties $psakeParameters;

if ($lastexitcode -ne 0 -and $lastexitcode -ne $null) {
	Write-Host "ERROR: $lastexitcode." -fore RED; exit $lastexitcode
}