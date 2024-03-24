# SharpToken Benchmark

This runs a encode benchmark over all samples in GptEncoderTestSamples.txt

#### How To Run

Make a release build and run the `SharpToken.Benchmark.exe`
in `..\SharpToken\SharpToken.Benchmark\bin\Release\net8.0\SharpToken.Benchmark.exe`


## Comparison to other Tokenizer

**After Optimization:**

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method         | Job                  | Runtime              | Mean     | Error    | StdDev   | Gen0       | Gen1      | Allocated |
|--------------- |--------------------- |--------------------- |---------:|---------:|---------:|-----------:|----------:|----------:|
| **SharpToken** | .NET 8.0             | .NET 8.0             | 100.4 ms |  1.95 ms |  1.91 ms |  2000.0000 |         - |  22.13 MB |
| **SharpToken** | .NET 6.0             | .NET 6.0             | 169.9 ms |  2.42 ms |  2.15 ms | 24333.3333 | 1000.0000 |  196.3 MB |
| **SharpToken** | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 455.3 ms |  8.34 ms |  6.97 ms | 34000.0000 | 1000.0000 | 204.39 MB |
|                |                      |                      |          |          |          |            |           |           |
| *TiktokenSharp*| .NET 8.0             | .NET 8.0             | 211.4 ms |  1.83 ms |  1.53 ms | 42000.0000 | 1000.0000 | 338.98 MB |
| *TiktokenSharp*| .NET 6.0             | .NET 6.0             | 258.6 ms |  5.09 ms |  6.25 ms | 39000.0000 | 1000.0000 | 313.26 MB |
| *TiktokenSharp*| .NET Framework 4.7.1 | .NET Framework 4.7.1 | 638.3 ms | 12.47 ms | 16.21 ms | 63000.0000 | 1000.0000 | 378.31 MB |
|                |                      |                      |          |          |          |            |           |           |
| *TokenizerLib* | .NET 8.0             | .NET 8.0             | 124.4 ms |  1.81 ms |  1.60 ms | 27250.0000 | 1000.0000 | 217.82 MB |
| *TokenizerLib* | .NET 6.0             | .NET 6.0             | 165.5 ms |  1.38 ms |  1.16 ms | 27000.0000 | 1000.0000 | 217.82 MB |
| *TokenizerLib* | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 499.7 ms |  9.81 ms | 14.07 ms | 40000.0000 | 1000.0000 | 243.79 MB |


**Before Optimization:**

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method         | Job                  | Runtime              | Mean     | Error    | StdDev  | Gen0       | Gen1      | Allocated |
|--------------- |--------------------- |--------------------- |---------:|---------:|--------:|-----------:|----------:|----------:|
| **SharpToken** | .NET 8.0             | .NET 8.0             | 214.1 ms |  4.26 ms | 3.99 ms | 33000.0000 | 1000.0000 | 264.44 MB |
| **SharpToken** | .NET 6.0             | .NET 6.0             | 267.9 ms |  2.61 ms | 2.31 ms | 33000.0000 | 1000.0000 | 264.82 MB |
| **SharpToken** | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 640.2 ms |  9.03 ms | 7.54 ms | 54000.0000 | 2000.0000 | 326.51 MB |
|                |                      |                      |          |          |          |            |           |           |
| *TiktokenSharp*| .NET 8.0             | .NET 8.0             | 218.2 ms |  4.25 ms | 6.49 ms | 42000.0000 | 1000.0000 | 338.98 MB |
| *TiktokenSharp*| .NET 6.0             | .NET 6.0             | 250.1 ms |  1.64 ms | 1.28 ms | 39000.0000 | 1000.0000 | 313.26 MB |
| *TiktokenSharp*| .NET Framework 4.7.1 | .NET Framework 4.7.1 | 654.2 ms | 11.57 ms | 9.66 ms | 63000.0000 | 1000.0000 | 378.31 MB |
|                |                      |                      |          |          |          |            |           |           |
| *TokenizerLib* | .NET 8.0             | .NET 8.0             | 130.3 ms |  2.29 ms | 2.03 ms | 27200.0000 | 1000.0000 | 217.82 MB |
| *TokenizerLib* | .NET 6.0             | .NET 6.0             | 166.3 ms |  0.74 ms | 0.58 ms | 27000.0000 | 1000.0000 | 217.82 MB |
| *TokenizerLib* | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 489.4 ms |  6.41 ms | 5.68 ms | 40000.0000 | 1000.0000 | 243.79 MB |


