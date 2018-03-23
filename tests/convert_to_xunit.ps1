Get-ChildItem -Recurse -Filter "**.csproj" | % { 
    Write-Host $_.FullName
    (Get-Content $_.FullName).Replace(
@'
1.0.9083
'@,
@'
2.0.9328
'@) | Out-File -FilePath $_.FullName
}

#Get-ChildItem -Recurse -Filter "**.cs" | % {
    #Write-Host $_.FullName
    #(Get-Content $_.FullName).Replace(
#@'

#'@,
#@'

#'@) | Out-File -FilePath $_.FullName
#}
