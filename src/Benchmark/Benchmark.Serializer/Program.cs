using Benchmark.Serializer.Serializer;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MessagePackSerializer = Benchmark.Serializer.Serializer.MessagePackSerializer;

namespace Benchmark.Serializer;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmark>();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class Benchmark
    {
        private readonly ISerializer _newtonJsonSerializer = new NewtonJsonSerializer();
        private readonly ISerializer _systemTextJsonSerializer = new SystemTextJsonSerializer();
        private readonly ISerializer _messagePackSerializer = new MessagePackSerializer();
        private readonly ISerializer _messagePackSerializer2 = new MessagePackSerializer2();

        private SmallMessage _smallMessage;
        private MediumMessage _mediumMessage;
        private LargeMessage _largeMessage;

        [GlobalSetup]
        public void Setup()
        {
            _smallMessage = new SmallMessage
            {
                Id = 1,
                Price = 1.1,
                Text = "Hello",
                Metadata = typeof(SmallMessage).AssemblyQualifiedName
            };
            _mediumMessage = new MediumMessage
            {
                Id = 1,
                Price = 1.1m,
                IsDeleted = false,
                Text = "Hello",
                Metadata = typeof(MediumMessage).AssemblyQualifiedName
            };
            _largeMessage = new LargeMessage
            {
                Id = 1,
                Bid = 1.1m,
                Ask = 1.2m,
                BidVolume = 1.3m,
                AskVolume = 1.4m,
                BidPrice = 1.5,
                AskPrice = 1.6,
                Text = "Hello",
                Metadata = typeof(LargeMessage).AssemblyQualifiedName
            };
        }

        [Benchmark]
        public byte[] NewtonSerializeSmallMessage()
        {
            return _newtonJsonSerializer.Serialize(_smallMessage);
        }

        [Benchmark]
        public byte[] NewtonSerializeMediumMessage()
        {
            return _newtonJsonSerializer.Serialize(_mediumMessage);
        }

        [Benchmark]
        public byte[] NewtonSerializeLargeMessage()
        {
            return _newtonJsonSerializer.Serialize(_largeMessage);
        }

        [Benchmark]
        public byte[] TextJsonSerializeSmallMessage()
        {
            return _systemTextJsonSerializer.Serialize(_smallMessage);
        }

        [Benchmark]
        public byte[] TextJsonSerializeMediumMessage()
        {
            return _systemTextJsonSerializer.Serialize(_mediumMessage);
        }

        [Benchmark]
        public byte[] TextJsonSerializeLargeMessage()
        {
            return _systemTextJsonSerializer.Serialize(_largeMessage);
        }

        [Benchmark]
        public byte[] MessagePackSerializeSmallMessage()
        {
            return _messagePackSerializer.Serialize(_smallMessage);
        }

        [Benchmark]
        public byte[] MessagePackSerializeMediumMessage()
        {
            return _messagePackSerializer.Serialize(_mediumMessage);
        }

        [Benchmark]
        public byte[] MessagePackSerializeLargeMessage()
        {
            return _messagePackSerializer.Serialize(_largeMessage);
        }

        [Benchmark]
        public byte[] MessagePack2SerializeSmallMessage()
        {
            return _messagePackSerializer2.Serialize(_smallMessage);
        }

        [Benchmark]
        public byte[] MessagePack2SerializeMediumMessage()
        {
            return _messagePackSerializer2.Serialize(_mediumMessage);
        }

        [Benchmark]
        public byte[] MessagePack2SerializeLargeMessage()
        {
            return _messagePackSerializer2.Serialize(_largeMessage);
        }

        /*
         * Add deserialization benchmarks here
         */

        [Benchmark]
        public SmallMessage NewtonDeserializeSmallMessage()
        {
            var bytes = _newtonJsonSerializer.Serialize(_smallMessage);

            return (SmallMessage)_newtonJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public MediumMessage NewtonDeserializeMediumMessage()
        {
            var bytes = _newtonJsonSerializer.Serialize(_mediumMessage);

            return (MediumMessage)_newtonJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public LargeMessage NewtonDeserializeLargeMessage()
        {
            var bytes = _newtonJsonSerializer.Serialize(_largeMessage);

            return (LargeMessage)_newtonJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public SmallMessage TextJsonDeserializeSmallMessage()
        {
            var bytes = _systemTextJsonSerializer.Serialize(_smallMessage);

            return (SmallMessage)_systemTextJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public MediumMessage TextJsonDeserializeMediumMessage()
        {
            var bytes = _systemTextJsonSerializer.Serialize(_mediumMessage);

            return (MediumMessage)_systemTextJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public LargeMessage TextJsonDeserializeLargeMessage()
        {
            var bytes = _systemTextJsonSerializer.Serialize(_largeMessage);

            return (LargeMessage)_systemTextJsonSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public SmallMessage MessagePackDeserializeSmallMessage()
        {
            var bytes = _messagePackSerializer.Serialize(_smallMessage);

            return (SmallMessage)_messagePackSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public MediumMessage MessagePackDeserializeMediumMessage()
        {
            var bytes = _messagePackSerializer.Serialize(_mediumMessage);

            return (MediumMessage)_messagePackSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public LargeMessage MessagePackDeserializeLargeMessage()
        {
            var bytes = _messagePackSerializer.Serialize(_largeMessage);

            return (LargeMessage)_messagePackSerializer.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public SmallMessage MessagePack2DeserializeSmallMessage()
        {
            var bytes = _messagePackSerializer2.Serialize(_smallMessage);

            return (SmallMessage)_messagePackSerializer2.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public MediumMessage MessagePack2DeserializeMediumMessage()
        {
            var bytes = _messagePackSerializer2.Serialize(_mediumMessage);

            return (MediumMessage)_messagePackSerializer2.Deserialize<IMessage>(bytes);
        }

        [Benchmark]
        public LargeMessage MessagePack2DeserializeLargeMessage()
        {
            var bytes = _messagePackSerializer2.Serialize(_largeMessage);

            return (LargeMessage)_messagePackSerializer2.Deserialize<IMessage>(bytes);
        }
    }
}