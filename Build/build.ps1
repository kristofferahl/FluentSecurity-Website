properties {
	$product		= 'FluentSecurity.Website'
	$version		= '2.0.0'

	$rootDir		= '.'
	$sourceDir		= '.'
	$artifactsDir	= '.\Artifacts'
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
	$cleanMessage
}

task Compile -depends Setup,Clean {
	$compileMessage
}

task Test -depends Compile {
	$testMessage
}

task Pack -depends Test {
	$packMessage
}

task Deploy -depends Pack {
	if ($deploymentDir -ne $null -and $deploymentDir -ne "") {
		Write-Host "Deploying to: $deploymentDir."
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
	if (!(test-path $directoryName -pathtype container)){
		New-Item $directoryName -Type directory -Verbose:$useVerbose
	}
}

function global:delete_directory($directoryName) {
	if (test-path $directoryName -pathtype container){
		Remove-Item -Recurse -Force $directoryName -Verbose:$useVerbose
	}
}

function global:copy_files($source, $destination, $exclude=@()) {
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude | Copy-Item -Destination { Join-Path $destination $_.FullName.Substring($source.length) }
}

function global:copy_files_flatten($source, $filter, $destination) {
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