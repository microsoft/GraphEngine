Function Init-Configuration {
    $Global:REPO_ROOT     = [System.IO.Path]::GetFullPath("$PSScriptRoot\..")

    $Global:TRINITY_CMAKELISTS            = "$REPO_ROOT\CMakeLists.txt"
    $Global:TRINITY_OUTPUT_DIR            = "$REPO_ROOT\bin"
    $Global:TRINITY_TEST_DIR              = "$REPO_ROOT\tests"
    $Global:NUGET_EXE                     = "$REPO_ROOT\tools\NuGet.exe"

    if (![System.IO.File]::Exists($NUGET_EXE)){
        Write-Output "Downloading NuGet package manager."
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
              Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $NUGET_EXE
    }

    New-Item -Path "$TRINITY_OUTPUT_DIR" -ItemType Directory -ErrorAction SilentlyContinue
}

Function Write-Configuration {
  Write-Output "GraphEngine repo root:         $REPO_ROOT"
  Write-Output "TRINITY_CMAKELISTS             $TRINITY_CMAKELISTS"
  Write-Output "TRINITY_OUTPUT_DIR:            $TRINITY_OUTPUT_DIR"
}


Function Remove-And-Print ($item) {
  if ($item -eq $null) { return }
  Write-Output "[x] Removing: $item"
  Remove-Item -Recurse -Force -ErrorAction Ignore -Path $item
}

Function Remove-Build {
  Remove-And-Print "$REPO_ROOT\bin"
  Remove-And-Print "$REPO_ROOT\build"
}

# Register local nuget source
# calling `nuget sources list` will create the config file if it does not exist
Function Register-LocalRepo {
  Invoke-Expression "& '$NUGET_EXE' sources list"
  Invoke-Expression "& '$NUGET_EXE' sources Remove -Name 'IKW Graph Engine OSS Local'"
  Invoke-Expression "& '$NUGET_EXE' sources Add -Name 'IKW Graph Engine OSS Local' -Source '$TRINITY_OUTPUT_DIR'"
}

Function Restore-GitSubmodules {
    Invoke-Expression "git submodule init" -ErrorAction Stop
    Invoke-Expression "git submodule update --recursive" -ErrorAction Stop
}

Init-Configuration
Export-ModuleMember -Variable *
Export-ModuleMember -Function *
