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
mkdir -p "$REPO_ROOT/build" && pushd "$_" || exit -1
cmake "$REPO_ROOT" -DCMAKE_BUILD_TYPE=Release || exit -1
make -j || exit -1
popd
