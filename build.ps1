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
	copy_files "$sourceDir\FluentSecurity-Website" @("Global.asax", "*.html", "Web.config") $artifactsDir
	copy_files "$sourceDir\FluentSecurity-Website\bin" "*.dll" "$artifactsDir\bin"
	copy_files "$sourceDir\FluentSecurity-Website\Content" "*.*" "$artifactsDir\Content"
	copy_files "$sourceDir\FluentSecurity-Website\Scripts" "*.*" "$artifactsDir\Scripts"
	copy_files "$sourceDir\FluentSecurity-Website\Views" "*.*" "$artifactsDir\Views"
	$packMessage
}

task Deploy -depends Pack {
	if ($deploymentDir -ne $null -and $deploymentDir -ne "") {
		Write-Host "Deploying to: $deploymentDir."
		
		create_directory "$artifactsDir\App_Data"
		copy_files "$deploymentDir\App_Data" "*.*" "$artifactsDir\App_Data"
		delete_directory "$deploymentDir"
		Get-ChildItem $artifactsDir -Recurse | Copy-Item -Destination { Join-Path $deploymentDir $_.FullName.Substring($artifactsDir.length) }
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

function global:copy_files($source, $include=@(), $destination, $exclude=@()) {
    if ($useVerbose -eq $true) {
		Write-Host "Copying files from '$source' to '$destination'."
	}
	
	create_directory $destination
	Get-ChildItem $source -Recurse -Include $include -Exclude $exclude | % {
		$sourcePath = $_.FullName
		$destinationPath = Join-Path $destination $_.FullName.Substring($pwd.path.length).Substring($source.length)
		create_directory (Split-Path $destinationPath)
		Copy-Item -Force $sourcePath $destinationPath
	}
}

function global:copy_files_flatten($source, $filter, $destination) {
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