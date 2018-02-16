# Graph Engine - Open Source

| - | Windows Multi Targeting | Ubuntu 16.04 .NET Core |
|:------:|:------:|:------:|
|Build|[<img src="https://trinitygraphengine.visualstudio.com/_apis/public/build/definitions/4cfbb293-cd2c-4f49-aa03-06894081c93b/3/badge"/>](https://trinitygraphengine.visualstudio.com/trinity-ci/_build/index?definitionId=3)|_|
|Tests|_|_|
|Stress|_|_|

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

Install [Visual Studio 2017](https://www.visualstudio.com/).
Install Windows 10 SDK (10.0.15063.0) for Desktop C++. 
The Windows build will generate multi-targeting nuget packages for all
the available modules, so make sure you also install `.NET Core SDK
2.0`.  Additionally, to build the python FFI packages, make sure you
install a python3 environment, and the Visual Studio 2015 C++
toolchain. All these dependencies can be installed with the visual
studio 2017 installer (yes, including MSVC 2015 stuff).

Run `tools/build.ps1` with `powershell`. 

The Linux native assemblies will be automatically packaged, so the
Windows build will also work for Linux `.Net Core`.

Nuget packages will be built and put at
`bin/GraphEngine**._version_.nupkg`. The folder `bin/` will be
registered as a local NuGet repository and the local package cache for
`GraphEngine.**` will be cleared. After the packages are built, you
can run `dotnet restore` to use the newly built package.

## Building for Linux

Install `libunwind8`, `g++`, `cmake` and `libssl-dev`.
Install `dotnet` package following [the official guide](https://www.microsoft.com/net/learn/get-started/linuxubuntu).

Execute `tools/build.sh`. 

The Windows native assemblies will be automatically packaged, so the
Linux build will also work for Windows .Net Core. Note, as targeting
`.Net Framework` is not supported, the packages built on Linux are not
equivalent to their Windows builds, and will only support `.Net Core`.

Nuget packages will be built and put at
`bin/GraphEngine**._version_.nupkg`. The folder `bin/` will be
registered as a local NuGet repository and the local package cache for
`GraphEngine.Core` will be cleared. After the packages are built, you
can run `dotnet restore` to use the newly built package.


Note: the build script currently only supports `Ubuntu 16.04`.

# License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.md) License.
