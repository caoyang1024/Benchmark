using System;

namespace Benchmark.MsgPackVsHandWritten;

public class Program
{
    private static void Main(string[] args)
    {
        MsgPackAndHandWritten instance = new();

        instance.Setup();

        var b1 = instance.ToMsgPackBytes();
        var b2 = instance.ToHandWrittenBytes0();
        var b3 = instance.ToHandWrittenBytes1();
        var b4 = instance.ToHandWrittenBytes2();

        Console.WriteLine($"{b1.Length} vs {b2.Length} vs {b3.Length} vs {b4.Length}");

        BenchmarkDotNet.Running.BenchmarkRunner.Run<MsgPackAndHandWritten>();
    }
}