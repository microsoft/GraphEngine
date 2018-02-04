$REPO_ROOT     = [System.IO.Path]::GetFullPath("$PSScriptRoot\..")
$VSWHERE_EXE   = "$REPO_ROOT\tools\vswhere.exe"
$VS_VERSION    = "[15.0,16.0)"
$REQUIRES      = "Microsoft.Component.MSBuild Microsoft.VisualStudio.Component.VC.Tools.x86.x64"
$VS_INSTALLDIR = $null
Invoke-Expression "$VSWHERE_EXE -version '$VS_VERSION' -products * -requires $REQUIRES -property installationPath" | ForEach-Object { $VS_INSTALLDIR = $_  }


if ($VS_INSTALLDIR -eq $null) {
  throw "Visual Studio 2017 or required components were not found"
}
$MSBUILD_EXE   = "$VS_INSTALLDIR\MSBuild\15.0\Bin\MSBuild.exe"
$NUGET_EXE     = "$REPO_ROOT\tools\NuGet.exe"
$DOTNET_EXE    = "dotnet"

if (![System.IO.File]::Exists($NUGET_EXE)){
  Write-Output "Downloading NuGet package manager."
  Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $NUGET_EXE
}

$TRINITY_CORE_SLN              = "$REPO_ROOT\src\Trinity.Core\Trinity.Core.sln"
$TRINITY_C_SLN                 = "$REPO_ROOT\src\Trinity.C\Trinity.C.sln"
$TRINITY_TSL_SLN               = "$REPO_ROOT\src\Trinity.TSL\Trinity.TSL.sln"
$SPARK_MODULE_ROOT             = "$REPO_ROOT\src\Modules\Spark"
$LIKQ_SLN                      = "$REPO_ROOT\src\Modules\LIKQ\LIKQ.sln"
$TRINITY_CLIENT_SLN            = "$REPO_ROOT\src\Modules\GraphEngine.Client\GraphEngine.Client.sln"
$TRINITY_DYNAMICCLUSTER_SLN    = "$REPO_ROOT\src\Modules\GraphEngine.DynamicCluster\GraphEngine.DynamicCluster.sln"
$TRINITY_SERVICE_FABRIC_SLN    = "$REPO_ROOT\src\Modules\GraphEngine.ServiceFabric\GraphEngine.ServiceFabric.sln"
$TRINITY_STORAGE_COMPOSITE_SLN = "$REPO_ROOT\src\Modules\GraphEngine.Storage.Composite\GraphEngine.Storage.Composite.sln"
$TRINITY_FFI_ROOT              = "$REPO_ROOT\src\Modules\Trinity.FFI"
$TRINITY_OUTPUT_DIR            = "$REPO_ROOT\bin"

New-Item -Path "$TRINITY_OUTPUT_DIR" -ItemType Directory -ErrorAction SilentlyContinue

Function Write-Configuration {
  Write-Output "GraphEngine repo root:         $REPO_ROOT"
  Write-Output "vswhere.exe:                   $VSWHERE_EXE"
  Write-Output "VS_VERSION:                    $VS_VERSION"
  Write-Output "REQUIRES:                      $REQUIRES"
  Write-Output "VS_INSTALLDIR:                 $VS_INSTALLDIR"
  Write-Output "MSBUILD_EXE:                   $MSBUILD_EXE"
  Write-Output "NUGET_EXE:                     $NUGET_EXE"
  Write-Output "DOTNET_EXE:                    $DOTNET_EXE"

  Write-Output "TRINITY_CORE_SLN               $TRINITY_CORE_SLN"
  Write-Output "TRINITY_C_SLN:                 $TRINITY_C_SLN"
  Write-Output "TRINITY_TSL_SLN:               $TRINITY_TSL_SLN"
  Write-Output "SPARK_MODULE_ROOT:             $SPARK_MODULE_ROOT"
  Write-Output "LIKQ_SLN:                      $LIKQ_SLN"
  Write-Output "TRINITY_CLIENT_SLN:            $TRINITY_CLIENT_SLN"
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

Function Invoke-DotNet($proj, $action, $config = $null)
{
  if ($config -eq $null){
    $cmd = @'
 & $DOTNET_EXE $action "$proj"
'@
  }else{
    $cmd = @'
 & $DOTNET_EXE $action -c $config "$proj"
'@
  }
    Write-Output "========> Invoke: $proj[$config] -> $action"
    Invoke-Expression $cmd -ErrorAction Stop
}

Function Invoke-MSBuild($proj, $config = "Release", $platform = $null)
{
  if ($platform -eq $null) {
    $cmd = @'
 & $MSBUILD_EXE "/p:Configuration=$config" "$proj"
'@
  }else{
    $cmd = @'
 & $MSBUILD_EXE "/p:Configuration=$config;Platform=$platform" "$proj"
'@
  }
  Write-Output "========> MSBUILD: $proj[$config|$platform]"
  Invoke-Expression $cmd -ErrorAction Stop
}

Function New-Package($proj, $config = "Release")
{
  Invoke-DotNet -proj $proj -action restore
  Invoke-DotNet -proj $proj -action build -config $config
  Invoke-DotNet -proj $proj -action pack -config $config
}

# Register local nuget source
# calling `nuget sources list` will create the config file if it does not exist
Function Register-LocalRepo {
  Invoke-Expression "$NUGET_EXE sources list"
  Invoke-Expression "$NUGET_EXE sources Remove -Name 'Graph Engine OSS Local'"
  Invoke-Expression "$NUGET_EXE sources Add -Name 'Graph Engine OSS Local' -Source '$TRINITY_OUTPUT_DIR'"
}

Function Invoke-Sub($sub) {
  $sub = Get-Item -Path $sub
  Write-Output ("========>[+] Invoke-Sub: " + $sub.FullName)
  Push-Location
  try { & $sub.FullName }
  finally { Pop-Location }
}

Export-ModuleMember -Variable *
Export-ModuleMember -Function *