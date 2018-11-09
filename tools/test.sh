#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi

# check for build environment, tools and libraries

if [ "$(command -v cmake)" == "" ] ; 
then 
	echo "error: cmake not found." 1>&2
	exit -1
fi

# test
mkdir -p "$REPO_ROOT/build" && pushd "$_" || exit -1
ctest -C Release
popd
