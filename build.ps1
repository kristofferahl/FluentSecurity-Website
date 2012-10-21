properties {
	$product		= 'FluentSecurity.Website'
	$version		= '2.0.0'

	$rootDir		= '.'
	$sourceDir		= '.'
	$artifactsDir	= '.\Build\Artifacts'
	$artifactsName	= "$product-$version-release" -replace "\.","_"
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
	pack_solution "$sourceDir\FluentSecurity-Website.sln" $artifactsDir $artifactsName
	delete_files "$artifactsDir\$artifactsName\bin" @("*.xml", "*.pdb")
	copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\lib\net40" "$artifactsDir\$artifactsName\bin" "*.dll"
	copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\NativeBinaries\amd64" "$artifactsDir\$artifactsName\bin" "*.dll"
	delete_directory "$artifactsDir\$artifactsName\bin\Microsoft.VC90.CRT"

	#copy_files "$sourceDir\FluentSecurity-Website" $artifactsDir @("Global.asax", "*.html", "Web.config", "favicon.ico", "favicon.png")
	#copy_files "$sourceDir\FluentSecurity-Website\bin" "$artifactsDir\bin" "*.dll"
	#copy_files "$sourceDir\FluentSecurity-Website\Content" "$artifactsDir\Content"
	#copy_files "$sourceDir\FluentSecurity-Website\Scripts" "$artifactsDir\Scripts"
	#copy_files "$sourceDir\FluentSecurity-Website\Views" "$artifactsDir\Views"
	#copy_files "$sourceDir\Packages\Microsoft.SqlServer.Compact.4.0.8854.2\NativeBinaries\amd64" "$artifactsDir\bin" "*.dll"

	$packMessage
}

task Deploy -depends Pack {
	if ($deploymentDir -ne $null -and $deploymentDir -ne "") {
		Write-Host "Deploying to: $deploymentDir."

		execute_retry {
			delete_files $deploymentDir @("*") @("App_Data")
		} "Delete deployed files"
		
		copy_files "$artifactsDir\$artifactsName" $deploymentDir
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

function global:delete_files($source, $include=@("*"), $exclude=@()) {
	if (test-path $source -pathtype container) {
		Write-Host "Removing files in '$source'. Include '$include'. Exclude '$exclude'"
		Remove-Item -Recurse -Force "$source\*" -Include $include -Exclude $exclude -Verbose:$useVerbose
	}
}

function global:copy_files($source, $destination, $include=@("*.*"), $exclude=@()) {
	if (test-path $source) {
		$copiedFiles = 0
		Write-Host "Copying '$source' to '$destination'. Include '$include'. Exclude '$exclude'"

		New-Item -ItemType Directory -Path $destination -Force | Out-Null

		Get-ChildItem $source -Recurse -Include $include -Exclude $exclude | % {
			$fullSourcePath = (Resolve-Path $source)
			$fullDestinationPath = (Resolve-Path $destination)
			$itemPath = $_.FullName -replace [regex]::Escape($fullSourcePath),[regex]::Escape($fullDestinationPath)

			if ($useVerbose -eq $true) { Write-Host "Copying '$_' to '$itemPath'." }

			if (!($_.PSIsContainer)) {
				New-Item -ItemType File -Path $itemPath -Force | Out-Null
			}
			Copy-Item -Force -Path $_ -Destination $itemPath | Out-Null

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

function global:pack_solution($solutionName, $destination, $packageName) {
	create_directory $destination

	$packageRoot	= (Resolve-Path $destination)
	$packageDir		= "$packageRoot\$packageName"

	create_directory $packageDir

	$msBuildVerbosity = "minimal"
	if ($useVerbose -eq $false) {
		$msBuildVerbosity = "quiet"
	}

	msbuild $solutionName `
		/target:Publish `
		/property:Configuration=Release `
		/p:_PackageTempDir=$packageDir `
		/verbosity:$msBuildVerbosity
}

function global:execute_retry($Command, $CommandName, $retries = 3) {
    $currentRetry = 0;
    $success = $false;

    do
    {
        try
        {
            & $Command;
            $success = $true;
            Write-Host "Successfully executed [$CommandName] command. Number of retries: $currentRetry.";
        }
        catch [System.Exception]
        {
            Write-Host "Exception occurred while trying to execute [$CommandName] command:" + $_.Exception.ToString() -fore Yellow;
            if ($currentRetry -gt $retries)
            {
                throw "Can not execute [$CommandName] command. The error: " + $_.Exception.ToString();
            }
            else
            {
               Write-Host "Sleeping before $currentRetry retry of [$CommandName] command";
               Start-Sleep -s 1;
            }
            $currentRetry = $currentRetry + 1;
        }
    }
    while (!$success);
}