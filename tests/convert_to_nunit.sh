#!/bin/bash

find . -name '*.cs' -not -path './**/*AssemblyInfo.cs' -not -path './**/obj/**/*.cs' -not -path './new_test.cs' \
  | xargs sed -i -f xunit2nunit.sed

