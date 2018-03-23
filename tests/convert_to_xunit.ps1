Get-ChildItem -Recurse -Filter "**.csproj" | % { 
    Write-Host $_.FullName
    (Get-Content $_.FullName).Replace(
@'
<TargetFrameworks>net461;netstandard2.0</TargetFrameworks>
'@,
@'
<TargetFrameworks>net461;netcoreapp2.0</TargetFrameworks>
'@).Replace(
@'
    <PackageReference Include="nunit" Version="3.6.1" />
'@,
@'
    <PackageReference Include="xunit" Version="2.3.1" />
    <DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />
'@) | Out-File -FilePath $_.FullName
}

Get-ChildItem -Recurse -Filter "**.cs" | % {
    Write-Host $_.FullName
    (Get-Content $_.FullName).Replace(
@'
    [Test]
'@,
@'
    [Fact]
'@).Replace(
@'
    [TestCase
'@,
@'
    [InlineData
'@).Replace(
@'
Assert.That
'@,
@'
Assert.True
'@).Replace(
@'
using NUnit.Framework;
'@,
@'
using Xunit;
'@).Replace(
@'
XUnit
'@,
@'
Xunit
'@) | Out-File -FilePath $_.FullName
}
