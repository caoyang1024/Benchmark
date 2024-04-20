using System;

namespace Benchmark.Serializer;

// 200 bytes
public class MediumMessage : IMessage
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
    public string Text { get; set; }
    public Guid Guid { get; set; }
    public string Metadata { get; set; }
}