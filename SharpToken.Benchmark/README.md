# SharpToken Benchmark

This runs a encode benchmark over all samples in GptEncoderTestSamples.txt

#### How To Run

Make a release build and run the `SharpToken.Benchmark.exe`
in `..\SharpToken\SharpToken.Benchmark\bin\Release\net8.0\SharpToken.Benchmark.exe`


## Results:


### feat(benchmark): don't make allocations in benchmark methods

- version: 1.2.17 + improvements - fix allocations in benchmark

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method      | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------ |--------------------- |--------------------- |-----------:|---------:|---------:|------:|---------:|-------:|----------:|------------:|
| Encode      | .NET 6.0             | .NET 6.0             |   836.5 us |  1.59 us |  1.24 us |  0.67 |  62.5000 | 0.9766 | 515.51 KB |        0.82 |
| Encode      | .NET 8.0             | .NET 8.0             |   553.8 us |  0.66 us |  0.55 us |  0.44 |   2.9297 |      - |  30.95 KB |        0.05 |
| Encode      | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,251.8 us | 20.60 us | 19.27 us |  1.00 | 101.5625 | 1.9531 | 628.84 KB |        1.00 |
|             |                      |                      |            |          |          |       |          |        |           |             |
| CountTokens | .NET 6.0             | .NET 6.0             |   781.3 us | 11.27 us | 10.54 us |  0.65 |  58.5938 | 0.9766 | 484.75 KB |       0.806 |
| CountTokens | .NET 8.0             | .NET 8.0             |   538.2 us |  1.24 us |  0.97 us |  0.45 |        - |      - |   4.06 KB |       0.007 |
| CountTokens | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,191.9 us |  9.85 us |  7.69 us |  1.00 |  97.6563 | 1.9531 | 601.37 KB |       1.000 |


## Results (This results mesured allocations produced in benchmark method)

All following benchmark results show allocations produced in benchmark method.
The benchmarks had allocations for `List.Add()`, `File.ReadAllLines()` and `IEnumerable<string>.GetEnumerator()`.

All Benchmarks above don't have this missleading allocations!
Therefore results can not be compared!


### feat(token-count): implement low allocation token count public method

- version: 1.2.17 + improvements + count tokens benchmark

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method      | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------ |--------------------- |--------------------- |-----------:|---------:|---------:|------:|---------:|-------:|----------:|------------:|
| Encode      | .NET 6.0             | .NET 6.0             | 1,113.1 us |  4.59 us |  3.59 us |  0.75 |  64.4531 | 5.8594 | 537.97 KB |        0.83 |
| Encode      | .NET 8.0             | .NET 8.0             |   649.5 us |  6.04 us |  5.65 us |  0.44 |   5.8594 |      - |  51.48 KB |        0.08 |
| Encode      | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,481.1 us | 13.32 us | 11.81 us |  1.00 | 105.4688 | 9.7656 | 652.02 KB |        1.00 |
|             |                      |                      |            |          |          |       |          |        |           |             |
| CountTokens | .NET 6.0             | .NET 6.0             | 1,038.1 us | 10.00 us |  9.36 us |  0.73 |  60.5469 | 1.9531 | 505.07 KB |        0.81 |
| CountTokens | .NET 8.0             | .NET 8.0             |   603.3 us |  1.81 us |  1.51 us |  0.42 |   1.9531 |      - |  22.46 KB |        0.04 |
| CountTokens | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,429.1 us | 19.10 us | 17.86 us |  1.00 |  99.6094 | 3.9063 |  622.4 KB |        1.00 |


### feat(performance): backport some optimizations to net6.0 and netstandard

- version: 1.2.17 + improvements optimizations to net6.0 and netstandard

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error   | StdDev  | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|--------:|--------:|------:|---------:|-------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             |   911.5 us | 4.22 us | 3.53 us |  0.69 |  65.4297 | 5.8594 | 537.97 KB |        0.83 |
| Encode | .NET 8.0             | .NET 8.0             |   623.2 us | 5.88 us | 5.50 us |  0.48 |   5.8594 |      - |  51.48 KB |        0.08 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,314.2 us | 3.73 us | 2.91 us |  1.00 | 105.4688 | 9.7656 | 652.02 KB |        1.00 |

**Notable Improvments**
- NET 6 and NET Framework got faster and require less memory
- Both also produce less garbage
- NET 8 also shows less allocations beacuse of allocation reduction for allowedSpecial and disallowedSpecial


### feat(performance): implement fast MultiBytePairEncoder with almost zero allocations

