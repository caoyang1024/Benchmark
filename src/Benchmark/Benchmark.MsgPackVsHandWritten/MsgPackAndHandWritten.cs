using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using K4os.Compression.LZ4;
using System;

namespace Benchmark.MsgPackVsHandWritten;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class MsgPackAndHandWritten
{
    private PriceInfo _price;

    [GlobalSetup]
    public void Setup()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        _price = new PriceInfo
        {
            CorrelationId = "test-correlation-id",
            LPOutTime = now,
            SourceInTime = now.AddMilliseconds(100),
            ServiceInTime = now.AddMilliseconds(200),
            ServiceOutTime = now.AddMilliseconds(300),
            MarketId = 12345,
            Symbol = "XYZ",
            FeederSource = "TestFeeder",
            Bid = 123.456m,
            BidVolume = 789.012m,
            Ask = 654.321m,
            AskVolume = 210.987m,
            Mid = 400.789m,
            DepthMarketId = 67890,
            Model = "TestModel"
        };
    }

    [Benchmark]
    public byte[] ToMsgPackBytes()
    {
        return MessagePack.MessagePackSerializer.Serialize(_price.GetType(), _price,
             MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Options);
    }

    private static readonly byte[] Buffer = new byte[2048];

    [Benchmark]
    public byte[] ToHandWrittenBytes0()
    {
        return _price.ToBytes();
    }

    [Benchmark]
    public Span<byte> ToHandWrittenBytes1()
    {
        var data = _price.ToBytes();

        int length = LZ4Codec.Encode(data, 0, data.Length, Buffer, 0, Buffer.Length);

        return Buffer.AsSpan(0, length);
    }

    [Benchmark]
    public byte[] ToHandWrittenBytes2()
    {
        var data = _price.ToBytes();

        return LZ4Pickler.Pickle(data);
    }
}