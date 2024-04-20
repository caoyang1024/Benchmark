namespace Benchmark.Serializer;

public interface ISerializer
{
    byte[] Serialize<T>(T obj) where T : IMessage;

    T Deserialize<T>(byte[] data) where T : IMessage;
}