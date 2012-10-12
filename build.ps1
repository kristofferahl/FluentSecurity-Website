properties {
	$product		= 'FluentSecurity.Website'
	$version		= '2.0.0'

	$rootDir		= '.'
	$sourceDir		= '.'
	$artifactsDir	= '.\Build\Artifacts'
	$deploymentDir	= ''
	
	$setupMessage	= 'Executed Setup!'
	$cleanMessage	= 'Executed Clean!'
	$compileMessage	= 'Executed Compile!'
	$testMessage	= 'Executed Test!'
	$packMessage	= 'Executed Pack!'
	$deployMessage	= 'Executed Deploy!'
	
	$useVerbose = $false
}

task default -depends Local

task Local {
	Write-Host "Running local build" -fore Yellow;
	Invoke-Task Pack
}
task Release {
	Write-Host "Running release build" -fore Yellow;
	Invoke-Task Deploy
}

task Setup {
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
	copy_files "$sourceDir\FluentSecurity-Website" $artifactsDir @("Global.asax", "*.html", "Web.config", "favicon.ico", "favicon.png")
	copy_files "$sourceDir\FluentSecurity-Website\bin" "$artifactsDir\bin" "*.dll"
	copy_files "$sourceDir\FluentSecurity-Website\Content" "$artifactsDir\Content"
	copy_files "$sourceDir\FluentSecurity-Website\Scripts" "$artifactsDir\Scripts"
	copy_files "$sourceDir\FluentSecurity-Website\Views" "$artifactsDir\Views"
	copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\NativeBinaries\x86" "$artifactsDir\bin" "*.dll"
	$packMessage
}

task Deploy -depends Pack {
	if ($deploymentDir -ne $null -and $deploymentDir -ne "") {
		Write-Host "Deploying to: $deploymentDir."
		
		copy_files "$deploymentDir\App_Data" "$artifactsDir\App_Data"
		delete_directory $deploymentDir
		copy_files $artifactsDir $deploymentDir
	} else {
		Write-Host "No deployment directory set!"
	}
	$deployMessage
}

task ? -Description "Help" {
	Write-Documentation
}




# -------------------------------------------------------------------------------------------------------------
# Reusable functions 
# --------------------------------------------------------------------------------------------------------------

function global:create_directory($directoryName) {
	if (!(test-path $directoryName -pathtype container)) {
		New-Item $directoryName -Type directory -Force -Verbose:$useVerbose
	}
}

function global:delete_directory($directoryName) {
	if (test-path $directoryName -pathtype container) {
		Remove-Item -Recurse -Force $directoryName -Verbose:$useVerbose
	}
}

function global:copy_files($source, $destination, $include=@("*.*"), $exclude=@()) {
	if (test-path $source) {
		$copiedFiles = 0
		Write-Host "Copying '$source' to '$destination'. Include '$include'. Exclude '$exclude'"
		
		Get-ChildItem $source -Recurse -Include $include -Exclude $exclude | % {
			New-Item -ItemType Directory -Path $destination -Force | Out-Null
			
			$fullSourcePath = (Resolve-Path $source)
			$fullDestinationPath = (Resolve-Path $destination)
			$itemPath = $_.FullName -replace [regex]::Escape($fullSourcePath),[regex]::Escape($fullDestinationPath)
			
			New-Item -ItemType File -Path $itemPath -Force | Out-Null
			Copy-Item -Force -Path $_ -Destination $itemPath | Out-Null
		
			if ($useVerbose -eq $true) { Write-Host "Copying '$_'." }
			$copiedFiles++
		}
		
		Write-Host "Copied $copiedFiles $(if ($copiedFiles -eq 1) { "item" } else { "items" })."
	}
}

function global:copy_files_flatten($source, $destination, $filter) {
	create_directory $destination
	foreach($f in $filter.split(",")) {
		ls $source -filter $f.trim() -r | cp -dest $destination
	}
}

function global:build_solution($solutionName) {
	if ($useVerbose -eq $false) {
		msbuild $solutionName /target:Rebuild /property:Configuration=Release /verbosity:quiet
	} else {
		msbuild $solutionName /target:Rebuild /property:Configuration=Release /verbosity:minial
	}
}