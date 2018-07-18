#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi

# check for build environment, tools and libraries

if [ "$(command -v cmake)" == "" ] ; 
then 
	echo "error: cmake not found." 1>&2
	exit -1
fi

if [ "$(command -v dotnet)" == "" ] ; 
then 
	echo "error: dotnet not found." 1>&2
	echo "see: https://www.microsoft.com/net/download/linux"
	exit -1
fi

# build Trinity.C
build_trinity_c()
{
	echo "Building Trinity.C"
	mkdir -p "$REPO_ROOT/build" && pushd "$_" || exit -1
	cmake "$REPO_ROOT/src" || exit -1
	make install || exit -1
	popd
}

# build Trinity.Core
build_trinity_core()
{
	echo "Building Trinity.Core"
	pushd "$REPO_ROOT/src/Trinity.Core"
	dotnet restore Trinity.Core.sln || exit -1
	dotnet build -c Release /p:TargetFrameworks=netstandard2.0 Trinity.Core.sln || exit -1
	dotnet pack -c Release /p:TargetFrameworks=netstandard2.0 Trinity.Core.sln || exit -1
	popd
}

# build LIKQ
build_likq()
{
	echo "Building Trinity.Core"
	pushd "$REPO_ROOT/src/Modules/LIKQ/FanoutSearch"
	dotnet restore FanoutSearch.csproj || exit -1
	dotnet build -c Release /p:TargetFrameworks=netstandard2.0 FanoutSearch.csproj || exit -1
	dotnet pack -c Release /p:TargetFrameworks=netstandard2.0 FanoutSearch.csproj || exit -1
	popd
}

# build Client
build_client()
{
	echo "Building Trinity.Client"
	pushd "$REPO_ROOT/src/Modules/GraphEngine.Client"
	dotnet restore GraphEngine.Client.sln || exit -1
	dotnet build -c Release /p:TargetFrameworks=netstandard2.0 GraphEngine.Client.sln || exit -1
	dotnet pack -c Release /p:TargetFrameworks=netstandard2.0 GraphEngine.Client.sln || exit -1
	popd
}

# build composite_ext
build_composite_ext()
{
	echo "Building GraphEngine.Storage.Composite"
	pushd "$REPO_ROOT/src/Modules/GraphEngine.Storage.Composite"
	dotnet restore GraphEngine.Storage.Composite.sln || exit -1
	dotnet build -c Release /p:TargetFrameworks=netstandard2.0 GraphEngine.Storage.Composite.sln || exit -1
	dotnet pack -c Release /p:TargetFrameworks=netstandard2.0 GraphEngine.Storage.Composite.sln || exit -1
	popd
}

# register local nuget repo, remove GraphEngine.Core packages in the cache.
setup_nuget_repo()
{
	nuget_repo_name="Graph Engine OSS Local" 
    nuget_repo_location=$(printf "%q" "$REPO_ROOT/bin")
	echo "registering NuGet local repository '$nuget_repo_name'."
	if [ "$(grep "$nuget_repo_name" ~/.nuget/NuGet/NuGet.Config)" != "" ];
	then
        sed -i "/$nuget_repo_name/d" ~/.nuget/NuGet/NuGet.Config
	fi
	sed -i "s#</packageSources>#    <add key=\"$nuget_repo_name\" value=\"$nuget_repo_location\" \/>\n  <\/packageSources>#g" ~/.nuget/NuGet/NuGet.Config
	echo "remove local package cache."
	rm -rf ~/.nuget/packages/graphengine.*
}

build_trinity_c
build_trinity_core
setup_nuget_repo
build_likq
build_client
build_composite_ext
