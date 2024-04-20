using MessagePack.Resolvers;

namespace Benchmark.Serializer.Serializer;

public class MessagePackSerializer2 : ISerializer
{
    public byte[] Serialize<T>(T obj) where T : IMessage
    {
        return MessagePack.MessagePackSerializer.Serialize(typeof(T), obj, ContractlessStandardResolver.Options);
    }

    public T Deserialize<T>(byte[] data) where T : IMessage
    {
        return (T)MessagePack.MessagePackSerializer.Deserialize(typeof(T), data, ContractlessStandardResolver.Options);
    }
}