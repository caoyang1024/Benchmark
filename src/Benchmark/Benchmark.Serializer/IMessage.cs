using System;

namespace Benchmark.Serializer;

public interface IMessage
{
    public Guid Guid { get; set; }
    public string Metadata { get; set; }
}