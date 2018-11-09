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

# build
build_repo()
{
	mkdir -p "$REPO_ROOT/build" && pushd "$_" || exit -1
	cmake "$REPO_ROOT" -DCMAKE_BUILD_TYPE=Release || exit -1
	make -j || exit -1
    ctest -C Release
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

build_repo
