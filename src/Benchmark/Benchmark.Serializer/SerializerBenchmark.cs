using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace Benchmark.Serializer;

public class SerializerBenchmark
{
    [Benchmark]
    public void SerializeSmallMessage()
    {
        var message = new SmallMessage
        {
            Id = 1,
            Price = 1.1,
            Text = "Hello, World!"
        };

        var json = JsonConvert.SerializeObject(message);
    }

    [Benchmark]
    public void SerializeMediumMessage()
    {
        var message = new MediumMessage
        {
            Id = 1,
            Price = 1.1m,
            IsDeleted = false,
            Text = "Hello, World!"
        };

        var json = JsonConvert.SerializeObject(message);
    }

    [Benchmark]
    public void SerializeLargeMessage()
    {
        var message = new LargeMessage
        {
            Id = 1,
            Bid = 1.1m,
            Ask = 1.2m,
            BidVolume = 1.3m,
            AskVolume = 1.4m,
            BidPrice = 1.5,
            AskPrice = 1.6,
            Text = "Hello, World!"
        };

        var json = JsonConvert.SerializeObject(message);
    }
}