using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.GUIDvsULID;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<Test>();
    }
}

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[RPlotExporter]
public class Test
{
    [Benchmark]
    public string GenerateGuid()
    {
        return GuidHelper.GetGuid();
    }

    [Benchmark]
    public string GenerateUlid()
    {
        return UlidHelper.GetUlid();
    }
}