## Global Test

- version: 1.2.17 (improved) after optimization

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method                | Job                  | Runtime              | Mean          | Error       | StdDev      | Ratio | RatioSD | Gen0      | Gen1     | Gen2     | Allocated | Alloc Ratio |
|---------------------- |--------------------- |--------------------- |--------------:|------------:|------------:|------:|--------:|----------:|---------:|---------:|----------:|------------:|
| Encode_SmallText      | .NET 6.0             | .NET 6.0             |     59.950 us |   1.1894 us |   2.1142 us |  0.57 |    0.02 |    6.0425 |   0.3662 |        - |   50784 B |        0.80 |
| Encode_SmallText      | .NET 8.0             | .NET 8.0             |     21.832 us |   0.0777 us |   0.0648 us |  0.21 |    0.00 |    0.0610 |        - |        - |     696 B |        0.01 |
| Encode_SmallText      | .NET Framework 4.7.1 | .NET Framework 4.7.1 |    106.063 us |   2.1103 us |   1.8707 us |  1.00 |    0.00 |   10.1318 |   0.6104 |        - |   63876 B |        1.00 |
|                       |                      |                      |               |             |             |       |         |           |          |          |           |             |
| Encode_LargeText      | .NET 6.0             | .NET 6.0             | 14,577.921 us | 288.6274 us | 527.7715 us |  0.67 |    0.03 |  796.8750 | 312.5000 | 125.0000 | 6382272 B |        0.82 |
| Encode_LargeText      | .NET 8.0             | .NET 8.0             |  4,391.384 us |  86.2051 us |  80.6363 us |  0.20 |    0.01 |         - |        - |        - |  155547 B |        0.02 |
| Encode_LargeText      | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 21,974.406 us | 387.1054 us | 362.0986 us |  1.00 |    0.00 | 1375.0000 | 562.5000 | 187.5000 | 7752266 B |        1.00 |
|                       |                      |                      |               |             |             |       |         |           |          |          |           |             |
| Decode_SmallText      | .NET 6.0             | .NET 6.0             |      2.587 us |   0.0442 us |   0.0413 us |  0.69 |    0.01 |    0.2747 |        - |        - |    2320 B |        0.98 |
| Decode_SmallText      | .NET 8.0             | .NET 8.0             |      1.609 us |   0.0306 us |   0.0352 us |  0.43 |    0.01 |    0.2766 |        - |        - |    2320 B |        0.98 |
| Decode_SmallText      | .NET Framework 4.7.1 | .NET Framework 4.7.1 |      3.775 us |   0.0524 us |   0.0465 us |  1.00 |    0.00 |    0.3738 |        - |        - |    2375 B |        1.00 |
|                       |                      |                      |               |             |             |       |         |           |          |          |           |             |
| Decode_LargeText      | .NET 6.0             | .NET 6.0             |    476.205 us |   4.9629 us |   4.6423 us |  0.81 |    0.02 |   30.2734 |  13.1836 |   8.7891 |  286982 B |        1.00 |
| Decode_LargeText      | .NET 8.0             | .NET 8.0             |    446.240 us |   5.9289 us |   5.2558 us |  0.76 |    0.02 |   25.3906 |   8.3008 |   3.9063 |  286979 B |        1.00 |
| Decode_LargeText      | .NET Framework 4.7.1 | .NET Framework 4.7.1 |    584.655 us |   9.8253 us |   9.1906 us |  1.00 |    0.00 |   38.0859 |  15.6250 |   9.7656 |  287525 B |        1.00 |
|                       |                      |                      |               |             |             |       |         |           |          |          |           |             |
| CountTokens_SmallText | .NET 6.0             | .NET 6.0             |     57.592 us |   1.1164 us |   1.2409 us |  0.55 |    0.01 |    5.9814 |   0.3052 |        - |   50080 B |       0.790 |
| CountTokens_SmallText | .NET 8.0             | .NET 8.0             |     21.703 us |   0.2386 us |   0.2232 us |  0.21 |    0.00 |         - |        - |        - |     184 B |       0.003 |
| CountTokens_SmallText | .NET Framework 4.7.1 | .NET Framework 4.7.1 |    104.900 us |   1.6103 us |   1.4275 us |  1.00 |    0.00 |   10.0098 |        - |        - |   63355 B |       1.000 |
|                       |                      |                      |               |             |             |       |         |           |          |          |           |             |
| CountTokens_LargeText | .NET 6.0             | .NET 6.0             | 13,565.199 us | 117.2530 us | 103.9417 us |  0.63 |    0.01 |  781.2500 | 328.1250 | 125.0000 | 6122858 B |       0.806 |
| CountTokens_LargeText | .NET 8.0             | .NET 8.0             |  4,293.409 us |  70.0026 us |  65.4805 us |  0.20 |    0.00 |         - |        - |        - |     195 B |       0.000 |
| CountTokens_LargeText | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 21,647.381 us | 336.1202 us | 314.4071 us |  1.00 |    0.00 | 1343.7500 | 531.2500 | 187.5000 | 7598360 B |       1.000 |



