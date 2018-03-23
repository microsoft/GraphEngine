``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2068)
Intel Xeon CPU E5-2690 v3 2.60GHz, 1 CPU, 24 logical cores and 12 physical cores
Frequency=2539062 Hz, Resolution=393.8462 ns, Timer=TSC
  [Host]     : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0
  Job-JFKWFM : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Job-KNTBNH : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0

Server=True  RunStrategy=Monitoring  

```
|       Method |     Toolchain | TotalCellCount | CellSize | ThreadCount |          Mean |       Error |      StdDev |
|------------- |-------------- |--------------- |--------- |------------ |--------------:|------------:|------------:|
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **1** |     **30.531 ms** |   **0.9352 ms** |   **0.6186 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           1 |     17.004 ms |   2.6173 ms |   1.7312 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           1 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           1 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **2** |     **17.402 ms** |   **1.0244 ms** |   **0.6775 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           2 |      8.719 ms |   1.6075 ms |   1.0632 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           2 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           2 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **4** |     **10.109 ms** |   **0.9714 ms** |   **0.6425 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           4 |      5.126 ms |   1.7824 ms |   1.1789 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           4 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           4 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **6** |      **7.055 ms** |   **0.9470 ms** |   **0.6264 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           6 |      3.633 ms |   1.1578 ms |   0.7658 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           6 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           6 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |           **8** |      **5.918 ms** |   **0.8549 ms** |   **0.5655 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |           8 |      3.010 ms |   0.8231 ms |   0.5444 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |           8 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |           8 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **10** |      **5.717 ms** |   **1.5434 ms** |   **1.0209 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          10 |      2.770 ms |   0.7120 ms |   0.4710 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          10 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          10 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **12** |      **5.440 ms** |   **1.1004 ms** |   **0.7279 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          12 |      3.239 ms |   0.8150 ms |   0.5391 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          12 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          12 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **14** |      **5.153 ms** |   **0.8303 ms** |   **0.5492 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          14 |      2.997 ms |   0.7206 ms |   0.4766 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          14 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          14 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **16** |      **5.792 ms** |   **1.6005 ms** |   **1.0586 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          16 |      2.895 ms |   0.7548 ms |   0.4992 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          16 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          16 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **18** |      **4.802 ms** |   **0.9875 ms** |   **0.6532 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          18 |      2.890 ms |   0.7265 ms |   0.4805 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          18 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          18 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **20** |      **4.661 ms** |   **0.8484 ms** |   **0.5611 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          20 |      3.024 ms |   0.6627 ms |   0.4384 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          20 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          20 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **22** |      **5.430 ms** |   **1.4119 ms** |   **0.9339 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          22 |      3.258 ms |   0.7505 ms |   0.4964 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          22 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          22 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **24** |      **6.357 ms** |   **6.1083 ms** |   **4.0403 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          24 |      3.322 ms |   0.9278 ms |   0.6137 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          24 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          24 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **26** |      **5.082 ms** |   **0.8514 ms** |   **0.5632 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          26 |      3.749 ms |   1.8191 ms |   1.2032 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          26 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          26 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **28** |      **5.532 ms** |   **1.6380 ms** |   **1.0834 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          28 |      3.407 ms |   0.7460 ms |   0.4934 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          28 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          28 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **30** |      **5.222 ms** |   **0.9264 ms** |   **0.6128 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          30 |      3.809 ms |   1.0731 ms |   0.7098 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          30 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          30 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **32** |      **5.342 ms** |   **0.9403 ms** |   **0.6219 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          32 |      3.876 ms |   1.0482 ms |   0.6933 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          32 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          32 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **34** |      **5.422 ms** |   **0.8157 ms** |   **0.5395 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          34 |      3.898 ms |   0.6651 ms |   0.4399 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          34 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          34 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **36** |      **5.516 ms** |   **0.8876 ms** |   **0.5871 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          36 |      4.059 ms |   0.7592 ms |   0.5021 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          36 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          36 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **38** |      **5.687 ms** |   **1.0248 ms** |   **0.6779 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          38 |      5.244 ms |   2.1349 ms |   1.4121 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          38 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          38 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **40** |      **5.891 ms** |   **0.8307 ms** |   **0.5494 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          40 |      4.422 ms |   0.6544 ms |   0.4328 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          40 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          40 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **42** |      **5.893 ms** |   **0.8852 ms** |   **0.5855 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          42 |      4.636 ms |   0.6269 ms |   0.4146 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          42 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          42 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **44** |      **6.048 ms** |   **0.9046 ms** |   **0.5983 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          44 |      4.911 ms |   0.9132 ms |   0.6040 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          44 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          44 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **46** |      **6.201 ms** |   **0.9941 ms** |   **0.6575 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          46 |      5.043 ms |   0.7894 ms |   0.5221 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          46 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          46 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **48** |      **6.307 ms** |   **0.8743 ms** |   **0.5783 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          48 |      5.346 ms |   0.8542 ms |   0.5650 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          48 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          48 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **50** |      **7.670 ms** |   **2.1685 ms** |   **1.4343 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          50 |      5.461 ms |   0.6314 ms |   0.4176 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          50 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          50 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **52** |      **6.573 ms** |   **0.9092 ms** |   **0.6014 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          52 |      5.453 ms |   0.6828 ms |   0.4516 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          52 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          52 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **54** |      **6.607 ms** |   **0.8996 ms** |   **0.5950 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          54 |      5.512 ms |   0.6692 ms |   0.4426 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          54 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          54 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **56** |      **7.687 ms** |   **1.3476 ms** |   **0.8914 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          56 |      5.828 ms |   0.7504 ms |   0.4963 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          56 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          56 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **58** |      **7.027 ms** |   **0.9177 ms** |   **0.6070 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          58 |      6.011 ms |   0.6244 ms |   0.4130 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          58 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          58 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **60** |      **7.052 ms** |   **0.8751 ms** |   **0.5789 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          60 |      6.154 ms |   0.6239 ms |   0.4127 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          60 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          60 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **62** |      **8.349 ms** |   **2.4830 ms** |   **1.6423 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          62 |      6.261 ms |   0.6672 ms |   0.4413 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          62 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          62 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **64** |      **7.408 ms** |   **1.0465 ms** |   **0.6922 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          64 |      6.379 ms |   0.7497 ms |   0.4959 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          64 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          64 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **66** |      **7.610 ms** |   **0.9118 ms** |   **0.6031 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          66 |      6.552 ms |   0.7488 ms |   0.4953 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          66 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          66 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **68** |      **8.116 ms** |   **2.6770 ms** |   **1.7707 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          68 |      6.752 ms |   0.9150 ms |   0.6052 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          68 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          68 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **70** |      **7.921 ms** |   **0.9339 ms** |   **0.6177 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          70 |      6.897 ms |   0.8309 ms |   0.5496 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          70 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          70 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **72** |      **8.023 ms** |   **0.9289 ms** |   **0.6144 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          72 |      7.704 ms |   1.0187 ms |   0.6738 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          72 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          72 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **74** |      **8.199 ms** |   **1.1039 ms** |   **0.7302 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          74 |      7.487 ms |   0.7406 ms |   0.4899 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          74 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          74 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **76** |      **8.383 ms** |   **0.8343 ms** |   **0.5518 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          76 |      7.759 ms |   0.5052 ms |   0.3342 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          76 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          76 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **78** |      **8.660 ms** |   **0.9444 ms** |   **0.6247 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          78 |      8.245 ms |   0.8591 ms |   0.5683 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          78 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          78 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **80** |      **8.766 ms** |   **0.9361 ms** |   **0.6192 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          80 |      8.124 ms |   0.4412 ms |   0.2918 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          80 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          80 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **82** |      **8.773 ms** |   **0.9070 ms** |   **0.5999 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          82 |      8.359 ms |   0.8404 ms |   0.5558 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          82 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          82 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **84** |      **9.119 ms** |   **1.0548 ms** |   **0.6977 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          84 |      8.950 ms |   1.4313 ms |   0.9467 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          84 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          84 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **86** |      **9.219 ms** |   **0.9662 ms** |   **0.6391 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          86 |      8.527 ms |   0.6715 ms |   0.4441 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          86 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          86 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **88** |      **9.453 ms** |   **0.9537 ms** |   **0.6308 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          88 |      8.650 ms |   0.6846 ms |   0.4528 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          88 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          88 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **90** |     **10.752 ms** |   **2.2125 ms** |   **1.4635 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          90 |      8.796 ms |   0.7810 ms |   0.5166 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          90 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          90 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **92** |      **9.698 ms** |   **0.9455 ms** |   **0.6254 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          92 |      9.037 ms |   0.8610 ms |   0.5695 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          92 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          92 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **94** |      **9.907 ms** |   **0.9645 ms** |   **0.6379 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          94 |      9.063 ms |   0.8630 ms |   0.5708 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          94 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          94 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |       **17** |          **96** |     **11.894 ms** |   **3.0802 ms** |   **2.0373 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |       17 |          96 |      9.238 ms |   0.9063 ms |   0.5995 ms |
| ParallelSave |  CsProjnet462 |        1048576 |       17 |          96 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |       17 |          96 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **1** |      **6.281 ms** |   **0.8988 ms** |   **0.5945 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           1 |      2.677 ms |   1.0344 ms |   0.6842 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           1 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           1 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **2** |      **3.730 ms** |   **1.0666 ms** |   **0.7055 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           2 |      1.604 ms |   0.8841 ms |   0.5848 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           2 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           2 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **4** |      **2.430 ms** |   **0.8664 ms** |   **0.5731 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           4 |      1.186 ms |   0.7690 ms |   0.5086 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           4 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           4 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **6** |      **2.091 ms** |   **0.8641 ms** |   **0.5715 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           6 |      1.180 ms |   0.7075 ms |   0.4680 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           6 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           6 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |           **8** |      **2.070 ms** |   **0.8544 ms** |   **0.5651 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |           8 |      1.264 ms |   0.5747 ms |   0.3801 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |           8 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |           8 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **10** |      **2.227 ms** |   **0.7846 ms** |   **0.5190 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          10 |      1.456 ms |   0.7115 ms |   0.4706 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          10 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          10 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **12** |      **2.296 ms** |   **1.0224 ms** |   **0.6763 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          12 |      1.567 ms |   0.5589 ms |   0.3697 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          12 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          12 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **14** |      **2.413 ms** |   **0.9430 ms** |   **0.6238 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          14 |      1.690 ms |   0.5343 ms |   0.3534 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          14 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          14 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **16** |      **2.466 ms** |   **0.8138 ms** |   **0.5383 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          16 |      1.926 ms |   0.5506 ms |   0.3642 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          16 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          16 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **18** |      **2.633 ms** |   **0.9010 ms** |   **0.5960 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          18 |      2.110 ms |   0.5499 ms |   0.3637 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          18 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          18 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **20** |      **2.764 ms** |   **0.8585 ms** |   **0.5679 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          20 |      2.248 ms |   0.5870 ms |   0.3883 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          20 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          20 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **22** |      **3.405 ms** |   **1.9337 ms** |   **1.2790 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          22 |      2.430 ms |   0.7182 ms |   0.4751 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          22 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          22 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **24** |      **3.217 ms** |   **0.8999 ms** |   **0.5952 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          24 |      2.554 ms |   0.5942 ms |   0.3930 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          24 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          24 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **26** |      **3.329 ms** |   **0.8534 ms** |   **0.5644 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          26 |      2.680 ms |   0.5549 ms |   0.3671 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          26 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          26 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **28** |      **3.445 ms** |   **0.8066 ms** |   **0.5335 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          28 |      2.867 ms |   0.5825 ms |   0.3853 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          28 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          28 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **30** |      **3.595 ms** |   **0.8090 ms** |   **0.5351 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          30 |      3.037 ms |   0.5481 ms |   0.3625 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          30 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          30 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **32** |      **3.755 ms** |   **0.8496 ms** |   **0.5620 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          32 |      3.202 ms |   0.6897 ms |   0.4562 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          32 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          32 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **34** |      **4.112 ms** |   **1.0498 ms** |   **0.6944 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          34 |      3.351 ms |   0.5602 ms |   0.3705 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          34 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          34 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **36** |      **4.275 ms** |   **0.7987 ms** |   **0.5283 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          36 |      3.564 ms |   0.7188 ms |   0.4755 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          36 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          36 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **38** |      **4.392 ms** |   **0.7937 ms** |   **0.5250 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          38 |      3.753 ms |   0.6222 ms |   0.4115 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          38 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          38 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **40** |      **4.677 ms** |   **0.7538 ms** |   **0.4986 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          40 |      3.992 ms |   0.5826 ms |   0.3854 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          40 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          40 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **42** |      **4.872 ms** |   **0.8344 ms** |   **0.5519 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          42 |      4.221 ms |   0.5353 ms |   0.3541 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          42 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          42 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **44** |      **5.050 ms** |   **0.8266 ms** |   **0.5467 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          44 |      4.543 ms |   0.5711 ms |   0.3778 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          44 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          44 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **46** |      **5.289 ms** |   **0.9532 ms** |   **0.6305 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          46 |      4.718 ms |   0.4243 ms |   0.2806 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          46 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          46 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **48** |      **5.303 ms** |   **0.7874 ms** |   **0.5208 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          48 |      4.901 ms |   0.4164 ms |   0.2754 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          48 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          48 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **50** |      **5.463 ms** |   **0.8278 ms** |   **0.5476 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          50 |      5.115 ms |   0.5486 ms |   0.3629 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          50 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          50 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **52** |      **5.656 ms** |   **0.8676 ms** |   **0.5738 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          52 |      5.342 ms |   0.4854 ms |   0.3210 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          52 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          52 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **54** |      **5.815 ms** |   **0.8377 ms** |   **0.5541 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          54 |      5.581 ms |   0.6039 ms |   0.3995 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          54 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          54 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **56** |      **5.986 ms** |   **0.8448 ms** |   **0.5588 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          56 |      5.941 ms |   0.7139 ms |   0.4722 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          56 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          56 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **58** |      **6.129 ms** |   **0.9245 ms** |   **0.6115 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          58 |      5.896 ms |   0.5241 ms |   0.3467 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          58 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          58 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **60** |      **6.239 ms** |   **0.8330 ms** |   **0.5510 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          60 |      6.070 ms |   0.5136 ms |   0.3397 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          60 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          60 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **62** |      **6.360 ms** |   **0.9949 ms** |   **0.6581 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          62 |      6.292 ms |   0.5874 ms |   0.3885 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          62 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          62 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **64** |      **6.530 ms** |   **0.8867 ms** |   **0.5865 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          64 |      6.493 ms |   0.5862 ms |   0.3877 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          64 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          64 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **66** |      **6.764 ms** |   **0.9016 ms** |   **0.5964 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          66 |      6.659 ms |   0.5236 ms |   0.3464 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          66 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          66 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **68** |      **7.142 ms** |   **1.0390 ms** |   **0.6872 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          68 |      6.878 ms |   0.5235 ms |   0.3463 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          68 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          68 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **70** |      **7.071 ms** |   **0.9254 ms** |   **0.6121 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          70 |      7.036 ms |   0.5865 ms |   0.3879 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          70 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          70 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **72** |      **7.241 ms** |   **0.8286 ms** |   **0.5481 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          72 |      7.230 ms |   0.5640 ms |   0.3730 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          72 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          72 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **74** |      **7.399 ms** |   **1.1457 ms** |   **0.7578 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          74 |      7.365 ms |   0.5868 ms |   0.3882 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          74 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          74 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **76** |      **7.573 ms** |   **0.8888 ms** |   **0.5879 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          76 |      7.606 ms |   0.5842 ms |   0.3864 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          76 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          76 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **78** |      **7.804 ms** |   **0.9067 ms** |   **0.5997 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          78 |      7.831 ms |   0.6301 ms |   0.4168 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          78 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          78 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **80** |      **8.109 ms** |   **0.8614 ms** |   **0.5698 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          80 |      8.027 ms |   0.8099 ms |   0.5357 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          80 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          80 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **82** |      **8.150 ms** |   **0.8824 ms** |   **0.5836 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          82 |      8.164 ms |   0.7411 ms |   0.4902 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          82 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          82 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **84** |      **8.509 ms** |   **1.0125 ms** |   **0.6697 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          84 |      8.331 ms |   0.6529 ms |   0.4319 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          84 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          84 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **86** |      **8.671 ms** |   **0.8362 ms** |   **0.5531 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          86 |      8.508 ms |   0.6091 ms |   0.4029 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          86 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          86 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **88** |      **8.928 ms** |   **0.8892 ms** |   **0.5882 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          88 |      8.719 ms |   0.7101 ms |   0.4697 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          88 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          88 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **90** |      **9.069 ms** |   **0.8410 ms** |   **0.5563 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          90 |      8.779 ms |   0.6361 ms |   0.4208 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          90 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          90 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **92** |      **9.337 ms** |   **1.0915 ms** |   **0.7220 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          92 |      9.030 ms |   0.5338 ms |   0.3530 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          92 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          92 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **94** |      **9.468 ms** |   **0.9424 ms** |   **0.6234 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          94 |      9.254 ms |   0.6563 ms |   0.4341 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          94 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          94 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |        **1048576** |      **134** |          **96** |      **9.831 ms** |   **0.9879 ms** |   **0.6534 ms** |
|  ParallelUse | .NET Core 2.0 |        1048576 |      134 |          96 |      9.339 ms |   0.6685 ms |   0.4422 ms |
| ParallelSave |  CsProjnet462 |        1048576 |      134 |          96 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |        1048576 |      134 |          96 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **1** | **15,212.151 ms** | **311.8493 ms** | **206.2690 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           1 | 10,499.914 ms | 177.1596 ms | 117.1801 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           1 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           1 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **2** |  **8,503.805 ms** | **344.3784 ms** | **227.7850 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           2 |  5,373.052 ms | 102.9143 ms |  68.0714 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           2 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           2 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **4** |  **4,617.400 ms** | **277.7068 ms** | **183.6859 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           4 |  2,772.016 ms |  38.5747 ms |  25.5147 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           4 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           4 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **6** |  **3,191.519 ms** | **114.6685 ms** |  **75.8461 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           6 |  1,867.866 ms |  48.6328 ms |  32.1676 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           6 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           6 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |           **8** |  **2,462.093 ms** | **113.4336 ms** |  **75.0293 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |           8 |  1,491.619 ms | 136.4067 ms |  90.2246 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |           8 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |           8 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **10** |  **2,016.534 ms** |  **81.0917 ms** |  **53.6371 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          10 |  1,178.345 ms |  38.8774 ms |  25.7150 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          10 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          10 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **12** |  **1,704.891 ms** |  **42.7372 ms** |  **28.2680 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          12 |  1,029.428 ms |  43.8818 ms |  29.0251 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          12 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          12 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **14** |  **1,637.913 ms** |  **44.8489 ms** |  **29.6648 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          14 |  1,090.432 ms |  32.1698 ms |  21.2783 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          14 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          14 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **16** |  **1,515.800 ms** |  **24.1567 ms** |  **15.9782 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          16 |    974.975 ms |  15.8787 ms |  10.5028 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          16 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          16 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **18** |  **1,442.905 ms** |  **19.1073 ms** |  **12.6383 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          18 |    884.797 ms |  14.3172 ms |   9.4699 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          18 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          18 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **20** |  **1,356.655 ms** |  **12.4658 ms** |   **8.2453 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          20 |    821.836 ms |  43.1377 ms |  28.5329 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          20 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          20 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **22** |  **1,280.063 ms** |  **15.2900 ms** |  **10.1134 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          22 |    751.657 ms |  28.6104 ms |  18.9240 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          22 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          22 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **24** |  **1,229.366 ms** |  **18.5003 ms** |  **12.2368 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          24 |    769.229 ms |  85.1207 ms |  56.3021 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          24 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          24 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **26** |  **1,252.258 ms** |  **26.5116 ms** |  **17.5358 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          26 |  1,106.292 ms |  34.5684 ms |  22.8648 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          26 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          26 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **28** |  **1,274.535 ms** |  **29.0828 ms** |  **19.2365 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          28 |  1,027.573 ms |   9.7844 ms |   6.4718 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          28 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          28 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **30** |  **1,283.829 ms** |  **40.6476 ms** |  **26.8858 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          30 |    976.154 ms |  28.7825 ms |  19.0379 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          30 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          30 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **32** |  **1,288.686 ms** |  **55.7888 ms** |  **36.9008 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          32 |    908.942 ms |  32.1481 ms |  21.2640 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          32 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          32 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **34** |  **1,293.643 ms** |  **49.6420 ms** |  **32.8351 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          34 |    875.528 ms |  24.6339 ms |  16.2938 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          34 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          34 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **36** |  **1,294.215 ms** |  **58.7599 ms** |  **38.8661 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          36 |    857.580 ms |  37.0310 ms |  24.4937 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          36 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          36 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **38** |  **1,274.106 ms** |  **34.4466 ms** |  **22.7843 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          38 |    856.486 ms |  12.2903 ms |   8.1293 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          38 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          38 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **40** |  **1,283.395 ms** |  **44.5759 ms** |  **29.4842 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          40 |    819.381 ms |  12.2540 ms |   8.1053 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          40 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          40 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **42** |  **1,283.097 ms** |  **65.1632 ms** |  **43.1014 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          42 |    782.782 ms |  13.6508 ms |   9.0292 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          42 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          42 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **44** |  **1,303.848 ms** |  **78.4211 ms** |  **51.8707 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          44 |    764.160 ms |  26.9069 ms |  17.7973 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          44 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          44 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **46** |  **1,296.780 ms** |  **38.5978 ms** |  **25.5300 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          46 |    749.321 ms |  31.1180 ms |  20.5827 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          46 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          46 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **48** |  **1,274.669 ms** |  **33.2148 ms** |  **21.9696 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          48 |    783.089 ms |  66.9051 ms |  44.2536 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          48 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          48 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **50** |  **1,295.023 ms** |  **78.1788 ms** |  **51.7104 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          50 |    913.849 ms |  12.8174 ms |   8.4779 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          50 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          50 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **52** |  **1,293.691 ms** |  **76.3727 ms** |  **50.5158 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          52 |    877.238 ms |   4.6396 ms |   3.0688 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          52 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          52 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **54** |  **1,276.985 ms** |  **43.4776 ms** |  **28.7577 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          54 |    857.318 ms |  15.0319 ms |   9.9427 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          54 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          54 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **56** |  **1,299.763 ms** |  **63.6511 ms** |  **42.1013 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          56 |    834.107 ms |  16.3681 ms |  10.8265 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          56 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          56 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **58** |  **1,309.183 ms** |  **77.1303 ms** |  **51.0169 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          58 |    803.563 ms |  11.5589 ms |   7.6455 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          58 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          58 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **60** |  **1,305.170 ms** |  **67.9140 ms** |  **44.9209 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          60 |    792.883 ms |  24.1779 ms |  15.9922 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          60 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          60 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **62** |  **1,295.855 ms** |  **56.6451 ms** |  **37.4672 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          62 |    793.905 ms |   9.0669 ms |   5.9972 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          62 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          62 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **64** |  **1,314.668 ms** | **103.3202 ms** |  **68.3399 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          64 |    780.594 ms |   6.1878 ms |   4.0928 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          64 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          64 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **66** |  **1,288.829 ms** |  **54.5537 ms** |  **36.0839 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          66 |    742.603 ms |   5.3720 ms |   3.5533 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          66 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          66 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **68** |  **1,299.411 ms** |  **85.5835 ms** |  **56.6082 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          68 |            NA |          NA |          NA |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          68 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          68 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **70** |  **1,315.625 ms** |  **92.9088 ms** |  **61.4534 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          70 |    728.584 ms |  20.5575 ms |  13.5975 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          70 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          70 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **72** |  **1,304.417 ms** |  **89.2164 ms** |  **59.0111 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          72 |    789.321 ms |  80.6489 ms |  53.3443 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          72 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          72 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **74** |  **1,303.658 ms** | **106.0452 ms** |  **70.1424 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          74 |    836.939 ms |   7.3708 ms |   4.8753 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          74 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          74 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **76** |            **NA** |          **NA** |          **NA** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          76 |    822.972 ms |  12.0505 ms |   7.9707 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          76 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          76 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **78** |  **1,302.443 ms** | **111.5781 ms** |  **73.8020 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          78 |    830.755 ms |  73.2344 ms |  48.4400 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          78 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          78 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **80** |  **1,300.333 ms** |  **74.0458 ms** |  **48.9767 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          80 |    788.316 ms |   7.0495 ms |   4.6628 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          80 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          80 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **82** |  **1,314.410 ms** |  **95.7549 ms** |  **63.3360 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          82 |    788.580 ms |  18.5335 ms |  12.2588 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          82 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          82 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **84** |  **1,293.824 ms** |  **71.0816 ms** |  **47.0161 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          84 |    785.195 ms |  17.4081 ms |  11.5144 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          84 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          84 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **86** |  **1,335.188 ms** | **138.1895 ms** |  **91.4038 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          86 |    772.445 ms |  16.9292 ms |  11.1976 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          86 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          86 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **88** |  **1,284.146 ms** |  **76.4276 ms** |  **50.5521 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          88 |            NA |          NA |          NA |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          88 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          88 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **90** |  **1,297.609 ms** |  **90.8226 ms** |  **60.0735 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          90 |            NA |          NA |          NA |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          90 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          90 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **92** |  **1,277.295 ms** |  **61.3611 ms** |  **40.5866 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          92 |    739.608 ms |  10.0235 ms |   6.6299 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          92 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          92 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **94** |  **1,313.334 ms** |  **96.9223 ms** |  **64.1081 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          94 |    724.962 ms |  12.2854 ms |   8.1260 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          94 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          94 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |       **17** |          **96** |  **1,314.961 ms** | **118.5429 ms** |  **78.4088 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |       17 |          96 |    778.790 ms |  63.0979 ms |  41.7353 ms |
| ParallelSave |  CsProjnet462 |      536870912 |       17 |          96 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |       17 |          96 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **1** |  **2,485.648 ms** |  **46.8205 ms** |  **30.9689 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           1 |  1,314.903 ms |  24.5863 ms |  16.2623 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           1 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           1 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **2** |  **1,372.652 ms** |  **39.4755 ms** |  **26.1106 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           2 |    670.675 ms |   5.1459 ms |   3.4037 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           2 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           2 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **4** |    **723.565 ms** |  **23.0957 ms** |  **15.2764 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           4 |    340.410 ms |   7.7399 ms |   5.1195 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           4 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           4 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **6** |    **499.787 ms** |   **7.2248 ms** |   **4.7788 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           6 |    230.588 ms |   6.1720 ms |   4.0824 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           6 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           6 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |           **8** |    **389.650 ms** |  **10.0128 ms** |   **6.6228 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |           8 |    175.480 ms |   2.2757 ms |   1.5053 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |           8 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |           8 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **10** |    **323.456 ms** |   **7.5440 ms** |   **4.9899 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          10 |    141.178 ms |   2.5858 ms |   1.7104 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          10 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          10 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **12** |    **290.424 ms** |  **17.0996 ms** |  **11.3103 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          12 |    133.712 ms |  22.1579 ms |  14.6561 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          12 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          12 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **14** |    **294.245 ms** |   **5.6823 ms** |   **3.7585 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          14 |    135.407 ms |   2.6723 ms |   1.7676 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          14 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          14 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **16** |    **277.960 ms** |   **3.3858 ms** |   **2.2395 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          16 |    118.538 ms |   4.0830 ms |   2.7007 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          16 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          16 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **18** |    **270.907 ms** |   **8.7843 ms** |   **5.8102 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          18 |    107.717 ms |   3.1484 ms |   2.0825 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          18 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          18 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **20** |    **269.751 ms** |   **9.6114 ms** |   **6.3574 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          20 |     99.497 ms |   3.9812 ms |   2.6333 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          20 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          20 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **22** |    **289.211 ms** |  **15.5142 ms** |  **10.2617 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          22 |     91.937 ms |   2.4140 ms |   1.5967 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          22 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          22 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **24** |    **294.950 ms** |  **14.6338 ms** |   **9.6794 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          24 |     83.648 ms |   1.6170 ms |   1.0695 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          24 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          24 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **26** |    **337.122 ms** |  **24.8316 ms** |  **16.4245 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          26 |    127.552 ms |   3.8343 ms |   2.5361 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          26 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          26 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **28** |    **332.682 ms** |  **17.5279 ms** |  **11.5936 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          28 |    117.693 ms |   3.1235 ms |   2.0660 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          28 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          28 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **30** |    **330.876 ms** |  **14.3872 ms** |   **9.5162 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          30 |    115.321 ms |   4.8550 ms |   3.2113 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          30 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          30 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **32** |    **310.623 ms** |  **12.9617 ms** |   **8.5734 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          32 |    111.454 ms |   9.3204 ms |   6.1648 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          32 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          32 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **34** |    **304.011 ms** |  **12.8767 ms** |   **8.5171 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          34 |    109.482 ms |   4.2970 ms |   2.8422 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          34 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          34 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **36** |    **304.307 ms** |  **12.6667 ms** |   **8.3782 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          36 |    103.789 ms |   2.5534 ms |   1.6889 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          36 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          36 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **38** |    **304.050 ms** |  **12.4745 ms** |   **8.2511 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          38 |    102.344 ms |   2.3184 ms |   1.5335 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          38 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          38 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **40** |    **292.710 ms** |  **12.6873 ms** |   **8.3918 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          40 |     97.282 ms |   1.8273 ms |   1.2086 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          40 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          40 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **42** |    **295.387 ms** |  **14.8534 ms** |   **9.8246 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          42 |     94.929 ms |   1.3545 ms |   0.8959 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          42 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          42 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **44** |    **294.976 ms** |  **14.7948 ms** |   **9.7858 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          44 |     90.158 ms |   1.8743 ms |   1.2397 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          44 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          44 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **46** |    **290.172 ms** |  **14.4498 ms** |   **9.5576 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          46 |     88.562 ms |   4.6883 ms |   3.1011 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          46 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          46 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **48** |    **306.345 ms** |  **18.3390 ms** |  **12.1301 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          48 |     92.265 ms |  12.7483 ms |   8.4322 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          48 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          48 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **50** |    **304.470 ms** |  **14.5004 ms** |   **9.5911 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          50 |    110.883 ms |   3.5438 ms |   2.3440 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          50 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          50 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **52** |    **302.177 ms** |   **9.8995 ms** |   **6.5479 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          52 |    105.544 ms |   3.7246 ms |   2.4636 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          52 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          52 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **54** |    **299.100 ms** |  **13.3002 ms** |   **8.7973 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          54 |    103.203 ms |   4.3087 ms |   2.8499 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          54 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          54 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **56** |    **297.302 ms** |  **14.5176 ms** |   **9.6025 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          56 |    104.076 ms |   3.3456 ms |   2.2129 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          56 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          56 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **58** |    **304.233 ms** |  **18.2178 ms** |  **12.0499 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          58 |    100.912 ms |   1.2109 ms |   0.8010 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          58 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          58 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **60** |    **310.670 ms** |  **16.5474 ms** |  **10.9451 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          60 |     99.743 ms |   0.9916 ms |   0.6559 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          60 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          60 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **62** |    **305.677 ms** |  **16.1153 ms** |  **10.6593 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          62 |     97.733 ms |   1.0627 ms |   0.7029 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          62 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          62 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **64** |    **307.017 ms** |  **12.6351 ms** |   **8.3573 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          64 |     94.158 ms |   0.9367 ms |   0.6196 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          64 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          64 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **66** |    **301.049 ms** |  **17.1369 ms** |  **11.3350 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          66 |     91.637 ms |   0.9247 ms |   0.6116 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          66 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          66 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **68** |    **299.839 ms** |  **13.1748 ms** |   **8.7143 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          68 |     90.432 ms |   2.2055 ms |   1.4588 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          68 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          68 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **70** |    **299.991 ms** |  **13.1318 ms** |   **8.6859 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          70 |     89.649 ms |   1.6484 ms |   1.0903 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          70 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          70 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **72** |    **307.769 ms** |  **15.0363 ms** |   **9.9456 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          72 |     93.258 ms |   8.6924 ms |   5.7495 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          72 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          72 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **74** |    **316.260 ms** |  **19.0072 ms** |  **12.5721 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          74 |    102.445 ms |   3.5722 ms |   2.3628 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          74 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          74 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **76** |    **314.475 ms** |  **18.1217 ms** |  **11.9864 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          76 |    100.733 ms |   1.8137 ms |   1.1996 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          76 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          76 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **78** |    **312.203 ms** |  **13.2532 ms** |   **8.7662 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          78 |     99.726 ms |   4.2134 ms |   2.7869 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          78 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          78 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **80** |    **299.250 ms** |  **15.9721 ms** |  **10.5646 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          80 |     98.554 ms |   3.2040 ms |   2.1193 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          80 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          80 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **82** |    **304.194 ms** |  **13.6109 ms** |   **9.0028 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          82 |     96.993 ms |   1.3628 ms |   0.9014 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          82 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          82 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **84** |    **303.794 ms** |  **16.6531 ms** |  **11.0150 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          84 |     95.611 ms |   1.7778 ms |   1.1759 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          84 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          84 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **86** |    **312.009 ms** |  **15.5230 ms** |  **10.2675 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          86 |     94.573 ms |   1.5487 ms |   1.0244 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          86 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          86 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **88** |    **300.986 ms** |  **16.3467 ms** |  **10.8123 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          88 |     92.127 ms |   1.4551 ms |   0.9625 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          88 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          88 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **90** |    **315.218 ms** |  **15.9402 ms** |  **10.5435 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          90 |     91.865 ms |   0.9134 ms |   0.6042 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          90 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          90 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **92** |    **302.791 ms** |  **14.1067 ms** |   **9.3307 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          92 |     90.679 ms |   3.0324 ms |   2.0057 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          92 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          92 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **94** |    **307.871 ms** |  **15.7361 ms** |  **10.4085 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          94 |     90.897 ms |   2.1447 ms |   1.4186 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          94 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          94 |            NA |          NA |          NA |
| **ParallelSave** | **.NET Core 2.0** |      **536870912** |      **134** |          **96** |    **335.359 ms** |  **89.5841 ms** |  **59.2543 ms** |
|  ParallelUse | .NET Core 2.0 |      536870912 |      134 |          96 |     94.329 ms |   5.5123 ms |   3.6461 ms |
| ParallelSave |  CsProjnet462 |      536870912 |      134 |          96 |            NA |          NA |          NA |
|  ParallelUse |  CsProjnet462 |      536870912 |      134 |          96 |            NA |          NA |          NA |

Benchmarks with issues:
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-JFKWFM(Server=True, Toolchain=.NET Core 2.0, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-JFKWFM(Server=True, Toolchain=.NET Core 2.0, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-JFKWFM(Server=True, Toolchain=.NET Core 2.0, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-JFKWFM(Server=True, Toolchain=.NET Core 2.0, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalCellCount=536870912, CellSize=134, ThreadCount=96]
