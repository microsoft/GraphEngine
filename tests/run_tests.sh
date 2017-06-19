#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi
export PATH="$REPO_ROOT/tools/dotnet":$PATH

dotnet build "$REPO_ROOT/tests/test_coreclr.csproj"
