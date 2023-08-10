#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi

sudo apt update
sudo apt install -y g++ cmake libssl-dev dotnet-sdk-7.0

echo "Print system info:"
lsb_release -a
uname -a
make --version
g++ --version
dotnet --list-sdks

if [ "$(command -v cmake)" == "" ] ;
then
	echo "error: cmake not found." 1>&2
	exit
fi

if [ "$(command -v dotnet)" == "" ] ;
then
	echo "error: dotnet not found." 1>&2
	echo "see: https://www.microsoft.com/net/download/linux"
	exit
fi

# build
mkdir -p "$REPO_ROOT/build" && pushd "$_" || exit
cmake "$REPO_ROOT" -DCMAKE_BUILD_TYPE=Release || exit
make -j $(nproc) || exit
popd