- version: 1.2.17 + improvements MultiBytePairEncoder in net8.0

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,150.4 us | 22.87 us | 36.27 us |  0.65 |    0.02 |  93.7500 |  7.8125 | 771.21 KB |        0.89 |
| Encode | .NET 8.0             | .NET 8.0             |   656.6 us | 12.76 us | 16.14 us |  0.38 |    0.01 |  11.7188 |       - | 100.23 KB |        0.12 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,754.3 us |  2.50 us |  2.09 us |  1.00 |    0.00 | 140.6250 | 11.7188 |  866.7 KB |        1.00 |

**Notable Improvments for .NET 8.0**
- 158 KB less allocations more then 2.5 times better!
- 83 us faster
- no Gen1 garbage produced

### feat(performance): use compile time generated regex in net8.0

- version: 1.2.17 + improvements for regex in net8.0

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,230.8 us | 24.22 us | 51.61 us |  0.67 |    0.03 |  99.6094 |  7.8125 | 826.18 KB |        0.90 |
| Encode | .NET 8.0             | .NET 8.0             |   739.7 us |  4.31 us |  4.03 us |  0.40 |    0.00 |  31.2500 |  1.9531 | 258.58 KB |        0.28 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,847.5 us | 17.26 us | 14.41 us |  1.00 |    0.00 | 148.4375 | 13.6719 | 921.82 KB |        1.00 |


### feat(performance): add support for ReadOnlySpan&lt;char> in net8.0

- version: 1.2.17 + improvements for net8.0

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|---------:|--------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,205.4 us | 22.61 us | 20.04 us |  0.64 |  99.6094 |  7.8125 | 826.18 KB |        0.90 |
| Encode | .NET 8.0             | .NET 8.0             |   754.3 us | 11.34 us | 10.05 us |  0.40 |  31.2500 |  1.9531 | 258.58 KB |        0.28 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,868.9 us | 23.18 us | 19.36 us |  1.00 | 148.4375 | 13.6719 | 921.81 KB |        1.00 |


### feat(performance): cache model parameters to do params preparation only once

- version: 1.2.17 + improvements

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|---------:|--------:|-----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,203.7 us | 13.06 us | 10.20 us |  0.63 | 105.4688 |  9.7656 |  876.44 KB |        0.84 |
| Encode | .NET 8.0             | .NET 8.0             |   975.9 us |  9.80 us |  9.16 us |  0.51 | 105.4688 |  7.8125 |  867.94 KB |        0.83 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,917.3 us | 17.89 us | 14.94 us |  1.00 | 169.9219 | 15.6250 | 1045.58 KB |        1.00 |


### feat(performance): replace SpecialTokenPatternRegex with faster alloc free solution

- version: 1.2.17 + improvements

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|-----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,280.0 us | 24.67 us | 29.37 us |  0.66 |    0.02 | 105.4688 |  7.8125 |  876.45 KB |        0.84 |
| Encode | .NET 8.0             | .NET 8.0             |   964.4 us | 19.17 us | 22.08 us |  0.50 |    0.02 | 105.4688 |  8.7891 |  867.93 KB |        0.83 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,932.4 us | 38.05 us | 49.48 us |  1.00 |    0.00 | 169.9219 | 15.6250 | 1045.58 KB |        1.00 |


### feat(performance): add benchmark project

- version: 1.2.17 + improvements allocation reduction and compiled regex

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|-----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,270.3 us | 25.03 us | 38.97 us |  0.64 |    0.03 | 105.4688 |  7.8125 |  876.45 KB |        0.84 |
| Encode | .NET 8.0             | .NET 8.0             |   982.2 us | 17.40 us | 14.53 us |  0.49 |    0.02 | 105.4688 |  7.8125 |  867.94 KB |        0.83 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 2,001.6 us | 39.25 us | 64.50 us |  1.00 |    0.00 | 167.9688 | 15.6250 | 1045.58 KB |        1.00 |


### feat(performance): add benchmark project

- version: 1.2.17 -- (baseline)

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Job                  | Runtime              | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |---------:|----------:|----------:|------:|--------:|---------:|--------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1.732 ms | 0.0309 ms | 0.0274 ms |  0.49 |    0.01 | 191.4063 | 15.6250 |   1.53 MB |        0.62 |
| Encode | .NET 8.0             | .NET 8.0             | 1.387 ms | 0.0277 ms | 0.0406 ms |  0.39 |    0.02 | 191.4063 | 15.6250 |   1.53 MB |        0.62 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 3.595 ms | 0.0704 ms | 0.1287 ms |  1.00 |    0.00 | 406.2500 | 39.0625 |   2.46 MB |        1.00 |
