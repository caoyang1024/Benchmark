using System;

namespace Benchmark.Serializer;

// 20 bytes
public class SmallMessage : IMessage
{
    public int Id { get; set; }
    public double Price { get; set; }
    public string Text { get; set; }
    public Guid Guid { get; set; }
    public string Metadata { get; set; }
}