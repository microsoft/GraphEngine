# Graph Engine - Open Source

| - | Windows .NET Framework | Windows .NET Core | Ubuntu 16.04 .NET Core |
|:------:|:------:|:------:|:------:|
|Build|[![Build Status](http://ci.graphengine.io/buildStatus/icon?job=GraphEngine-Windows-NetFX)](http://ci.graphengine.io/job/GraphEngine-Windows-NetFX/)|[![Build Status](http://ci.graphengine.io/buildStatus/icon?job=GraphEngine-Windows-NetCore)](http://ci.graphengine.io/job/GraphEngine-Windows-NetCore/)|[![Build Status](http://ci.graphengine.io/buildStatus/icon?job=GraphEngine-Ubuntu-NetCore)](http://ci.graphengine.io/job/GraphEngine-Ubuntu-NetCore/)
|Tests|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Windows-NetFx.svg)](http://ci.graphengine.io/job/GraphEngine-Windows-NetFx/)|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Windows-NetCore.svg)](http://ci.graphengine.io/job/GraphEngine-Windows-NetCore/)|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Ubuntu-NetCore.svg)](http://ci.graphengine.io/job/GraphEngine-Ubuntu-NetCore/)|
|Stress|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Windows-NetFx-Stress.svg)](http://ci.graphengine.io/job/GraphEngine-Windows-NetFx-Stress/)|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Windows-NetCore-Stress.svg)](http://ci.graphengine.io/job/GraphEngine-Windows-NetCore-Stress/)|[![Tests](https://img.shields.io/jenkins/t/http/ci.graphengine.io/job/GraphEngine-Ubuntu1604-NetCore-Stress.svg)](http://ci.graphengine.io/job/GraphEngine-Ubuntu-NetCore-Stress/)|

Microsoft [Graph Engine](http://www.graphengine.io/) is a distributed
in-memory data processing engine, underpinned by a strongly-typed
in-memory key-value store and a general distributed computation
engine.

This repository contains the source code of Graph Engine and its graph
query language -- <a
href="https://www.graphengine.io/video/likq.video.html"
target="_blank">Language Integrated Knowledge Query</a> (LIKQ).
[LIKQ](https://github.com/Microsoft/GraphEngine/tree/master/src/Modules/LIKQ)
is a versatile graph query language on top of Graph Engine. It
combines the capability of fast graph exploration and the flexibility
of lambda expression: server-side computations can be expressed in
lambda expressions, embedded in LIKQ, and executed on the server side
during graph traversal.  LIKQ is powering [Academic Graph Search
API](https://azure.microsoft.com/en-us/services/cognitive-services/academic-knowledge/),
which is part of Microsoft Cognitive Services.

## Downloads

Graph Engine is regularly released with bug fixes and feature enhancements.

You can install Graph Engine Visual Studio Extension by searching
"Graph Engine" in Visual Studio _Extensions and Updates_
(recommended). It can also be downloaded from <a
href="https://visualstudiogallery.msdn.microsoft.com/12835dd2-2d0e-4b8e-9e7e-9f505bb909b8" target="_blank">Visual
Studio Gallery</a>.

NuGet packages <a
href="https://www.nuget.org/packages/GraphEngine.Core/"
target="_blank">Graph Engine Core</a> and <a
href="https://www.nuget.org/packages/GraphEngine.LIKQ/"
target="_blank">LIKQ</a> are available in the NuGet Gallery.

## How to Contribute

Pull requests, issue reports, and suggestions are welcome.

If you are interested in contributing to the code, please fork the
repository and submit pull requests to the `master` branch.

Please submit bugs and feature requests in [GitHub Issues](https://github.com/Microsoft/GraphEngine/issues).

## Building for Windows

To build the `.Net Framework` version, install [Visual Studio 2017](https://www.visualstudio.com/) and run `tools/build.bat`.

To build the `CoreCLR` version, you need to download and install the
latest [CoreCLR 2.0
SDK](https://dotnetcli.blob.core.windows.net/dotnet/Sdk/master/dotnet-dev-win-x64.latest.exe).
Then, run `tools/build_coreclr.bat`.

## Building for Linux

Install `libunwind8`, `g++`, `cmake` and `libssl-dev`, then execute
`tools/build.sh`.  When the build script is executed for the first
time, it will download and install the latest CoreCLR 2.0 SDK to
`tools/dotnet`.  A nuget package will be built and put at
`bin/coreclr/GraphEngine.CoreCLR._version_.nupkg`. The folder
`bin/coreclr` will be registered as a local NuGet repository and the
local package cache for `GraphEngine.CoreCLR` will be cleared. After
the package is built, you can run `dotnet restore` to use the newly
built package.

Note: the build script currently only supports `Ubuntu 16.04`.

# License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.md) License.
