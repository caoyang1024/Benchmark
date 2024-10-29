namespace Benchmark.MsgPackVsHandWritten;

public class Program
{
    private static void Main(string[] args)
    {
        //MsgPackAndHandWritten msgPackAndHandWritten = new MsgPackAndHandWritten();
        //msgPackAndHandWritten.Setup();
        //var price = msgPackAndHandWritten.FromHandWrittenBytes();

        BenchmarkDotNet.Running.BenchmarkRunner.Run<MsgPackAndHandWritten>();
    }
}