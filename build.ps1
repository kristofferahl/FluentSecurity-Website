packages @{
	"Koshu.NuGet"=""
}

properties {
	$product		= 'FluentSecurity.Website'
	$version		= '2.0.0'

	$rootDir		= '.'
	$sourceDir		= '.'
	$artifactsDir	= '.\Build\Artifacts'
	$artifactsName	= "$product-$version-release" -replace "\.","_"
	$deployTo		= ''

	$buildNumber	= $null
	$octopusApiKey	= $null
	$octopusServer	= $null

	$setupMessage	= 'Executed Setup!'
	$cleanMessage	= 'Executed Clean!'
	$compileMessage	= 'Executed Compile!'
	$testMessage	= 'Executed Test!'
	$packMessage	= 'Executed Pack!'
	$deployMessage	= 'Executed Deploy!'

	$useVerbose = $false
}

task default -depends Info, Deploy

task Info {
	Write-Host "Running local build" -fore Yellow
	Write-Host "Product:        $product" -fore Yellow
	Write-Host "Version:        $version" -fore Yellow
	Write-Host "Build version:  $buildVersion" -fore Yellow
}

task Setup {
	nuget_exe install ".nuget\packages.config" -OutputDirectory "Source\Packages"
	$setupMessage
}

task Clean {
	delete_directory $artifactsDir
	create_directory $artifactsDir
	$cleanMessage
}

task Compile -depends Setup,Clean {
	build_solution "$sourceDir\FluentSecurity-Website.sln"
	$compileMessage
}

task Test -depends Compile {
	$testMessage
}

task Pack -depends Test {
	pack_solution "$sourceDir\FluentSecurity-Website.sln" $artifactsDir $artifactsName
	delete_files "$artifactsDir\$artifactsName\bin" @("*.xml", "*.pdb")
	copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\lib\net40" "$artifactsDir\$artifactsName\bin" "*.dll"
	copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\NativeBinaries\amd64" "$artifactsDir\$artifactsName\bin" "*.dll"
	delete_directory "$artifactsDir\$artifactsName\bin\Microsoft.VC90.CRT"

	generate-nuspec `
	    -destination "$artifactsDir\$artifactsName" `
	    -id $product `
	    -version $version `
	    -author 'Kristoffer Ahl' `
	    -description 'The website for FluentSecurity' `
	    -basePath "$artifactsDir\$artifactsName"

	nuget_exe pack "$artifactsDir\$artifactsName\$product.nuspec" -outputdirectory $artifactsDir -nopackageanalysis

	$packMessage
}

task Deploy -depends Pack {
	if ($deployTo -eq 'Production' -or $deployTo -eq 'Develop') {
		Write-Host "Pushing nuget packages to octopus"

		nuget_exe push (resolve-path "$artifactsDir\$($product).$($buildVersion).nupkg") -source "$octopusServer/nuget/packages" $octopusApiKey

		$octo = "$sourceDir\packages\OctopusTools.2.5.10.39\octo.exe"

		Write-Host "Creating Octopus Deploy release ($buildVersion)"

		exec {
			& $octo create-release `
				--project="FluentSecurity.Web" `
				--server=$octopusServer `
				--apiKey=$octopusApiKey `
				--version=$buildVersion `
				--releasenotes="$product ($buildLabel) - Release created from buildscript." `
				--deployTo=$deployTo
		}
	} else {
		Write-Host "Skipping deployment..."
	}
	$deployMessage
}

task ? -Description "Help" {
	Write-Documentation
}

taskSetup {
	$script:buildVersion = ?: {$buildNumber -ne $null} {"$version.$buildNumber"} {$version}
}