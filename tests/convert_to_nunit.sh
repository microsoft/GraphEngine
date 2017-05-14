#!/bin/bash

set -e

for set in unittests stresstests; do
    find "$set" -name 'packages.config' \
        | while read line
    do
        project_dir=$(dirname "$line")
        project_name=$(basename "$project_dir")
        find "$project_dir" -name "*.cs" -not -path '*AssemblyInfo.cs' -not -path '*/obj/*.cs' | xargs sed -i -f xunit2nunit.sed
        sed "s/test_name/$project_name/g" template/standalone/test_name.csproj > "${project_dir}/${project_name}.csproj"
        sed "s/test_name/$project_name/g" template/standalone/test_name_coreclr.csproj > "${project_dir}/${project_name}_coreclr.csproj"
        cp template/standalone/packages.config "$project_dir/"
        echo $project_name
    done
done