- version: 1.2.17 (baseline) before optimization

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method           | Job                  | Runtime              | Mean          | Error       | StdDev      | Ratio | RatioSD | Gen0      | Gen1      | Gen2     | Allocated   | Alloc Ratio |
|----------------- |--------------------- |--------------------- |--------------:|------------:|------------:|------:|--------:|----------:|----------:|---------:|------------:|------------:|
| Encode_SmallText | .NET 6.0             | .NET 6.0             |     82.479 us |   0.1286 us |   0.1074 us |  0.48 |    0.00 |   10.1318 |    0.6104 |        - |    82.89 KB |        0.66 |
| Encode_SmallText | .NET 8.0             | .NET 8.0             |     59.351 us |   0.4628 us |   0.3864 us |  0.35 |    0.00 |   10.1318 |    0.6104 |        - |    82.85 KB |        0.66 |
| Encode_SmallText | .NET Framework 4.7.1 | .NET Framework 4.7.1 |    171.859 us |   1.8375 us |   1.6289 us |  1.00 |    0.00 |   20.2637 |    1.2207 |        - |   125.24 KB |        1.00 |
|                  |                      |                      |               |             |             |       |         |           |           |          |             |             |
| Encode_LargeText | .NET 6.0             | .NET 6.0             | 18,357.779 us |  81.1564 us |  71.9430 us |  0.52 |    0.01 | 1500.0000 |  500.0000 | 156.2500 | 11171.52 KB |        0.66 |
| Encode_LargeText | .NET 8.0             | .NET 8.0             | 16,315.221 us | 323.6828 us | 503.9351 us |  0.46 |    0.02 | 1500.0000 |  500.0000 | 156.2500 | 11171.35 KB |        0.66 |
| Encode_LargeText | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 35,173.965 us | 681.1456 us | 668.9760 us |  1.00 |    0.00 | 3133.3333 | 1400.0000 | 400.0000 | 16983.19 KB |        1.00 |
|                  |                      |                      |               |             |             |       |         |           |           |          |             |             |
| Decode_SmallText | .NET 6.0             | .NET 6.0             |      2.220 us |   0.0211 us |   0.0187 us |  0.65 |    0.01 |    0.2708 |         - |        - |     2.23 KB |        0.98 |
| Decode_SmallText | .NET 8.0             | .NET 8.0             |      1.606 us |   0.0200 us |   0.0187 us |  0.47 |    0.01 |    0.2728 |         - |        - |     2.23 KB |        0.98 |
| Decode_SmallText | .NET Framework 4.7.1 | .NET Framework 4.7.1 |      3.406 us |   0.0430 us |   0.0403 us |  1.00 |    0.00 |    0.3700 |         - |        - |     2.29 KB |        1.00 |
|                  |                      |                      |               |             |             |       |         |           |           |          |             |             |
| Decode_LargeText | .NET 6.0             | .NET 6.0             |    432.534 us |   1.9471 us |   1.8213 us |  0.80 |    0.02 |   33.6914 |   18.5547 |  12.2070 |   280.23 KB |        1.00 |
| Decode_LargeText | .NET 8.0             | .NET 8.0             |    373.513 us |   1.3223 us |   1.1722 us |  0.69 |    0.01 |   34.1797 |   19.0430 |  12.6953 |   280.23 KB |        1.00 |
| Decode_LargeText | .NET Framework 4.7.1 | .NET Framework 4.7.1 |    540.774 us |  10.3775 us |  10.1921 us |  1.00 |    0.00 |   40.0391 |   17.5781 |  11.7188 |   280.41 KB |        1.00 |



