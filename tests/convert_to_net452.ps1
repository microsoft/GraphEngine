Get-ChildItem -Recurse -Filter "**.csproj" | % { 
    Write-Host $_.FullName
    (Get-Content $_.FullName).Replace(
@'
net451
'@,
@'
net452
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
