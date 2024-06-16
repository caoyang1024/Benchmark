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
    private static async Task Main(string[] args)
    {
        const int numberOfSymbols = 100;
        const int limitPerSecond = 100;
        const int stopCount = limitPerSecond * 20;

        RunThrottle(limitPerSecond, stopCount);
        await RunKeyedThrottle(numberOfSymbols, limitPerSecond, stopCount);

        //BenchmarkDotNet.Running.BenchmarkRunner.Run<ThrottleBenchmark>();
    }

    private static void RunThrottle(int limitPerSecond, int stopCount)
    {
        Throttle throttle = Throttle.Create(limitPerSecond);

        var timestamps = new List<DateTimeOffset>();

        while (true)
        {
            if (throttle.TryAcquire())
            {
                timestamps.Add(DateTimeOffset.UtcNow);
            }

            if (timestamps.Count >= stopCount)
            {
                break;
            }
        }

        Print(timestamps);
    }

    private static async Task RunKeyedThrottle(int numOfSymbols, int limitPerSecond, int stopCount)
    {
        KeyedThrottle keyedThrottle = new KeyedThrottle();
        List<List<(string, DateTimeOffset)>> all = new();
        object locker = new object();

        for (int i = 0; i < numOfSymbols; i++)
        {
            keyedThrottle.AddOrUpdate($"{i}", TimeSpan.FromSeconds(1), limitPerSecond);
        }

        List<Task> tasks = new();

        for (int i = 0; i < numOfSymbols; i++)
        {
            string key = $"{i}";

            tasks.Add(Task.Run(() =>
            {
                var timestamps = new List<(string, DateTimeOffset)>();

                while (true)
                {
                    if (keyedThrottle.TryAcquire(key))
                    {
                        timestamps.Add((key, DateTimeOffset.UtcNow));
                    }

                    if (timestamps.Count >= stopCount)
                    {
                        lock (locker)
                        {
                            all.Add(timestamps);
                        }

                        break;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        Print(all.SelectMany(x => x));
    }

    public static void Print(IEnumerable<(string Symbol, DateTimeOffset Time)> timestamps)
    {
        foreach (var group in timestamps.GroupBy(x => new
        {
            x.Symbol,
            x.Time.Year,
            x.Time.Month,
            x.Time.Day,
            x.Time.Hour,
            x.Time.Minute,
            x.Time.Second
        }).OrderBy(x => x.Key.Symbol))
        {
            Console.WriteLine($"{group.Key} {group.Count()}");
        }
    }

    public static void Print(IEnumerable<DateTimeOffset> timestamps)
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