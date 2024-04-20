using System;

namespace Benchmark.Serializer;

// 1000 bytes
public class LargeMessage : IMessage
{
    public int Id { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal BidVolume { get; set; }
    public decimal AskVolume { get; set; }
    public double BidPrice { get; set; }
    public double AskPrice { get; set; }
    public string Text { get; set; }
    public Guid Guid { get; set; }
    public string Metadata { get; set; }
}