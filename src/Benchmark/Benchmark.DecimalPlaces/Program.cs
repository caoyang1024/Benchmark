using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.DecimalPlaces;

public class Program
{
    private static readonly decimal[] Values =
    {
        1.23450m,
        1.2345m,
        1.23456789m,
        13231323213.23232323m,
        4334.3432m,
        32445.433434m,
        0m,
        decimal.MaxValue,
        -1.2345000m,
        -1.2345m,
        -1.23456789m,
        -13231323213.23232323m,
        -4334.3432m,
        -32445.433434m,
        -0m,
        decimal.MinValue,
    };

    public static void Main(string[] args)
    {
        foreach (var value in Values)
        {
            Console.WriteLine($"value: {value} has {DecimalPlaces.GetDecimalPlaces1(value)} decimal places");
            Console.WriteLine($"value: {value} has {DecimalPlaces.GetDecimalPlaces2(value)} decimal places");
            Console.WriteLine($"value: {value} has {DecimalPlaces.GetDecimalPlaces3(value)} decimal places");
        }

        BenchmarkDotNet.Running.BenchmarkRunner.Run<DecimalPlaces>();
    }
}

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class DecimalPlaces
{
    private static readonly decimal[] Values =
    {
        1.2345000m,
        1.2345m,
        1.23456789m,
        13231323213.23232323m,
        4334.3432m,
        32445.433434m,
        -1.2345000m,
        -1.2345m,
        -1.23456789m,
        -13231323213.23232323m,
        -4334.3432m,
        -32445.433434m,
        0m,
        -0m,
    };

    [Benchmark]
    public void Get1()
    {
        foreach (var value in Values)
        {
            GetDecimalPlaces1(value);
        }
    }

    [Benchmark]
    public void Get2()
    {
        foreach (var value in Values)
        {
            GetDecimalPlaces2(value);
        }
    }

    [Benchmark]
    public void Get3()
    {
        foreach (var value in Values)
        {
            GetDecimalPlaces3(value);
        }
    }

    public static int GetDecimalPlaces1(decimal value)
    {
        Span<int> data = stackalloc int[4];
        decimal.GetBits(value, data);
        // extract bits 16-23 of the flags value
        const int mask = (1 << 8) - 1;
        return (data[3] >> 16) & mask;
    }

    public static int GetDecimalPlaces2(decimal number)
    {
        int decimalPlaces = 0;
        while (number != Math.Round(number))
        {
            decimalPlaces++;
            number *= 10;
        }
        return decimalPlaces;
    }

    public static int GetDecimalPlaces3(decimal n)
    {
        n = Math.Abs(n); //make sure it is positive.
        n -= (ulong)n;     //remove the integer part of the number.
        var decimalPlaces = 0;
        while (n > 0)
        {
            decimalPlaces++;
            n *= 10;
            n -= (int)n;
        }
        return decimalPlaces;
    }
}