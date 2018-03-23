``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Xeon CPU E5-2690 v3 2.60GHz, 1 CPU, 24 logical cores and 12 physical cores
Frequency=2539061 Hz, Resolution=393.8464 ns, Timer=TSC
.NET Core SDK=2.1.102
  [Host]     : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Job-JIMEBN : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Job-ZAQFPX : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0

Server=True  RunStrategy=Monitoring  

```
|       Method |     Toolchain | TotalCellCount | CellSize | ThreadCount |            Mean |        Error |       StdDev |
|------------- |-------------- |--------------- |--------- |------------ |----------------:|-------------:|-------------:|
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **1** |     **25,287.9 us** |   **1,141.4 us** |     **755.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           1 |     14,274.6 us |   2,710.6 us |   1,792.9 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **2** |     **14,541.8 us** |   **1,025.4 us** |     **678.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           2 |      7,412.0 us |   1,549.3 us |   1,024.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **4** |      **8,488.3 us** |     **930.1 us** |     **615.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           4 |      4,611.2 us |   1,668.8 us |   1,103.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **6** |      **6,107.2 us** |     **827.4 us** |     **547.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           6 |      2,967.6 us |     981.7 us |     649.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **8** |      **5,096.0 us** |     **982.1 us** |     **649.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           8 |      2,547.4 us |     824.0 us |     545.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **10** |      **4,902.9 us** |   **1,348.8 us** |     **892.1 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          10 |      2,335.8 us |     720.9 us |     476.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **12** |      **4,667.6 us** |     **893.6 us** |     **591.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          12 |      2,659.6 us |     768.6 us |     508.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **14** |      **4,384.8 us** |     **975.6 us** |     **645.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          14 |      2,461.3 us |     681.9 us |     451.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **16** |      **4,765.8 us** |   **1,440.4 us** |     **952.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          16 |      2,366.0 us |     698.0 us |     461.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **18** |      **3,953.9 us** |     **831.2 us** |     **549.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          18 |      2,363.7 us |     738.9 us |     488.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **20** |      **3,920.1 us** |     **967.0 us** |     **639.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          20 |      2,614.1 us |   1,021.2 us |     675.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **22** |      **3,797.3 us** |     **849.2 us** |     **561.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          22 |      2,555.8 us |     746.3 us |     493.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **24** |      **4,122.2 us** |   **1,030.8 us** |     **681.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          24 |      2,591.1 us |     705.1 us |     466.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **26** |      **4,150.6 us** |     **863.6 us** |     **571.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          26 |      3,336.9 us |   1,729.5 us |   1,143.9 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **28** |      **4,199.0 us** |     **911.2 us** |     **602.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          28 |      2,733.3 us |     717.8 us |     474.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **30** |      **4,237.6 us** |     **873.8 us** |     **578.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          30 |      2,852.5 us |     749.1 us |     495.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **32** |      **4,602.7 us** |   **1,144.1 us** |     **756.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          32 |      2,940.7 us |     671.6 us |     444.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **34** |      **4,349.1 us** |     **943.3 us** |     **623.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          34 |      3,033.8 us |     648.1 us |     428.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **36** |      **4,486.2 us** |     **907.9 us** |     **600.5 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          36 |      3,185.7 us |     814.8 us |     538.9 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **38** |      **5,139.5 us** |   **1,055.2 us** |     **697.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          38 |      3,273.7 us |     648.9 us |     429.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **40** |      **4,608.3 us** |     **933.2 us** |     **617.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          40 |      3,395.5 us |     672.0 us |     444.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **42** |      **4,639.0 us** |     **904.4 us** |     **598.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          42 |      3,488.8 us |     677.8 us |     448.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **44** |      **5,544.4 us** |   **1,530.4 us** |   **1,012.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          44 |      3,725.6 us |     692.4 us |     458.0 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **46** |      **4,854.8 us** |   **1,041.9 us** |     **689.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          46 |      3,787.6 us |     636.7 us |     421.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **48** |      **4,947.2 us** |     **870.9 us** |     **576.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          48 |      4,090.4 us |     875.5 us |     579.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **50** |      **4,939.5 us** |     **852.5 us** |     **563.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          50 |      3,990.1 us |     623.2 us |     412.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **52** |      **5,046.4 us** |     **917.3 us** |     **606.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          52 |      4,087.8 us |     620.5 us |     410.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **54** |      **5,190.2 us** |   **1,063.4 us** |     **703.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          54 |      4,952.4 us |   1,416.7 us |     937.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **56** |      **5,304.6 us** |     **892.3 us** |     **590.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          56 |      4,304.8 us |     696.7 us |     460.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **58** |      **5,359.4 us** |     **869.0 us** |     **574.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          58 |      4,481.4 us |     561.9 us |     371.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **60** |      **5,624.4 us** |     **988.7 us** |     **653.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          60 |      4,701.4 us |     858.9 us |     568.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **62** |      **5,604.0 us** |     **855.5 us** |     **565.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          62 |      4,859.9 us |     621.3 us |     410.9 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **64** |      **5,734.7 us** |     **939.8 us** |     **621.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          64 |      4,941.0 us |     576.9 us |     381.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **66** |      **6,582.7 us** |   **1,068.3 us** |     **706.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          66 |      4,994.4 us |     629.7 us |     416.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **68** |      **5,918.3 us** |     **928.8 us** |     **614.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          68 |      5,112.2 us |     641.9 us |     424.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **70** |      **6,055.5 us** |     **946.1 us** |     **625.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          70 |      5,214.2 us |     618.5 us |     409.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **72** |      **6,982.1 us** |   **2,305.0 us** |   **1,524.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          72 |      5,305.2 us |     600.0 us |     396.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **74** |      **6,264.4 us** |     **912.2 us** |     **603.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          74 |      5,444.1 us |     851.8 us |     563.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **76** |      **6,413.3 us** |     **986.6 us** |     **652.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          76 |      5,578.4 us |     848.5 us |     561.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **78** |      **6,804.7 us** |   **1,877.5 us** |   **1,241.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          78 |      5,735.9 us |     834.7 us |     552.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **80** |      **6,677.1 us** |     **898.0 us** |     **594.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          80 |      5,848.5 us |     769.6 us |     509.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **82** |      **6,787.9 us** |     **952.6 us** |     **630.1 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          82 |      6,711.1 us |     845.5 us |     559.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **84** |      **6,922.3 us** |     **910.4 us** |     **602.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          84 |      6,402.1 us |     819.7 us |     542.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **86** |      **7,017.1 us** |     **853.7 us** |     **564.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          86 |      6,495.9 us |     564.7 us |     373.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **88** |      **7,177.0 us** |     **913.2 us** |     **604.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          88 |      7,126.2 us |   1,270.3 us |     840.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **90** |      **7,353.6 us** |     **947.8 us** |     **626.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          90 |      6,775.8 us |     540.8 us |     357.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **92** |      **7,485.0 us** |     **907.4 us** |     **600.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          92 |      6,929.2 us |     549.6 us |     363.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **94** |      **7,939.9 us** |   **1,035.6 us** |     **685.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          94 |      6,986.8 us |     516.5 us |     341.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **96** |      **7,672.2 us** |     **963.8 us** |     **637.5 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          96 |      7,126.3 us |     727.0 us |     480.9 us |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **1** |      **5,872.6 us** |     **945.2 us** |     **625.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           1 |      2,221.3 us |     978.5 us |     647.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **2** |      **3,944.2 us** |   **1,146.7 us** |     **758.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           2 |      1,344.7 us |     921.8 us |     609.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **4** |      **2,196.8 us** |     **959.7 us** |     **634.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           4 |      1,006.7 us |     853.6 us |     564.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **6** |      **1,870.4 us** |     **851.2 us** |     **563.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           6 |        953.9 us |     664.3 us |     439.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **8** |      **1,801.8 us** |     **780.0 us** |     **515.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           8 |      1,039.5 us |     587.8 us |     388.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **10** |      **1,801.2 us** |     **810.8 us** |     **536.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          10 |      1,124.4 us |     539.8 us |     357.0 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **12** |      **1,923.5 us** |     **902.7 us** |     **597.1 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          12 |      1,224.4 us |     494.6 us |     327.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **14** |      **2,008.2 us** |     **956.9 us** |     **632.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          14 |      1,333.4 us |     564.7 us |     373.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **16** |      **2,094.4 us** |     **780.0 us** |     **515.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          16 |      1,454.2 us |     556.3 us |     368.0 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **18** |      **2,165.1 us** |     **781.1 us** |     **516.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          18 |      1,651.9 us |     527.1 us |     348.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **20** |      **2,385.4 us** |     **789.7 us** |     **522.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          20 |      1,778.4 us |     528.2 us |     349.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **22** |      **2,443.8 us** |     **828.2 us** |     **547.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          22 |      1,900.4 us |     540.8 us |     357.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **24** |      **2,585.7 us** |     **922.0 us** |     **609.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          24 |      2,006.1 us |     640.6 us |     423.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **26** |      **2,741.0 us** |     **949.8 us** |     **628.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          26 |      2,114.8 us |     553.6 us |     366.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **28** |      **2,870.2 us** |     **948.9 us** |     **627.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          28 |      2,207.7 us |     582.9 us |     385.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **30** |      **2,936.7 us** |     **784.7 us** |     **519.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          30 |      2,308.5 us |     552.8 us |     365.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **32** |      **3,066.0 us** |     **940.3 us** |     **621.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          32 |      2,435.0 us |     587.3 us |     388.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **34** |      **3,228.8 us** |     **866.8 us** |     **573.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          34 |      2,546.2 us |     578.4 us |     382.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **36** |      **3,272.9 us** |     **805.6 us** |     **532.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          36 |      2,708.8 us |     611.2 us |     404.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **38** |      **3,398.8 us** |     **773.1 us** |     **511.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          38 |      2,821.5 us |     577.8 us |     382.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **40** |      **3,563.2 us** |     **926.2 us** |     **612.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          40 |      2,988.0 us |     534.6 us |     353.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **42** |      **3,796.2 us** |     **911.5 us** |     **602.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          42 |      3,160.2 us |     544.8 us |     360.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **44** |      **3,957.2 us** |     **913.8 us** |     **604.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          44 |      3,387.1 us |     697.5 us |     461.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **46** |      **4,158.3 us** |     **952.2 us** |     **629.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          46 |      3,468.1 us |     460.1 us |     304.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **48** |      **4,264.9 us** |     **881.1 us** |     **582.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          48 |      3,649.1 us |     556.0 us |     367.8 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **50** |      **4,434.4 us** |     **921.8 us** |     **609.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          50 |      3,845.4 us |     620.3 us |     410.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **52** |      **4,541.2 us** |     **805.6 us** |     **532.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          52 |      3,960.4 us |     582.8 us |     385.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **54** |      **4,658.5 us** |     **956.3 us** |     **632.5 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          54 |      4,126.1 us |     691.9 us |     457.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **56** |      **4,764.6 us** |     **818.8 us** |     **541.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          56 |      4,199.7 us |     540.1 us |     357.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **58** |      **4,870.3 us** |     **906.8 us** |     **599.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          58 |      4,411.9 us |     565.9 us |     374.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **60** |      **4,995.5 us** |     **871.6 us** |     **576.5 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          60 |      4,522.5 us |     636.1 us |     420.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **62** |      **5,133.4 us** |     **906.0 us** |     **599.3 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          62 |      4,665.7 us |     680.5 us |     450.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **64** |      **5,173.3 us** |     **827.3 us** |     **547.2 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          64 |      4,809.9 us |     522.3 us |     345.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **66** |      **5,352.9 us** |     **899.6 us** |     **595.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          66 |      4,949.4 us |     614.8 us |     406.7 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **68** |      **5,414.2 us** |     **939.1 us** |     **621.1 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          68 |      5,064.1 us |     526.7 us |     348.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **70** |      **5,589.6 us** |   **1,027.7 us** |     **679.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          70 |      5,236.3 us |     529.9 us |     350.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **72** |      **5,707.5 us** |   **1,082.0 us** |     **715.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          72 |      5,405.5 us |     586.0 us |     387.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **74** |      **5,770.4 us** |     **981.1 us** |     **648.9 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          74 |      5,543.6 us |     542.9 us |     359.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **76** |      **5,857.5 us** |     **913.7 us** |     **604.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          76 |      5,656.5 us |     647.1 us |     428.0 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **78** |      **5,955.3 us** |   **1,071.0 us** |     **708.4 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          78 |      5,764.8 us |     586.7 us |     388.1 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **80** |      **6,077.8 us** |     **924.9 us** |     **611.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          80 |      5,883.0 us |     573.2 us |     379.2 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **82** |      **6,278.6 us** |   **1,000.2 us** |     **661.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          82 |      6,043.7 us |     599.2 us |     396.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **84** |      **6,358.2 us** |     **888.5 us** |     **587.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          84 |      6,239.0 us |     553.8 us |     366.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **86** |      **6,447.4 us** |     **899.3 us** |     **594.8 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          86 |      6,354.5 us |     699.3 us |     462.5 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **88** |      **6,700.9 us** |   **1,071.9 us** |     **709.0 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          88 |      6,532.8 us |     591.2 us |     391.0 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **90** |      **6,741.5 us** |     **838.6 us** |     **554.7 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          90 |      6,629.2 us |     646.0 us |     427.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **92** |      **6,878.0 us** |     **983.6 us** |     **650.6 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          92 |      6,745.1 us |     584.1 us |     386.3 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **94** |      **7,073.9 us** |   **1,090.2 us** |     **721.1 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          94 |      6,839.7 us |     596.6 us |     394.6 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **96** |      **7,239.5 us** |     **971.4 us** |     **642.5 us** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          96 |      7,012.6 us |     637.1 us |     421.4 us |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **1** | **12,280,772.5 us** | **279,094.6 us** | **184,603.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           1 |  9,033,981.6 us | 290,527.9 us | 192,166.2 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **2** |  **6,991,821.6 us** | **136,314.7 us** |  **90,163.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           2 |  4,530,474.8 us |  94,008.1 us |  62,180.5 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **4** |  **3,679,534.7 us** | **147,569.0 us** |  **97,607.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           4 |  2,358,648.6 us |  36,463.4 us |  24,118.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **6** |  **2,593,503.6 us** | **152,430.4 us** | **100,823.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           6 |  1,616,744.8 us |  93,461.6 us |  61,819.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **8** |  **2,032,411.7 us** | **113,108.1 us** |  **74,814.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           8 |  1,237,920.1 us |  35,958.9 us |  23,784.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **10** |  **1,719,197.6 us** |  **75,096.0 us** |  **49,671.4 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          10 |    997,219.9 us |  10,809.0 us |   7,149.5 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **12** |  **1,431,450.5 us** |  **84,537.3 us** |  **55,916.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          12 |    899,948.9 us |  70,015.5 us |  46,310.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **14** |  **1,389,717.8 us** |  **46,147.4 us** |  **30,523.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          14 |    908,178.8 us |  25,460.2 us |  16,840.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **16** |  **1,299,305.5 us** |  **27,210.1 us** |  **17,997.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          16 |    822,036.4 us |  33,930.1 us |  22,442.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **18** |  **1,201,779.2 us** |  **18,259.0 us** |  **12,077.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          18 |    745,166.9 us |  20,223.2 us |  13,376.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **20** |  **1,103,874.7 us** |  **14,907.2 us** |   **9,860.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          20 |    696,875.5 us |  30,933.9 us |  20,460.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **22** |  **1,040,188.5 us** |  **11,054.5 us** |   **7,311.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          22 |    631,619.5 us |  12,750.2 us |   8,433.5 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **24** |  **1,119,202.5 us** | **183,197.2 us** | **121,173.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          24 |    683,385.8 us |  78,465.9 us |  51,900.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **26** |  **2,197,822.4 us** | **238,480.5 us** | **157,740.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          26 |    921,897.1 us |  14,946.0 us |   9,885.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **28** |  **2,278,592.1 us** | **274,621.7 us** | **181,645.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          28 |    870,172.2 us |   8,840.8 us |   5,847.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **30** |  **2,686,660.4 us** | **278,237.4 us** | **184,036.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          30 |    831,063.9 us |  30,697.2 us |  20,304.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **32** |  **2,608,729.1 us** | **311,336.4 us** | **205,929.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          32 |    781,326.6 us |  30,536.2 us |  20,197.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **34** |  **2,849,769.5 us** | **317,168.3 us** | **209,787.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          34 |    743,976.1 us |  28,100.9 us |  18,587.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **36** |  **2,871,832.7 us** | **387,406.6 us** | **256,245.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          36 |    699,393.9 us |  11,520.4 us |   7,620.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **38** |  **2,707,887.0 us** | **503,193.9 us** | **332,831.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          38 |    706,994.8 us |   9,273.9 us |   6,134.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **40** |  **2,456,580.8 us** | **359,766.3 us** | **237,963.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          40 |    676,917.5 us |   6,652.3 us |   4,400.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **42** |  **2,641,735.6 us** | **284,036.4 us** | **187,872.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          42 |    655,691.0 us |  14,424.6 us |   9,541.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **44** |  **2,467,925.8 us** | **321,854.6 us** | **212,886.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          44 |    630,317.7 us |   8,442.6 us |   5,584.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **46** |  **2,542,019.4 us** | **216,665.5 us** | **143,310.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          46 |    612,128.9 us |  21,493.6 us |  14,216.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **48** |  **2,407,023.4 us** | **508,756.0 us** | **336,510.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          48 |    660,075.3 us |  66,398.9 us |  43,918.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **50** |  **2,082,917.2 us** | **320,349.9 us** | **211,891.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          50 |    753,756.2 us |  11,794.8 us |   7,801.5 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **52** |  **2,276,448.7 us** | **354,467.4 us** | **234,458.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          52 |    725,867.1 us |   9,808.5 us |   6,487.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **54** |  **2,198,869.5 us** | **241,930.3 us** | **160,021.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          54 |    705,159.0 us |   6,701.1 us |   4,432.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **56** |  **1,996,578.1 us** | **273,735.2 us** | **181,058.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          56 |    680,888.7 us |   5,050.2 us |   3,340.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **58** |  **2,454,586.3 us** | **252,605.8 us** | **167,083.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          58 |    670,960.7 us |  20,087.8 us |  13,286.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **60** |  **2,293,596.8 us** | **393,493.9 us** | **260,271.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          60 |    652,081.2 us |  15,265.7 us |  10,097.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **62** |  **2,223,502.9 us** | **238,271.4 us** | **157,601.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          62 |    656,787.7 us |   9,887.9 us |   6,540.2 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **64** |  **2,303,973.4 us** | **335,603.1 us** | **221,980.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          64 |    641,694.2 us |   7,365.4 us |   4,871.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **66** |  **2,272,790.6 us** | **295,793.9 us** | **195,649.4 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          66 |    626,895.3 us |  11,947.8 us |   7,902.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **68** |  **2,323,811.5 us** | **555,447.2 us** | **367,393.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          68 |    615,929.7 us |  14,050.5 us |   9,293.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **70** |  **2,031,021.6 us** | **258,827.8 us** | **171,198.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          70 |    597,742.2 us |   9,999.7 us |   6,614.2 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **72** |  **2,055,769.4 us** | **266,677.4 us** | **176,390.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          72 |    649,854.5 us |  73,319.6 us |  48,496.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **74** |  **2,217,291.1 us** | **397,491.2 us** | **262,915.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          74 |    689,475.6 us |   9,323.4 us |   6,166.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **76** |  **2,202,874.1 us** | **377,456.9 us** | **249,664.4 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          76 |    675,317.6 us |   6,647.4 us |   4,396.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **78** |  **2,245,688.4 us** | **257,998.8 us** | **170,650.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          78 |    664,867.2 us |   8,100.0 us |   5,357.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **80** |  **2,090,640.4 us** | **245,519.9 us** | **162,396.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          80 |    644,395.9 us |   6,362.8 us |   4,208.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **82** |  **2,176,145.2 us** | **289,761.2 us** | **191,659.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          82 |    640,374.2 us |  10,589.9 us |   7,004.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **84** |  **2,144,683.8 us** | **250,253.5 us** | **165,527.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          84 |    632,662.1 us |  10,246.0 us |   6,777.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **86** |  **2,156,073.1 us** | **112,829.6 us** |  **74,629.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          86 |    631,454.0 us |   4,287.3 us |   2,835.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **88** |  **1,989,160.5 us** | **212,597.5 us** | **140,620.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          88 |    621,032.1 us |   7,307.6 us |   4,833.5 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **90** |  **2,031,408.9 us** | **340,771.1 us** | **225,399.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          90 |    608,580.8 us |   3,753.6 us |   2,482.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **92** |  **1,955,458.5 us** | **214,748.6 us** | **142,042.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          92 |    603,687.9 us |   5,091.9 us |   3,368.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **94** |  **1,981,427.0 us** | **213,202.6 us** | **141,020.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          94 |    595,641.1 us |  13,008.0 us |   8,604.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **96** |  **1,852,422.8 us** | **197,749.6 us** | **130,799.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          96 |    648,107.8 us |  56,113.9 us |  37,115.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **1** |  **2,079,095.7 us** |  **98,358.5 us** |  **65,058.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           1 |  1,099,189.0 us |  14,544.2 us |   9,620.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **2** |  **1,162,209.8 us** |  **28,924.9 us** |  **19,132.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           2 |    554,749.5 us |   6,498.0 us |   4,298.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **4** |    **601,236.5 us** |   **9,363.0 us** |   **6,193.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           4 |    289,760.2 us |   5,369.9 us |   3,551.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **6** |    **422,009.1 us** |  **14,184.2 us** |   **9,382.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           6 |    198,848.1 us |   2,734.6 us |   1,808.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **8** |    **330,484.6 us** |  **10,053.1 us** |   **6,649.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           8 |    153,570.7 us |   3,443.2 us |   2,277.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **10** |    **276,249.8 us** |   **8,834.1 us** |   **5,843.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          10 |    125,467.5 us |   4,062.0 us |   2,686.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **12** |    **259,914.2 us** |  **12,407.3 us** |   **8,206.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          12 |    116,933.7 us |  13,427.3 us |   8,881.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **14** |    **254,590.6 us** |   **6,506.1 us** |   **4,303.4 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          14 |    110,601.2 us |   3,271.7 us |   2,164.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **16** |    **245,087.5 us** |   **5,844.2 us** |   **3,865.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          16 |    103,682.0 us |   4,247.4 us |   2,809.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **18** |    **243,773.5 us** |   **7,385.3 us** |   **4,884.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          18 |     92,980.5 us |   3,879.9 us |   2,566.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **20** |    **246,106.4 us** |   **9,745.3 us** |   **6,445.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          20 |     83,306.2 us |   5,241.2 us |   3,466.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **22** |    **275,122.5 us** |  **13,937.4 us** |   **9,218.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          22 |     76,125.6 us |   1,948.3 us |   1,288.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **24** |    **304,355.5 us** |   **8,510.1 us** |   **5,628.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          24 |     76,781.5 us |  26,190.4 us |  17,323.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **26** |    **630,871.0 us** |  **87,749.1 us** |  **58,040.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          26 |    109,012.8 us |   3,705.7 us |   2,451.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **28** |    **666,925.8 us** |  **95,849.1 us** |  **63,398.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          28 |    104,144.4 us |   3,419.2 us |   2,261.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **30** |    **629,221.3 us** | **125,271.4 us** |  **82,859.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          30 |     97,530.4 us |   2,956.0 us |   1,955.2 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **32** |    **610,536.4 us** |  **95,827.3 us** |  **63,383.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          32 |     95,828.9 us |   4,494.7 us |   2,973.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **34** |    **614,850.5 us** | **147,362.4 us** |  **97,471.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          34 |     92,495.8 us |   3,175.0 us |   2,100.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **36** |    **620,116.5 us** | **106,086.5 us** |  **70,169.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          36 |     88,512.4 us |   2,270.3 us |   1,501.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **38** |    **633,683.4 us** |  **81,073.2 us** |  **53,624.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          38 |     85,471.3 us |   1,656.8 us |   1,095.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **40** |    **638,049.6 us** | **167,598.6 us** | **110,856.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          40 |     81,505.2 us |   1,487.2 us |     983.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **42** |    **629,873.3 us** |  **98,504.8 us** |  **65,154.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          42 |     77,641.1 us |   1,499.5 us |     991.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **44** |    **612,243.3 us** | **151,145.6 us** |  **99,973.4 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          44 |     75,308.7 us |   1,226.6 us |     811.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **46** |    **627,346.8 us** |  **85,393.5 us** |  **56,482.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          46 |     73,229.4 us |   1,160.6 us |     767.7 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **48** |    **550,476.9 us** | **179,612.5 us** | **118,802.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          48 |     73,926.3 us |   5,821.8 us |   3,850.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **50** |    **526,575.3 us** |  **75,494.5 us** |  **49,934.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          50 |     90,439.3 us |   3,970.5 us |   2,626.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **52** |    **622,651.2 us** | **106,159.6 us** |  **70,218.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          52 |     89,859.6 us |   3,902.9 us |   2,581.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **54** |    **619,767.5 us** |  **85,430.3 us** |  **56,506.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          54 |     86,792.6 us |   3,868.6 us |   2,558.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **56** |    **625,787.8 us** |  **61,136.6 us** |  **40,438.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          56 |     86,571.9 us |   4,223.8 us |   2,793.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **58** |    **612,945.4 us** | **113,363.6 us** |  **74,983.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          58 |     84,616.1 us |   1,864.1 us |   1,233.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **60** |    **626,351.5 us** |  **57,082.1 us** |  **37,756.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          60 |     81,913.5 us |   2,062.8 us |   1,364.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **62** |    **629,790.9 us** | **111,110.9 us** |  **73,493.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          62 |     80,318.2 us |   1,361.1 us |     900.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **64** |    **621,163.3 us** |  **84,956.9 us** |  **56,193.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          64 |     78,242.5 us |     904.3 us |     598.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **66** |    **626,866.3 us** |  **92,231.0 us** |  **61,005.1 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          66 |     76,434.9 us |   1,327.2 us |     877.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **68** |    **621,367.9 us** | **104,828.8 us** |  **69,337.8 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          68 |     74,358.1 us |   1,078.9 us |     713.6 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **70** |    **612,471.6 us** |  **84,385.4 us** |  **55,815.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          70 |     73,070.8 us |     944.8 us |     624.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **72** |    **617,335.9 us** | **154,123.1 us** | **101,942.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          72 |     76,204.5 us |   5,703.3 us |   3,772.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **74** |    **637,548.6 us** |  **93,385.5 us** |  **61,768.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          74 |     83,347.7 us |     849.6 us |     562.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **76** |    **634,975.7 us** |  **93,671.1 us** |  **61,957.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          76 |     83,629.2 us |   3,305.3 us |   2,186.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **78** |    **638,301.4 us** |  **98,350.2 us** |  **65,052.6 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          78 |     80,989.8 us |   2,490.1 us |   1,647.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **80** |    **624,477.5 us** | **100,537.3 us** |  **66,499.2 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          80 |     80,536.9 us |   2,275.1 us |   1,504.8 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **82** |    **618,856.3 us** |  **70,818.4 us** |  **46,842.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          82 |     77,464.9 us |   2,818.0 us |   1,863.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **84** |    **618,479.0 us** |  **95,709.6 us** |  **63,306.0 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          84 |     77,568.6 us |   1,455.0 us |     962.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **86** |    **612,933.6 us** | **117,108.3 us** |  **77,459.9 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          86 |     76,988.4 us |     808.6 us |     534.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **88** |    **615,246.2 us** | **113,101.0 us** |  **74,809.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          88 |     76,098.3 us |   1,516.4 us |   1,003.0 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **90** |    **630,158.2 us** | **109,105.2 us** |  **72,166.3 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          90 |     74,572.0 us |     928.9 us |     614.4 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **92** |    **619,705.6 us** |  **94,061.0 us** |  **62,215.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          92 |     73,551.7 us |   1,046.3 us |     692.1 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **94** |    **621,220.2 us** | **109,462.5 us** |  **72,402.7 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          94 |     73,517.9 us |     875.3 us |     578.9 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **96** |    **516,506.7 us** |  **83,987.5 us** |  **55,552.5 us** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          96 |     78,807.0 us |   2,461.7 us |   1,628.3 us |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          96 |              NA |           NA |           NA |

Benchmarks with issues:
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-ZAQFPX(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=96]
