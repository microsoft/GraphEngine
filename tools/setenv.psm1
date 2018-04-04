Function Init-Configuration {
    $Global:REPO_ROOT     = [System.IO.Path]::GetFullPath("$PSScriptRoot\..")
    $Global:VSWHERE_EXE   = "$REPO_ROOT\tools\vswhere.exe"
    $Global:VS_VERSION    = "[15.0,16.0)"
    $Global:REQUIRES      = "Microsoft.Component.MSBuild Microsoft.VisualStudio.Component.VC.Tools.x86.x64"
    $Global:VS_INSTALLDIR = $null
    Invoke-Expression "& '$VSWHERE_EXE' -version '$VS_VERSION' -products * -requires $REQUIRES -property installationPath" | ForEach-Object { $Global:VS_INSTALLDIR = $_  }

    if ($VS_INSTALLDIR -eq $null) {
        throw "Visual Studio 2017 or required components were not found"
    }
    $Global:MSBUILD_EXE   = "$VS_INSTALLDIR\MSBuild\15.0\Bin\MSBuild.exe"
    $Global:DEVENV_EXE    = "$VS_INSTALLDIR\Common7\IDE\devenv.exe"
    $Global:DEVENV_COM    = "$VS_INSTALLDIR\Common7\IDE\devenv.com"
    $Global:NUGET_EXE     = "$REPO_ROOT\tools\NuGet.exe"
    $Global:DOTNET_EXE    = "dotnet"

    if (![System.IO.File]::Exists($NUGET_EXE)){
        Write-Output "Downloading NuGet package manager."
            Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $NUGET_EXE
    }

    $Global:TRINITY_CORE_SLN              = "$REPO_ROOT\src\Trinity.Core\Trinity.Core.sln"
    $Global:TRINITY_C_SLN                 = "$REPO_ROOT\src\Trinity.C\Trinity.C.sln"
    $Global:TRINITY_TSL_SLN               = "$REPO_ROOT\src\Trinity.TSL\Trinity.TSL.sln"
    $Global:SPARK_MODULE_ROOT             = "$REPO_ROOT\src\Modules\Spark"
    $Global:LIKQ_SLN                      = "$REPO_ROOT\src\Modules\LIKQ\LIKQ.sln"
    $Global:TRINITY_CLIENT_ROOT           = "$REPO_ROOT\src\Modules\GraphEngine.Client"
    $Global:TRINITY_DYNAMICCLUSTER_SLN    = "$REPO_ROOT\src\Modules\GraphEngine.DynamicCluster\GraphEngine.DynamicCluster.sln"
    $Global:TRINITY_SERVICE_FABRIC_SLN    = "$REPO_ROOT\src\Modules\GraphEngine.ServiceFabric\GraphEngine.ServiceFabric.sln"
    $Global:TRINITY_STORAGE_COMPOSITE_SLN = "$REPO_ROOT\src\Modules\GraphEngine.Storage.Composite\GraphEngine.Storage.Composite.sln"
    $Global:TRINITY_FFI_ROOT              = "$REPO_ROOT\src\Modules\Trinity.FFI"
    $Global:TRINITY_JIT_ROOT              = "$REPO_ROOT\src\Modules\GraphEngine.Jit"
    $Global:TRINITY_OUTPUT_DIR            = "$REPO_ROOT\bin"
    $Global:TRINITY_TEST_DIR              = "$REPO_ROOT\tests"

    New-Item -Path "$TRINITY_OUTPUT_DIR" -ItemType Directory -ErrorAction SilentlyContinue
}

Function Write-Configuration {
  Write-Output "GraphEngine repo root:         $REPO_ROOT"
  Write-Output "vswhere.exe:                   $VSWHERE_EXE"
  Write-Output "VS_VERSION:                    $VS_VERSION"
  Write-Output "REQUIRES:                      $REQUIRES"
  Write-Output "VS_INSTALLDIR:                 $VS_INSTALLDIR"
  Write-Output "MSBUILD_EXE:                   $MSBUILD_EXE"
  Write-Output "NUGET_EXE:                     $NUGET_EXE"
  Write-Output "DOTNET_EXE:                    $DOTNET_EXE"
  Write-Output "DEVENV_COM:                    $DEVENV_COM"
  Write-Output "DEVENV_EXE:                    $DEVENV_EXE"

  Write-Output "TRINITY_CORE_SLN               $TRINITY_CORE_SLN"
  Write-Output "TRINITY_C_SLN:                 $TRINITY_C_SLN"
  Write-Output "TRINITY_TSL_SLN:               $TRINITY_TSL_SLN"
  Write-Output "SPARK_MODULE_ROOT:             $SPARK_MODULE_ROOT"
  Write-Output "LIKQ_SLN:                      $LIKQ_SLN"
  Write-Output "TRINITY_CLIENT_ROOT:           $TRINITY_CLIENT_ROOT"
  Write-Output "TRINITY_DYNAMICCLUSTER_SLN:    $TRINITY_DYNAMICCLUSTER_SLN"
  Write-Output "TRINITY_SERVICE_FABRIC_SLN:    $TRINITY_SERVICE_FABRIC_SLN"
  Write-Output "TRINITY_STORAGE_COMPOSITE_SLN: $TRINITY_STORAGE_COMPOSITE_SLN"
  Write-Output "TRINITY_FFI_ROOT:              $TRINITY_FFI_ROOT"
  Write-Output "TRINITY_OUTPUT_DIR:            $TRINITY_OUTPUT_DIR"
}


