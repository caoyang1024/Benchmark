namespace Benchmark.Serializer.Serializer;

public class SystemTextJsonSerializer : ISerializer
{
    public byte[] Serialize<T>(T obj) where T : IMessage
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, typeof(T));
    }

    public T Deserialize<T>(byte[] data) where T : IMessage
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }
}