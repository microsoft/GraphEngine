#!/usr/bin/env bash

if [ "$REPO_ROOT" == "" ] ; then REPO_ROOT="$(readlink -f $(dirname $(readlink -f $0))/../)" ; fi

$REPO_ROOT/tests/run_tests.sh
