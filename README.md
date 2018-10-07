# Graph Engine - Open Source

| - | Windows Multi Targeting | Ubuntu 16.04 .NET Core |
|:------:|:------:|:------:|
|Build|[<img src="https://trinitygraphengine.visualstudio.com/_apis/public/build/definitions/4cfbb293-cd2c-4f49-aa03-06894081c93b/3/badge"/>](https://trinitygraphengine.visualstudio.com/trinity-ci/_build/index?definitionId=3)|[<img src="https://trinitygraphengine.visualstudio.com/_apis/public/build/definitions/4cfbb293-cd2c-4f49-aa03-06894081c93b/4/badge"/>](https://trinitygraphengine.visualstudio.com/trinity-ci/_build/index?definitionId=4)|
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

### Graph Engine SDK
- Search "Graph Engine" in Visual Studio Extensions and Updates (Recommended).
- Alternatively, download [Graph Engine VSExtension](https://visualstudiogallery.msdn.microsoft.com/12835dd2-2d0e-4b8e-9e7e-9f505bb909b8) from Visual Studio Gallery.
- A NuGet package [Graph Engine Core](https://www.nuget.org/packages/GraphEngine.Core/) is available in NuGet Gallery.

## How to Contribute

Pull requests, issue reports, and suggestions are welcome.

If you are interested in contributing to the code, please fork the
repository and submit pull requests to the `master` branch.

Please submit bugs and feature requests in [GitHub Issues](https://github.com/Microsoft/GraphEngine/issues).

## Building for Windows

Install [Visual Studio 2017](https://www.visualstudio.com/).
Make sure to install the following workloads and components:

- .NET desktop development
- Desktop development with C++
- cmake
- `.NET Core SDK 2.0` or above

The Windows build will generate multi-targeting nuget packages for all the available modules.
Run `tools/build.ps1` with `powershell`. It will setup a workspace folder `build`, and build with `cmake`.
The Linux native assemblies will be automatically packaged (pre-built at `lib`), so the
Windows build will also work for Linux `.Net Core`.

Nuget packages will be built and put at
`build/GraphEngine**._version_.nupkg`. The folder `build/` will be
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
`build/GraphEngine**._version_.nupkg`. The folder `build/` will be
registered as a local NuGet repository and the local package cache for
`GraphEngine.Core` will be cleared. After the packages are built, you
can run `dotnet restore` to use the newly built package.


Note: the build script currently only supports `Ubuntu 16.04`.

# License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.md) License.
