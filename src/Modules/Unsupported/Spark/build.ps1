Import-Module -Force -WarningAction Ignore -Verbose "$PSScriptRoot\..\..\..\tools\setenv.psm1"
$cmd = @'
& $NUGET_EXE restore "$SPARK_MODULE_ROOT\Spark.sln"
'@
Invoke-Expression $cmd -ErrorAction Stop
Invoke-MSBuild -proj "$SPARK_MODULE_ROOT\Spark.sln" -ErrorAction Stop
