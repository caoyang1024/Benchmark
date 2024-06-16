using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Throttle;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[RPlotExporter]
public class ThrottleBenchmark
{
    [Params(10, 100, 1000, 10000)]
    public int LimitPerSecond;

    private Throttle _throttle;
    private FixedWindowRateLimiter _fixedLimiter;

    [GlobalSetup]
    public void Setup()
    {
        _throttle = Benchmark.Throttle.Throttle.Create(LimitPerSecond);
        _fixedLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromSeconds(1),
            AutoReplenishment = true,
            PermitLimit = LimitPerSecond,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    }

    [Benchmark]
    public void Throttle()
    {
        _throttle.TryAcquire();
    }

    [Benchmark]
    public async Task RateLimiter()
    {
        var lease = await _fixedLimiter.AcquireAsync();
        if (lease.IsAcquired)
        {
        }
    }
}

public class Program
{
    private static void Main(string[] args)
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<ThrottleBenchmark>();
    }

    public static void Print(IList<DateTimeOffset> timestamps)
    {
        foreach (var group in timestamps.GroupBy(x => new
        {
            x.Year,
            x.Month,
            x.Day,
            x.Hour,
            x.Minute,
            x.Second
        }))
        {
            Console.WriteLine($"{group.Key} {group.Count()}");
        }
    }
}