# Microsoft Graph Engine

| - | Windows | Linux |
|:------:|:------:|:------:|
|Build|![Build status badge](https://msai.visualstudio.com/GraphEngine/_apis/build/status/GraphEngine-Windows)|![Build status badge](https://msai.visualstudio.com/GraphEngine/_apis/build/status/GraphEngine-Linux)|

This repository contains the source code of [Microsoft Graph
Engine][graph-engine] and its graph query language -- [Language Integrated
Knowledge Query][likq] (LIKQ).

Microsoft Graph Engine is a distributed in-memory data processing engine,
underpinned by a strongly-typed in-memory key-value store and a general-purpose
distributed computation engine.

[LIKQ][likq-gh] is a versatile graph query language built atop Graph Engine. It
combines the capability of fast graph exploration with the flexibility of lambda
expressions. Server-side computations can be expressed in lambda expressions,
embedded in LIKQ, and executed on the Graph Engine servers during graph
traversal.

## Recent changes

The main version number is bumped to 3.0 to reflect the recent toolchain updates.
- .NET 6.0 is now the mainly supported target framework
- Visual Studio 2022 is supported

One goal of Graph Engine 3.0 is to bring the system up to date and make it
slimmer. Some obsolete or outdated modules and tools have been moved to the
[Unsupported](src/Modules/Unsupported) directory and removed from the main build pipeline.

## Getting started

Recommended operating system: Windows 10 or Ubuntu 20.04.

### Building on Windows

Download and install [Visual Studio][vs] with the following "workloads" and
"individual components" selected:

- The ".NET desktop development" and "Desktop development with C++" workloads.
- The ".NET Portable Library targeting pack" individual component.

Open a PowerShell window, run `tools/build.ps1` for building the NuGet packages.
The script has been tested on Windows 10 (21H2) with Visual Studio 2022.

### Building on Linux

Install g++, cmake, and libssl-dev. For example, on Ubuntu, simply run

```shell
sudo apt update && sudo apt install g++ cmake libssl-dev
```

Install [.NET SDK x64 6.0][dotnet6-on-ubuntu20-04] and run:

```shell
bash tools/build.sh
```

The build script has been tested on Ubuntu 20.04 with g++ 9.4.0.

### Using the built packages

You can find the built NuGet packages `build/GraphEngine**._version_.nupkg` in
the `build` folder. In the building process, the `build` directory has been
registered as a local NuGet repository and the local package cache for
`GraphEngine.Core` has been cleared. After the packages are built, run `dotnet
restore` to use the newly built packages.

### Running your first Graph Engine app

Go to the `samples/Friends` folder, execute `dotnet restore` and `dotnet run` to
run the sample project.

## Contributing

Pull requests, issue reports, and suggestions are welcome.

Please read the [code of conduct](CODE_OF_CONDUCT.md) before contributing code.

Follow these [instructions](SECURITY.md) for reporting security issues.

## License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.md) license.

## Disclaimer

Microsoft Graph Engine is a research project. It is not an officially supported Microsoft product.

<!--
Links
-->

[graph-engine]: https://www.graphengine.io/

[likq]: https://www.graphengine.io/video/likq.video.html

[likq-gh]: https://github.com/Microsoft/GraphEngine/tree/master/src/Modules/LIKQ

[academic-graph-search]: https://azure.microsoft.com/en-us/services/cognitive-services/academic-knowledge/

[vs-extension]: https://visualstudiogallery.msdn.microsoft.com/12835dd2-2d0e-4b8e-9e7e-9f505bb909b8

[graph-engine-core]: https://www.nuget.org/packages/GraphEngine.Core/

[likq-nuget]: https://www.nuget.org/packages/GraphEngine.LIKQ/

[vs]: https://www.visualstudio.com/

[dotnet-download]: https://dotnet.microsoft.com/

[dotnet6-on-ubuntu20-04]: https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#2004

[license]: LICENSE.md