## Results:

### feat(benchmark): add benchmark for large file token count

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

| Method                 | Job                  | Runtime              | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Gen2     | Allocated | Alloc Ratio |
|----------------------- |--------------------- |--------------------- |------------:|----------:|----------:|------:|--------:|----------:|---------:|---------:|----------:|------------:|
| Encode                 | .NET 6.0             | .NET 6.0             |    973.9 us |  19.27 us |  31.66 us |  0.77 |    0.03 |   62.5000 |   0.9766 |        - |  524034 B |        0.82 |
| Encode                 | .NET 8.0             | .NET 8.0             |    552.3 us |  10.73 us |  11.48 us |  0.44 |    0.01 |    2.9297 |        - |        - |   27841 B |        0.04 |
| Encode                 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |  1,252.7 us |  24.12 us |  23.69 us |  1.00 |    0.00 |  101.5625 |   1.9531 |        - |  640074 B |        1.00 |
|                        |                      |                      |             |           |           |       |         |           |          |          |           |             |
| CountTokens            | .NET 6.0             | .NET 6.0             |    801.2 us |  16.00 us |  19.65 us |  0.65 |    0.02 |   58.5938 |   0.9766 |        - |  496386 B |       0.806 |
| CountTokens            | .NET 8.0             | .NET 8.0             |    557.3 us |  10.89 us |  10.69 us |  0.45 |    0.02 |         - |        - |        - |    4161 B |       0.007 |
| CountTokens            | .NET Framework 4.7.1 | .NET Framework 4.7.1 |  1,235.5 us |  24.52 us |  31.01 us |  1.00 |    0.00 |   97.6563 |   1.9531 |        - |  615801 B |       1.000 |
|                        |                      |                      |             |           |           |       |         |           |          |          |           |             |
| CountTokens_LargeInput | .NET 6.0             | .NET 6.0             | 13,955.6 us | 258.91 us | 242.19 us |  0.64 |    0.01 |  781.2500 | 328.1250 | 125.0000 | 6122849 B |       0.806 |
| CountTokens_LargeInput | .NET 8.0             | .NET 8.0             |  5,781.9 us | 115.07 us | 127.91 us |  0.27 |    0.01 |         - |        - |        - |      75 B |       0.000 |
| CountTokens_LargeInput | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 21,674.7 us | 374.37 us | 350.18 us |  1.00 |    0.00 | 1312.5000 | 500.0000 | 156.2500 | 7596775 B |       1.000 |

**Notable Improvments**
- CountTokens_LargeInput on .NET 8.0 only allocated 75 B!
- CountTokens_LargeInput runs only one iteration while all other run around 66 iterations!


### feat(performance): reduce minor allocations

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

| Method      | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------ |--------------------- |--------------------- |-----------:|---------:|---------:|------:|---------:|-------:|----------:|------------:|
| Encode      | .NET 6.0             | .NET 6.0             |   818.8 us | 16.27 us | 17.41 us |  0.67 |  62.5000 | 0.9766 | 511.75 KB |        0.82 |
| Encode      | .NET 8.0             | .NET 8.0             |   551.0 us |  2.82 us |  2.21 us |  0.45 |   2.9297 |      - |  27.19 KB |        0.04 |
| Encode      | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,232.3 us |  5.40 us |  4.79 us |  1.00 | 101.5625 | 1.9531 | 625.07 KB |        1.00 |
|             |                      |                      |            |          |          |       |          |        |           |             |
| CountTokens | .NET 6.0             | .NET 6.0             |   802.6 us |  7.71 us |  7.21 us |  0.64 |  58.5938 | 0.9766 | 484.75 KB |       0.806 |
| CountTokens | .NET 8.0             | .NET 8.0             |   553.1 us |  3.97 us |  3.52 us |  0.44 |        - |      - |   4.06 KB |       0.007 |
| CountTokens | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 1,247.6 us | 16.49 us | 14.62 us |  1.00 |  97.6563 | 1.9531 | 601.37 KB |       1.000 |


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
