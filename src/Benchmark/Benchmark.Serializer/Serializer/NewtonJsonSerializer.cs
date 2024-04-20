using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Benchmark.Serializer.Serializer;

public class NewtonJsonSerializer : ISerializer
{
    private readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All
    });

    public byte[] Serialize<T>(T obj) where T : IMessage
    {
        using var ms = new MemoryStream();
        using var writer = new BsonDataWriter(ms);
        _serializer.Serialize(writer, obj);
        return ms.ToArray();
    }

    public T Deserialize<T>(byte[] data) where T : IMessage
    {
        using var ms = new MemoryStream(data);
        using var reader = new BsonDataReader(ms);
        return _serializer.Deserialize<T>(reader);
    }
}