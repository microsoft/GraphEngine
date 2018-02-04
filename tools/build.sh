#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi

# check for build environment, tools and libraries

if [ "$(command -v cmake)" == "" ] ; 
then 
	echo "error: cmake not found." 1>&2
	exit -1
fi

if [ ! -e /etc/os-release ] ;
then
	echo "error: cannot determine distro." 1>&2
	exit -1
else
	source /etc/os-release
	echo "current distro: $ID $VERSION_ID"
fi

# check if we have dotnet sdk
if [ ! -e "$REPO_ROOT/tools/dotnet" ];
then
	tmp=$(tempfile)
	echo "dotnet sdk not found, downloading."
	case "$ID $VERSION_ID" in
	"ubuntu 16.04")
		dotnet_url="https://download.microsoft.com/download/D/7/2/D725E47F-A4F1-4285-8935-A91AE2FCC06A/dotnet-sdk-2.0.3-linux-x64.tar.gz"
		;;
	*)
		echo "error: unsupported distro." 1>&2
		exit -1
		;;
	esac
	wget $dotnet_url -O $tmp || exit -1
	mkdir "$REPO_ROOT/tools/dotnet"
	tar -xzf $tmp -C "$REPO_ROOT/tools/dotnet" || exit -1
	rm $tmp
fi
export PATH="$REPO_ROOT/tools/dotnet":$PATH

# build Trinity.C
build_trinity_c()
{
	echo "Building Trinity.C"
	mkdir -p "$REPO_ROOT/bin" || exit -1
	mkdir -p "$REPO_ROOT/bin/build_trinityc" && pushd "$_" || exit -1
	cmake "$REPO_ROOT/src/Trinity.C" || exit -1
	make || exit -1
	# copy native Trinity.C for Windows-CoreCLR
	cp "$REPO_ROOT/lib/Trinity.dll" "$REPO_ROOT/bin/Trinity.dll" || exit -1
	# copy native Trinity.C.dll for Windows
	cp "$REPO_ROOT/lib/Trinity.C.dll" "$REPO_ROOT/bin/Trinity.C.dll" || exit -1
	# copy freshly built Trinity.C for Linux-CoreCLR
	cp "$REPO_ROOT/bin/build_trinityc/libTrinity.so" "$REPO_ROOT/bin/libTrinity.so" || exit -1
	popd
}

# build Trinity.TSL
build_trinity_tsl()
{
	echo "Building Trinity.TSL"
	mkdir -p "$REPO_ROOT/bin/tsl" && pushd "$_" || exit -1
	cmake "$REPO_ROOT/src/Trinity.TSL" || exit -1
	make || exit -1
	# copy native Trinity.TSL.CodeGen.exe for Windows
	cp "$REPO_ROOT/tools/Trinity.TSL.CodeGen.exe" "$REPO_ROOT/bin/" || exit -1
	# copy freshly built Trinity.TSL.CodeGen for Linux
	cp "Trinity.TSL.CodeGen" "$REPO_ROOT/bin/" || exit -1
	popd
}

# build Trinity.Core
build_trinity_core()
{
	echo "Building Trinity.Core"
	pushd "$REPO_ROOT/src/Trinity.Core"
	dotnet restore Trinity.Core.sln || exit -1
	dotnet build -c Release Trinity.Core.sln || exit -1
	dotnet pack -c Release Trinity.Core.sln || exit -1
	popd
}

# build LIKQ
build_likq()
{
	echo "Building Trinity.Core"
	pushd "$REPO_ROOT/src/Modules/LIKQ"
	dotnet restore LIKQ.sln || exit -1
	dotnet build -c Release LIKQ.sln || exit -1
	dotnet pack -c Release LIKQ.sln || exit -1
	popd
}

# build Client
build_client()
{
	echo "Building Trinity.Client"
	pushd "$REPO_ROOT/src/Modules/GraphEngine.Client"
	dotnet restore GraphEngine.Client.sln || exit -1
	dotnet build -c Release GraphEngine.Client.sln || exit -1
	dotnet pack -c Release GraphEngine.Client.sln || exit -1
	popd
}

# build composite_ext
build_composite_ext()
{
	echo "Building Trinity.CompositeExtension"
	pushd "$REPO_ROOT/src/Modules/Trinity.Storage.CompositeExtension"
	dotnet restore Trinity.Storage.CompositeExtension.sln || exit -1
	dotnet build -c Release Trinity.Storage.CompositeExtension.sln || exit -1
	dotnet pack -c Release Trinity.Storage.CompositeExtension.sln || exit -1
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
build_trinity_tsl
build_trinity_core
setup_nuget_repo
build_likq
build_client
build_composite_ext
