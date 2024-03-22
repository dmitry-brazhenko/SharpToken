

This runs a encode benchmark over all samples in GptEncoderTestSamples.txt

# How To Run

Make a release build and run the `SharpToken.Benchmark.exe`
in `..\SharpToken\SharpToken.Benchmark\bin\Release\net8.0\SharpToken.Benchmark.exe`




## Results:


### commit | feat(performance): add benchmark project | version: 1.2.17 + improvements allocation reduction and compiled regex

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256


| Method | Job                  | Runtime              | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------- |--------------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|-----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1,270.3 us | 25.03 us | 38.97 us |  0.64 |    0.03 | 105.4688 |  7.8125 |  876.45 KB |        0.84 |
| Encode | .NET 8.0             | .NET 8.0             |   982.2 us | 17.40 us | 14.53 us |  0.49 |    0.02 | 105.4688 |  7.8125 |  867.94 KB |        0.83 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 2,001.6 us | 39.25 us | 64.50 us |  1.00 |    0.00 | 167.9688 | 15.6250 | 1045.58 KB |        1.00 |


### commit | feat(performance): add benchmark project | version: 1.2.17

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.200
  [Host]               : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET Framework 4.7.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256


| Method | Job                  | Runtime              | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------- |--------------------- |--------------------- |---------:|----------:|----------:|------:|--------:|---------:|--------:|----------:|------------:|
| Encode | .NET 6.0             | .NET 6.0             | 1.732 ms | 0.0309 ms | 0.0274 ms |  0.49 |    0.01 | 191.4063 | 15.6250 |   1.53 MB |        0.62 |
| Encode | .NET 8.0             | .NET 8.0             | 1.387 ms | 0.0277 ms | 0.0406 ms |  0.39 |    0.02 | 191.4063 | 15.6250 |   1.53 MB |        0.62 |
| Encode | .NET Framework 4.7.1 | .NET Framework 4.7.1 | 3.595 ms | 0.0704 ms | 0.1287 ms |  1.00 |    0.00 | 406.2500 | 39.0625 |   2.46 MB |        1.00 |
