# Sequential performance benchmarking

See `MemStoreTest1`. This group evaluates the performance of primitive/TSL-enhanced local memory storage operations, on a single thread.
Four runs are conducted.

- `records_v0`: The original code from the `multi_cell_lock` branch.
- `records_v1`: Optimize `MTHash::_Lookup_LockEntry` family. See `6bb43521d713d1de3075a2477b192a8f04f0666e`. The hashtable lookup algorithm is adjusted so that the hot path only contains logic for a successful lookup run.
- `records_v2_x`: Optimize `Trinity.TSL Cell codegen`. See `47704207d04143f01758fc35793e9faf58921745`.
- `records_v2_tls`: Stock `[ThreadStatic]` object cache.
- `records_v2_no_pool`: No pooling. Works well for small accessors but terrible for more complex items.
- `records_v3`: Use Microsoft.Extension.ObjectPool for caching accessors. See `c1ea0cc15d4c989350b35b8c387ff98e1a201f4f`.

## MicroTests - NoFill

Micro tests run on empty local storage.
Testbed:
```
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i7-4770 CPU 3.40GHz (Haswell), 1 CPU, 8 logical cores and 4 physical cores
Frequency=3312642 Hz, Resolution=301.8739 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-UVGHVT : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Job-ULUNQO : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0

Server=True  RunStrategy=Throughput  
```


| Method         | Toolchain     | v0        | v1        | v2_nopool | v2_tls    |
|----------------|---------------|-----------|-----------|-----------|-----------|
| Use            | .NET Core 2.0 | 121.94 ns | 115.00 ns | 91.49 ns  | 111.36 ns |
| UseManipulate  | .NET Core 2.0 | 121.89 ns | 118.52 ns | 94.72 ns  | 118.36 ns |
| UseC2          | .NET Core 2.0 | -         | -         | 345.79 ns | 110.24 ns |
| UseManipulate2 | .NET Core 2.0 | -         | -         | 458.48 ns | 229.50 ns |
| LoadMyCell     | .NET Core 2.0 | 128.43 ns | 118.77 ns | 93.49 ns  | 121.52 ns |
| SaveMyCell     | .NET Core 2.0 | 86.77 ns  | 86.18 ns  | 87.78 ns  | 88.80 ns  |
| LoadCellBinary | .NET Core 2.0 | 75.63 ns  | 80.07 ns  | 79.52 ns  | 77.62 ns  |
| Contains       | .NET Core 2.0 | 24.00 ns  | 24.47 ns  | 24.11 ns  | 24.09 ns  |
| GetCellType    | .NET Core 2.0 | 39.91 ns  | 40.59 ns  | 40.04 ns  | 39.94 ns  |
| Use            | CsProjnet47   | 97.07 ns  | 90.43 ns  | 70.88 ns  | 87.62 ns  |
| UseManipulate  | CsProjnet47   | 102.43 ns | 87.38 ns  | 72.14 ns  | 90.54 ns  |
| UseC2          | CsProjnet47   | -         | -         | 273.70 ns | 82.83 ns  |
| UseManipulate2 | CsProjnet47   | -         | -         | 394.87 ns | 184.68 ns |
| LoadMyCell     | CsProjnet47   | 101.40 ns | 89.07 ns  | 74.11 ns  | 91.73 ns  |
| SaveMyCell     | CsProjnet47   | 85.68 ns  | 88.49 ns  | 90.34 ns  | 85.95 ns  |
| LoadCellBinary | CsProjnet47   | 62.91 ns  | 64.28 ns  | 64.50 ns  | 62.55 ns  |
| Contains       | CsProjnet47   | 29.66 ns  | 29.52 ns  | 29.36 ns  | 29.33 ns  |
| GetCellType    | CsProjnet47   | 44.13 ns  | 45.04 ns  | 43.26 ns  | 43.29 ns  |

