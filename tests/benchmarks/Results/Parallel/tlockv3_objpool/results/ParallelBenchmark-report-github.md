``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Xeon CPU E5-2690 v3 2.60GHz, 1 CPU, 24 logical cores and 12 physical cores
Frequency=2539061 Hz, Resolution=393.8464 ns, Timer=TSC
  [Host]     : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0
  Job-JFKWFM : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Job-KNTBNH : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0

Server=True  RunStrategy=Monitoring  

```
|       Method |     Toolchain | TotalSize | CellSize | ThreadCount |            Mean |        Error |       StdDev |
|------------- |-------------- |---------- |--------- |------------ |----------------:|-------------:|-------------:|
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |           **1** |     **27,803.7 us** |     **968.1 us** |     **640.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |           1 |     14,143.1 us |   2,601.6 us |   1,720.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |           **2** |     **16,107.7 us** |   **1,014.1 us** |     **670.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |           2 |      7,547.4 us |   1,758.1 us |   1,162.9 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |           **4** |      **8,903.3 us** |   **1,008.2 us** |     **666.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |           4 |      5,059.0 us |   2,121.7 us |   1,403.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |           **6** |      **6,471.6 us** |     **816.8 us** |     **540.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |           6 |      3,410.6 us |   1,542.7 us |   1,020.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |           **8** |      **5,247.5 us** |     **826.1 us** |     **546.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |           8 |      2,606.7 us |     864.0 us |     571.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **10** |      **4,952.9 us** |     **903.7 us** |     **597.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          10 |      2,364.8 us |     893.5 us |     591.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **12** |      **5,290.7 us** |   **1,159.8 us** |     **767.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          12 |      2,588.4 us |     753.4 us |     498.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **14** |      **4,652.6 us** |     **783.9 us** |     **518.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          14 |      2,493.5 us |     838.1 us |     554.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **16** |      **5,101.4 us** |   **1,598.2 us** |   **1,057.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          16 |      2,376.7 us |     714.6 us |     472.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **18** |      **4,693.9 us** |   **1,375.3 us** |     **909.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          18 |      2,398.2 us |     660.2 us |     436.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **20** |      **4,137.6 us** |     **899.8 us** |     **595.2 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          20 |      2,473.4 us |     803.5 us |     531.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **22** |      **4,093.8 us** |     **805.2 us** |     **532.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          22 |      2,695.8 us |   1,075.5 us |     711.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **24** |      **4,424.0 us** |   **1,108.9 us** |     **733.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          24 |      2,604.9 us |     738.6 us |     488.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **26** |      **4,449.2 us** |     **950.5 us** |     **628.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          26 |      3,458.4 us |   1,153.9 us |     763.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **28** |      **4,473.4 us** |     **847.3 us** |     **560.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          28 |      3,671.3 us |   2,752.8 us |   1,820.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **30** |      **4,511.9 us** |     **872.4 us** |     **577.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          30 |      2,888.8 us |     690.0 us |     456.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **32** |      **4,474.8 us** |     **846.0 us** |     **559.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          32 |      2,991.4 us |     683.0 us |     451.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **34** |      **5,621.9 us** |   **1,691.1 us** |   **1,118.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          34 |      3,096.5 us |     777.3 us |     514.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **36** |      **4,805.5 us** |     **950.5 us** |     **628.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          36 |      3,221.4 us |     681.2 us |     450.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **38** |      **5,709.9 us** |   **2,095.4 us** |   **1,386.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          38 |      3,326.0 us |     658.2 us |     435.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **40** |      **6,152.9 us** |   **2,852.2 us** |   **1,886.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          40 |      3,463.7 us |     769.1 us |     508.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **42** |      **4,979.2 us** |     **949.2 us** |     **627.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          42 |      3,567.8 us |     743.6 us |     491.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **44** |      **4,975.7 us** |     **888.4 us** |     **587.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          44 |      3,702.5 us |     682.3 us |     451.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **46** |      **5,076.0 us** |     **954.5 us** |     **631.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          46 |      3,834.1 us |     709.6 us |     469.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **48** |      **5,097.6 us** |     **967.1 us** |     **639.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          48 |      4,827.0 us |   2,439.0 us |   1,613.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **50** |      **5,277.1 us** |     **984.7 us** |     **651.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          50 |      4,137.7 us |     721.3 us |     477.1 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **52** |      **5,342.8 us** |     **830.0 us** |     **549.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          52 |      4,165.1 us |     780.4 us |     516.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **54** |      **7,176.8 us** |   **3,648.8 us** |   **2,413.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          54 |      4,304.9 us |     599.5 us |     396.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **56** |      **5,461.2 us** |     **886.9 us** |     **586.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          56 |      4,561.2 us |     836.0 us |     553.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **58** |      **5,594.5 us** |     **918.9 us** |     **607.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          58 |      4,681.5 us |     672.1 us |     444.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **60** |      **7,142.7 us** |   **3,221.5 us** |   **2,130.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          60 |      4,854.7 us |     598.1 us |     395.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **62** |      **5,921.2 us** |   **1,062.1 us** |     **702.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          62 |      4,974.5 us |     668.6 us |     442.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **64** |      **5,997.5 us** |     **959.1 us** |     **634.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          64 |      5,394.9 us |   1,670.5 us |   1,105.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **66** |      **6,075.0 us** |     **920.5 us** |     **608.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          66 |      5,141.9 us |     637.5 us |     421.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **68** |      **6,176.5 us** |     **912.6 us** |     **603.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          68 |      5,194.0 us |     620.6 us |     410.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **70** |      **6,321.0 us** |     **908.3 us** |     **600.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          70 |      6,006.4 us |   2,532.2 us |   1,674.9 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **72** |      **6,412.2 us** |     **966.6 us** |     **639.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          72 |      5,396.5 us |     622.5 us |     411.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **74** |      **6,615.6 us** |     **997.3 us** |     **659.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          74 |      5,495.8 us |     684.7 us |     452.9 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **76** |      **8,232.3 us** |   **3,029.9 us** |   **2,004.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          76 |      5,767.8 us |     767.1 us |     507.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **78** |      **6,724.8 us** |     **917.9 us** |     **607.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          78 |      5,849.4 us |     735.6 us |     486.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **80** |      **6,935.0 us** |     **861.5 us** |     **569.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          80 |      6,132.3 us |     762.3 us |     504.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **82** |      **8,307.8 us** |   **3,099.7 us** |   **2,050.2 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          82 |      6,317.1 us |     738.3 us |     488.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **84** |     **21,751.5 us** |  **69,463.5 us** |  **45,945.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          84 |      6,499.1 us |     579.3 us |     383.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **86** |      **7,262.5 us** |     **984.9 us** |     **651.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          86 |      7,601.2 us |   2,398.6 us |   1,586.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **88** |      **7,391.0 us** |     **991.8 us** |     **656.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          88 |      6,755.0 us |     498.5 us |     329.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **90** |      **7,500.3 us** |     **961.5 us** |     **636.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          90 |      6,936.5 us |     639.7 us |     423.1 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **92** |      **7,609.8 us** |     **939.7 us** |     **621.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          92 |      7,513.5 us |   2,136.4 us |   1,413.1 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **94** |      **7,864.4 us** |     **938.8 us** |     **620.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          94 |      7,037.6 us |     522.0 us |     345.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |       **17** |          **96** |      **7,835.1 us** |     **906.9 us** |     **599.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |       17 |          96 |      7,221.6 us |     652.2 us |     431.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |       17 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |       17 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |           **1** |      **6,724.3 us** |   **1,198.0 us** |     **792.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |           1 |      2,228.8 us |     979.1 us |     647.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |           **2** |      **3,507.1 us** |     **872.0 us** |     **576.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |           2 |      1,335.1 us |     846.1 us |     559.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |           **4** |      **2,289.1 us** |     **845.1 us** |     **559.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |           4 |      1,018.9 us |     799.1 us |     528.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |           **6** |      **2,406.6 us** |   **1,136.6 us** |     **751.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |           6 |        957.5 us |     685.6 us |     453.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |           **8** |      **1,820.9 us** |     **832.1 us** |     **550.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |           8 |      1,060.1 us |     664.2 us |     439.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **10** |      **1,840.4 us** |     **783.6 us** |     **518.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          10 |      1,168.9 us |     551.0 us |     364.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **12** |      **1,993.3 us** |     **738.2 us** |     **488.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          12 |      1,247.3 us |     516.4 us |     341.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **14** |      **2,021.7 us** |     **854.4 us** |     **565.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          14 |      1,371.7 us |     650.8 us |     430.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **16** |      **2,123.4 us** |     **827.5 us** |     **547.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          16 |      1,481.9 us |     545.4 us |     360.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **18** |      **2,246.6 us** |     **864.9 us** |     **572.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          18 |      1,683.1 us |     553.0 us |     365.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **20** |      **2,397.6 us** |     **811.3 us** |     **536.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          20 |      1,803.1 us |     544.6 us |     360.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **22** |      **2,468.2 us** |     **846.2 us** |     **559.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          22 |      1,951.2 us |     668.2 us |     442.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **24** |      **2,599.4 us** |     **876.3 us** |     **579.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          24 |      2,022.8 us |     581.5 us |     384.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **26** |      **2,785.4 us** |     **812.5 us** |     **537.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          26 |      2,136.0 us |     649.5 us |     429.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **28** |      **2,825.1 us** |     **793.7 us** |     **525.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          28 |      2,257.4 us |     647.1 us |     428.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **30** |      **3,057.9 us** |     **933.4 us** |     **617.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          30 |      2,347.9 us |     563.4 us |     372.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **32** |      **3,163.3 us** |     **921.4 us** |     **609.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          32 |      2,481.6 us |     598.7 us |     396.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **34** |      **3,247.5 us** |     **839.9 us** |     **555.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          34 |      2,611.6 us |     617.1 us |     408.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **36** |      **3,392.4 us** |     **830.4 us** |     **549.2 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          36 |      2,788.1 us |     593.1 us |     392.3 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **38** |      **3,507.4 us** |     **872.0 us** |     **576.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          38 |      2,898.9 us |     584.2 us |     386.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **40** |      **3,663.1 us** |     **832.0 us** |     **550.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          40 |      3,011.9 us |     566.5 us |     374.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **42** |      **3,747.7 us** |     **847.2 us** |     **560.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          42 |      3,231.1 us |     558.6 us |     369.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **44** |      **4,004.2 us** |     **808.1 us** |     **534.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          44 |      3,418.9 us |     483.5 us |     319.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **46** |      **4,162.4 us** |     **912.7 us** |     **603.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          46 |      3,570.8 us |     490.1 us |     324.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **48** |      **4,329.8 us** |     **844.3 us** |     **558.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          48 |      3,780.8 us |     493.2 us |     326.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **50** |      **4,518.2 us** |     **855.0 us** |     **565.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          50 |      3,897.6 us |     531.6 us |     351.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **52** |      **4,660.2 us** |     **830.8 us** |     **549.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          52 |      4,094.2 us |     608.7 us |     402.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **54** |      **4,764.9 us** |     **871.5 us** |     **576.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          54 |      4,196.9 us |     688.9 us |     455.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **56** |      **4,903.7 us** |     **841.1 us** |     **556.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          56 |      4,345.7 us |     594.4 us |     393.1 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **58** |      **5,020.0 us** |     **859.8 us** |     **568.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          58 |      4,526.1 us |     544.6 us |     360.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **60** |      **5,120.5 us** |     **901.2 us** |     **596.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          60 |      4,684.3 us |     699.1 us |     462.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **62** |      **5,217.4 us** |     **878.0 us** |     **580.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          62 |      4,789.8 us |     554.5 us |     366.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **64** |      **5,335.0 us** |     **898.9 us** |     **594.6 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          64 |      4,872.1 us |     548.8 us |     363.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **66** |      **5,513.2 us** |   **1,013.8 us** |     **670.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          66 |      5,083.3 us |     579.7 us |     383.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **68** |      **5,632.1 us** |     **989.0 us** |     **654.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          68 |      5,164.4 us |     558.5 us |     369.4 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **70** |      **5,649.6 us** |     **959.9 us** |     **634.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          70 |      5,360.3 us |     744.1 us |     492.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **72** |      **5,788.4 us** |     **903.2 us** |     **597.4 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          72 |      5,507.0 us |     666.8 us |     441.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **74** |      **5,904.3 us** |     **948.7 us** |     **627.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          74 |      5,685.4 us |     640.5 us |     423.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **76** |      **6,061.6 us** |   **1,003.1 us** |     **663.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          76 |      5,809.0 us |     672.3 us |     444.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **78** |      **6,134.3 us** |     **947.5 us** |     **626.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          78 |      5,952.6 us |     633.7 us |     419.2 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **80** |      **6,198.6 us** |     **940.2 us** |     **621.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          80 |      6,086.8 us |     651.8 us |     431.1 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **82** |      **6,373.7 us** |   **1,108.2 us** |     **733.0 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          82 |      6,276.3 us |     706.8 us |     467.5 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **84** |      **6,532.6 us** |   **1,005.6 us** |     **665.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          84 |      6,328.9 us |     568.3 us |     375.9 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **86** |      **6,741.2 us** |   **1,037.0 us** |     **685.9 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          86 |      6,492.6 us |     652.8 us |     431.8 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **88** |      **6,833.8 us** |     **889.5 us** |     **588.3 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          88 |      6,632.2 us |     674.3 us |     446.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **90** |      **6,863.8 us** |     **931.4 us** |     **616.1 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          90 |      6,764.8 us |     749.5 us |     495.7 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **92** |      **7,124.7 us** |     **970.1 us** |     **641.7 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          92 |      6,900.7 us |     629.0 us |     416.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **94** |      **7,226.3 us** |     **935.6 us** |     **618.8 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          94 |      7,018.8 us |     575.4 us |     380.6 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** |   **1048576** |      **134** |          **96** |      **7,303.8 us** |   **1,010.7 us** |     **668.5 us** |
|  ParallelUse | .NET Core 2.0 |   1048576 |      134 |          96 |      7,153.3 us |     573.0 us |     379.0 us |
| ParallelSave |  CsProjnet462 |   1048576 |      134 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 |   1048576 |      134 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |           **1** | **13,294,357.4 us** | **203,019.9 us** | **134,285.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |           1 |  8,909,102.9 us | 198,201.1 us | 131,097.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |           **2** |  **7,601,662.3 us** | **428,230.7 us** | **283,248.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |           2 |  4,556,151.5 us |  62,970.1 us |  41,650.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |           **4** |  **4,053,513.1 us** | **106,465.1 us** |  **70,420.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |           4 |  2,411,426.7 us | 104,788.8 us |  69,311.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |           **6** |  **2,807,386.7 us** |  **60,898.9 us** |  **40,280.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |           6 |  1,646,245.6 us |  90,760.4 us |  60,032.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |           **8** |  **2,259,154.9 us** |  **51,161.9 us** |  **33,840.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |           8 |  1,244,303.6 us |  24,370.1 us |  16,119.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **10** |  **1,817,528.6 us** |  **67,081.5 us** |  **44,370.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          10 |  1,022,225.5 us |  72,760.0 us |  48,126.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **12** |  **1,518,529.3 us** |  **52,705.0 us** |  **34,861.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          12 |    899,737.3 us |  53,705.1 us |  35,522.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **14** |  **1,447,748.6 us** |  **45,390.6 us** |  **30,023.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          14 |    978,715.0 us | 227,830.3 us | 150,695.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **16** |  **1,388,285.3 us** |  **18,818.6 us** |  **12,447.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          16 |    838,072.5 us |  28,607.5 us |  18,922.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **18** |  **1,316,199.5 us** |  **16,117.2 us** |  **10,660.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          18 |    767,921.4 us |  26,092.3 us |  17,258.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **20** |  **1,232,083.0 us** |  **16,233.6 us** |  **10,737.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          20 |    702,284.4 us |  28,939.0 us |  19,141.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **22** |  **1,159,877.7 us** |  **11,030.7 us** |   **7,296.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          22 |    637,678.0 us |  11,918.6 us |   7,883.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **24** |  **1,106,070.2 us** |  **16,272.3 us** |  **10,763.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          24 |    661,206.9 us |  78,099.1 us |  51,657.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **26** |  **1,156,282.1 us** |  **34,410.5 us** |  **22,760.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          26 |    931,794.2 us |  11,245.4 us |   7,438.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **28** |  **1,171,622.2 us** |  **35,982.0 us** |  **23,799.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          28 |    886,953.2 us |  22,901.2 us |  15,147.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **30** |  **1,168,701.1 us** |  **33,267.0 us** |  **22,004.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          30 |    834,175.3 us |   8,151.9 us |   5,392.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **32** |  **1,211,144.5 us** |  **36,682.1 us** |  **24,262.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          32 |    783,232.2 us |   5,402.4 us |   3,573.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **34** |  **1,202,441.1 us** |  **63,145.8 us** |  **41,767.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          34 |    741,584.6 us |   4,180.9 us |   2,765.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **36** |  **1,186,952.4 us** |  **60,257.6 us** |  **39,856.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          36 |    738,797.8 us |  27,651.7 us |  18,289.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **38** |  **1,192,273.0 us** |  **51,550.5 us** |  **34,097.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          38 |    728,819.7 us |   7,546.4 us |   4,991.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **40** |  **1,194,920.1 us** |  **51,543.8 us** |  **34,093.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          40 |    696,917.4 us |  17,790.0 us |  11,767.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **42** |  **1,216,192.5 us** |  **74,331.7 us** |  **49,165.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          42 |    670,995.1 us |   9,246.9 us |   6,116.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **44** |  **1,208,657.9 us** |  **87,340.8 us** |  **57,770.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          44 |    645,957.8 us |   2,959.6 us |   1,957.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **46** |  **1,223,083.5 us** |  **60,931.8 us** |  **40,302.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          46 |    625,663.3 us |  11,423.8 us |   7,556.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **48** |  **1,252,727.8 us** |  **81,520.1 us** |  **53,920.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          48 |    659,341.2 us |  73,656.7 us |  48,719.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **50** |  **1,289,612.0 us** | **126,723.3 us** |  **83,819.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          50 |    775,458.6 us |   4,219.9 us |   2,791.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **52** |  **1,329,829.7 us** | **107,767.4 us** |  **71,281.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          52 |    752,128.4 us |   7,309.3 us |   4,834.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **54** |  **1,327,192.0 us** |  **71,162.1 us** |  **47,069.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          54 |    724,391.8 us |  10,920.3 us |   7,223.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **56** |  **1,346,958.4 us** |  **74,748.3 us** |  **49,441.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          56 |    709,408.7 us |  14,724.4 us |   9,739.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **58** |  **1,344,226.0 us** |  **85,842.1 us** |  **56,779.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          58 |    687,704.9 us |  14,570.5 us |   9,637.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **60** |  **1,358,564.4 us** |  **64,343.0 us** |  **42,558.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          60 |    680,256.0 us |  16,432.8 us |  10,869.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **62** |  **1,416,721.1 us** | **129,602.3 us** |  **85,723.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          62 |    676,479.1 us |   5,355.3 us |   3,542.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **64** |  **1,402,652.6 us** | **121,620.1 us** |  **80,444.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          64 |    655,017.3 us |   3,958.5 us |   2,618.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **66** |  **1,436,574.1 us** |  **84,990.9 us** |  **56,216.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          66 |    641,145.9 us |   4,135.3 us |   2,735.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **68** |  **1,417,989.7 us** | **111,295.4 us** |  **73,615.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          68 |    626,647.6 us |   5,080.7 us |   3,360.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **70** |  **1,463,958.9 us** | **105,485.5 us** |  **69,772.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          70 |    612,756.8 us |   3,550.5 us |   2,348.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **72** |  **1,491,355.5 us** | **140,622.3 us** |  **93,012.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          72 |    649,717.2 us |  92,950.9 us |  61,481.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **74** |  **1,546,997.2 us** | **183,011.0 us** | **121,050.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          74 |    719,994.1 us |  15,562.3 us |  10,293.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **76** |  **1,520,387.9 us** |  **92,449.7 us** |  **61,149.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          76 |    704,860.5 us |   9,171.3 us |   6,066.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **78** |  **1,528,852.2 us** |  **76,895.8 us** |  **50,861.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          78 |    689,805.2 us |  11,520.3 us |   7,620.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **80** |  **1,577,909.3 us** | **195,106.5 us** | **129,050.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          80 |    672,825.6 us |   4,812.8 us |   3,183.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **82** |  **1,530,900.4 us** | **145,027.7 us** |  **95,926.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          82 |    658,006.5 us |   3,068.5 us |   2,029.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **84** |  **1,578,980.5 us** | **149,699.3 us** |  **99,016.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          84 |    658,981.3 us |  12,451.9 us |   8,236.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **86** |  **1,537,134.8 us** | **130,692.7 us** |  **86,445.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          86 |    654,760.6 us |   8,967.5 us |   5,931.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **88** |  **1,551,413.2 us** | **173,427.9 us** | **114,711.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          88 |              NA |           NA |           NA |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **90** |  **1,550,195.9 us** | **108,292.0 us** |  **71,628.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          90 |    632,026.6 us |   7,241.4 us |   4,789.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **92** |  **1,620,418.5 us** | **183,950.3 us** | **121,671.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          92 |    623,469.6 us |   7,862.4 us |   5,200.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **94** |  **1,576,910.0 us** | **183,946.8 us** | **121,669.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          94 |    612,191.5 us |   3,570.2 us |   2,361.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |       **17** |          **96** |  **1,588,576.6 us** | **142,974.2 us** |  **94,568.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |       17 |          96 |    631,306.5 us |  43,424.9 us |  28,722.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |       17 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |       17 |          96 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |           **1** |  **2,189,271.2 us** |  **54,632.2 us** |  **36,135.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |           1 |  1,100,663.1 us |  23,638.3 us |  15,635.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |           1 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |           1 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |           **2** |  **1,215,433.5 us** |  **27,470.8 us** |  **18,170.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |           2 |    555,523.2 us |  17,404.9 us |  11,512.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |           2 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |           2 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |           **4** |    **659,499.7 us** |  **15,642.8 us** |  **10,346.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |           4 |    292,063.3 us |   8,862.6 us |   5,862.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |           4 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |           4 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |           **6** |    **452,337.3 us** |  **14,028.1 us** |   **9,278.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |           6 |    197,028.8 us |   3,858.8 us |   2,552.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |           6 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |           6 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |           **8** |    **358,479.5 us** |  **10,108.4 us** |   **6,686.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |           8 |    150,988.3 us |   2,302.2 us |   1,522.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |           8 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |           8 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **10** |    **300,038.0 us** |   **3,828.6 us** |   **2,532.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          10 |    123,605.1 us |   1,615.2 us |   1,068.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          10 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          10 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **12** |    **265,392.5 us** |  **12,503.6 us** |   **8,270.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          12 |    117,594.8 us |  16,995.3 us |  11,241.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          12 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          12 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **14** |    **264,156.2 us** |  **11,161.7 us** |   **7,382.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          14 |    111,542.8 us |   2,074.5 us |   1,372.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          14 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          14 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **16** |    **257,836.8 us** |   **6,108.8 us** |   **4,040.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          16 |    103,148.3 us |   2,718.4 us |   1,798.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          16 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          16 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **18** |    **249,136.2 us** |   **5,954.3 us** |   **3,938.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          18 |     92,134.1 us |   2,368.1 us |   1,566.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          18 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          18 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **20** |    **250,359.6 us** |   **9,153.4 us** |   **6,054.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          20 |     84,459.3 us |   1,969.3 us |   1,302.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          20 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          20 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **22** |    **257,924.2 us** |  **13,148.6 us** |   **8,697.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          22 |     77,289.1 us |   3,050.7 us |   2,017.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          22 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          22 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **24** |    **284,804.0 us** |  **14,047.4 us** |   **9,291.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          24 |     71,119.9 us |   1,970.2 us |   1,303.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          24 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          24 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **26** |    **304,981.2 us** |  **17,862.8 us** |  **11,815.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          26 |    110,564.5 us |   5,274.4 us |   3,488.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          26 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          26 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **28** |    **315,183.3 us** |  **17,404.5 us** |  **11,512.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          28 |    103,306.4 us |   2,790.5 us |   1,845.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          28 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          28 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **30** |    **312,891.9 us** |  **12,225.3 us** |   **8,086.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          30 |     99,671.9 us |   4,553.5 us |   3,011.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          30 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          30 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **32** |    **287,677.1 us** |  **12,077.3 us** |   **7,988.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          32 |     95,021.9 us |   7,334.7 us |   4,851.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          32 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          32 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **34** |    **287,760.8 us** |   **9,661.0 us** |   **6,390.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          34 |     91,916.7 us |   1,437.4 us |     950.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          34 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          34 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **36** |    **283,905.5 us** |  **13,135.9 us** |   **8,688.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          36 |     86,709.7 us |   2,056.8 us |   1,360.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          36 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          36 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **38** |    **283,945.2 us** |  **12,420.3 us** |   **8,215.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          38 |     85,373.9 us |     984.4 us |     651.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          38 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          38 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **40** |    **277,480.6 us** |  **10,772.4 us** |   **7,125.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          40 |     81,208.7 us |   1,959.4 us |   1,296.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          40 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          40 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **42** |    **278,558.1 us** |  **11,554.1 us** |   **7,642.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          42 |     78,696.1 us |     931.5 us |     616.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          42 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          42 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **44** |    **277,644.3 us** |  **11,911.8 us** |   **7,879.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          44 |     75,463.3 us |   1,681.1 us |   1,111.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          44 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          44 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **46** |    **282,398.7 us** |  **15,789.4 us** |  **10,443.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          46 |     73,520.4 us |   3,414.8 us |   2,258.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          46 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          46 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **48** |    **285,437.0 us** |  **13,767.1 us** |   **9,106.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          48 |     73,874.1 us |   3,658.2 us |   2,419.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          48 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          48 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **50** |    **296,182.3 us** |  **12,852.4 us** |   **8,501.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          50 |     92,373.6 us |   2,404.3 us |   1,590.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          50 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          50 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **52** |    **295,363.1 us** |  **13,004.8 us** |   **8,601.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          52 |     92,212.2 us |   4,560.9 us |   3,016.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          52 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          52 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **54** |    **289,792.4 us** |  **13,595.6 us** |   **8,992.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          54 |     87,099.6 us |   3,694.7 us |   2,443.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          54 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          54 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **56** |    **290,707.1 us** |  **11,858.0 us** |   **7,843.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          56 |     87,086.4 us |   3,816.5 us |   2,524.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          56 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          56 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **58** |    **292,702.8 us** |  **12,634.9 us** |   **8,357.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          58 |     84,807.2 us |   1,872.1 us |   1,238.3 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          58 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          58 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **60** |    **291,619.3 us** |  **13,907.8 us** |   **9,199.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          60 |     82,518.0 us |   1,495.5 us |     989.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          60 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          60 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **62** |    **294,047.4 us** |  **12,957.1 us** |   **8,570.3 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          62 |     81,366.7 us |     965.7 us |     638.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          62 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          62 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **64** |    **289,197.6 us** |  **10,095.3 us** |   **6,677.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          64 |     79,346.7 us |   1,218.2 us |     805.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          64 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          64 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **66** |    **294,719.4 us** |  **15,043.7 us** |   **9,950.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          66 |     76,429.4 us |   1,086.1 us |     718.4 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          66 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          66 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **68** |    **289,012.3 us** |  **15,422.1 us** |  **10,200.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          68 |     75,454.4 us |     992.8 us |     656.7 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          68 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          68 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **70** |    **292,962.6 us** |  **11,214.0 us** |   **7,417.4 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          70 |     75,873.1 us |   7,301.5 us |   4,829.5 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          70 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          70 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **72** |    **282,142.8 us** |  **11,958.9 us** |   **7,910.0 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          72 |     77,833.3 us |   3,536.1 us |   2,338.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          72 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          72 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **74** |    **295,241.2 us** |  **15,173.8 us** |  **10,036.5 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          74 |     86,209.5 us |   1,282.9 us |     848.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          74 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          74 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **76** |    **281,943.3 us** |   **9,560.7 us** |   **6,323.8 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          76 |     84,572.5 us |   2,793.6 us |   1,847.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          76 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          76 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **78** |    **295,688.1 us** |  **14,909.4 us** |   **9,861.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          78 |     82,940.0 us |   1,635.8 us |   1,082.0 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          78 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          78 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **80** |    **290,748.2 us** |  **13,172.9 us** |   **8,713.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          80 |     82,855.4 us |   2,714.7 us |   1,795.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          80 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          80 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **82** |    **299,193.3 us** |  **12,834.2 us** |   **8,489.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          82 |     82,123.9 us |   1,859.2 us |   1,229.8 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          82 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          82 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **84** |    **293,156.9 us** |  **11,777.7 us** |   **7,790.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          84 |     80,583.4 us |   1,234.0 us |     816.2 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          84 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          84 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **86** |    **298,822.3 us** |  **12,218.4 us** |   **8,081.7 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          86 |     79,426.5 us |   1,179.1 us |     779.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          86 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          86 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **88** |    **297,225.7 us** |  **14,896.6 us** |   **9,853.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          88 |     77,760.2 us |     837.8 us |     554.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          88 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          88 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **90** |    **293,679.9 us** |  **11,631.0 us** |   **7,693.2 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          90 |     76,681.1 us |   1,354.7 us |     896.1 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          90 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          90 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **92** |    **289,772.3 us** |  **12,803.8 us** |   **8,468.9 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          92 |     74,668.7 us |   1,109.6 us |     733.9 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          92 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          92 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **94** |    **300,641.3 us** |  **14,084.7 us** |   **9,316.1 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          94 |     78,495.0 us |   6,763.4 us |   4,473.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          94 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          94 |              NA |           NA |           NA |
| **ParallelSave** | **.NET Core 2.0** | **536870912** |      **134** |          **96** |    **297,708.9 us** |  **16,149.0 us** |  **10,681.6 us** |
|  ParallelUse | .NET Core 2.0 | 536870912 |      134 |          96 |     79,983.8 us |   2,238.4 us |   1,480.6 us |
| ParallelSave |  CsProjnet462 | 536870912 |      134 |          96 |              NA |           NA |           NA |
|  ParallelUse |  CsProjnet462 | 536870912 |      134 |          96 |              NA |           NA |           NA |

Benchmarks with issues:
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=1048576, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-JFKWFM(Server=True, Toolchain=.NET Core 2.0, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=17, ThreadCount=96]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=1]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=2]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=4]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=6]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=8]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=10]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=12]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=14]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=16]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=18]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=20]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=22]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=24]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=26]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=28]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=30]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=32]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=34]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=36]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=38]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=40]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=42]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=44]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=46]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=48]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=50]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=52]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=54]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=56]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=58]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=60]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=62]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=64]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=66]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=68]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=70]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=72]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=74]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=76]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=78]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=80]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=82]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=84]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=86]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=88]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=90]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=92]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=94]
  ParallelBenchmark.ParallelSave: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=96]
  ParallelBenchmark.ParallelUse: Job-KNTBNH(Server=True, Toolchain=CsProjnet462, RunStrategy=Monitoring) [TotalSize=536870912, CellSize=134, ThreadCount=96]
