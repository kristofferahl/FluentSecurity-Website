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

task default -depends Info, Deploy

task Info {
	Write-Host "Running build" -fore Yellow;
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
	$packMessage
}

task Deploy -depends Pack {
	if ($deploymentDir -ne $null -and $deploymentDir -ne "") {
		Write-Host "Deploying to: $deploymentDir."

		with_retry {
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

function global:with_retry($Command, $CommandName, $retries = 3) {
    $currentRetry = 0;
    $success = $false;

    do {
        try {
            & $Command;
            $success = $true;
            Write-Host "Successfully executed [$CommandName] command. Number of retries: $currentRetry.";
        }
        catch [System.Exception] {
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