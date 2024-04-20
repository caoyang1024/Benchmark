namespace Benchmark.Serializer.Serializer;

public class MessagePackSerializer : ISerializer
{
    public byte[] Serialize<T>(T obj) where T : IMessage
    {
        return MessagePack.MessagePackSerializer.Typeless.Serialize(obj);
    }

    public T Deserialize<T>(byte[] data) where T : IMessage
    {
        return (T)MessagePack.MessagePackSerializer.Typeless.Deserialize(data);
    }
}