Function Remove-And-Print ($item) {
  if ($item -eq $null) { return }
  Write-Output "[x] Removing: $item"
  Remove-Item -Recurse -Force -ErrorAction Ignore -Path $item
}

Function Remove-GraphEngineCache ($prefix="graphengine") {
  Write-Output "Cleanning caches"
  $package_loc = (& $NUGET_EXE locals global-packages -list)
  $package_loc = $package_loc.Substring(17)
  Write-Output "Package location = $package_loc"
  Get-ChildItem $package_loc -Filter "$prefix*" | ForEach-Object { Remove-And-Print $_.FullName }
  Get-ChildItem -Path $REPO_ROOT -Recurse -Filter "packages" | Get-ChildItem -Filter "prefix*" | ForEach-Object { Remove-And-Print $_.FullName }
}

Function Remove-Build {
  Remove-And-Print "$REPO_ROOT\bin"
}

Function Invoke-DotNet($proj, $action, $config = $null) {
  Write-Output "========> Invoke: $proj[$config] -> $action"
  if ($config -eq $null){
    & $DOTNET_EXE $action "$proj"
  }else{
    & $DOTNET_EXE "$action" -c $config "$proj"
  }
}

Function Invoke-MSBuild($proj, $config = "Release", $platform = $null) {
  Write-Output "========> MSBUILD: $proj[$config|$platform]"
  if ($platform -eq $null) {
     & $MSBUILD_EXE "/p:Configuration=$config" "$proj"
  }else{
     & $MSBUILD_EXE "/p:Configuration=$config;Platform=$platform" "$proj"
  }
}

Function New-Package($proj, $config = "Release") {
  Invoke-DotNet -proj $proj -action restore
  Invoke-DotNet -proj $proj -action build -config $config
  Invoke-DotNet -proj $proj -action pack -config $config
}

# Register local nuget source
# calling `nuget sources list` will create the config file if it does not exist
Function Register-LocalRepo {
  Invoke-Expression "& '$NUGET_EXE' sources list"
  Invoke-Expression "& '$NUGET_EXE' sources Remove -Name 'Graph Engine OSS Local'"
  Invoke-Expression "& '$NUGET_EXE' sources Add -Name 'Graph Engine OSS Local' -Source '$TRINITY_OUTPUT_DIR'"
}

Function Invoke-Sub($sub) {
  $sub = Get-Item -Path $sub
  Write-Output ("========>[+] Invoke-Sub: " + $sub.FullName)
  Push-Location
  try { . $sub.FullName }
  finally { Pop-Location; Init-Configuration }
# Call Init-Configuration again in case the sub erased the variables, or they are somehow lost.
}

Function Invoke-Test($proj, $block) {
  Write-Output ("========>[+] Invoke-Test: " + $proj)
  Push-Location
  try { & $block }
  finally { Pop-Location }
}

Function Test-Xunit($proj, $config = "Release") {
  $proj = Get-Item -Path $proj
  $dir  = $proj.Directory.FullName
  $proj = $proj.Name

  Invoke-Test -proj $proj -block {
      Set-Location $dir
      Invoke-DotNet -proj $proj -action restore
      Invoke-DotNet -proj $proj -action build -config $config
      Invoke-Expression "& $DOTNET_EXE xunit"
  }
}

Function Test-ExeProject($proj, $config = "Release") {
    $proj = Get-Item -Path $proj
    $dir  = $proj.Directory
    $proj = $proj.Name

    Invoke-Test -proj $proj -block {
        Set-Location $dir
        Invoke-DotNet -proj $proj -action restore
        Invoke-DotNet -proj $proj -action build -config $config
        Invoke-Expression "& $DOTNET_EXE run --framework net461 '$proj'"
        Invoke-Expression "& $DOTNET_EXE run --framework netcoreapp2.0 '$proj'"
    }
}

Function Restore-GitSubmodules {
    Invoke-Expression "git submodule init" -ErrorAction Stop
    Invoke-Expression "git submodule update --recursive" -ErrorAction Stop
}

Init-Configuration
Export-ModuleMember -Variable *
Export-ModuleMember -Function *
