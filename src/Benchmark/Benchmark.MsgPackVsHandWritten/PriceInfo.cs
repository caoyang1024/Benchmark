using System;

namespace Benchmark.MsgPackVsHandWritten;

public sealed partial class PriceInfo 
{
    public const string TopicPrefix = $"{nameof(PriceInfo)}.";

    public string Topic => $"{TopicPrefix}{MarketId}";

    public string FullName => $"{nameof(PriceInfo)}:{MarketId}:{Symbol}:{FeederSource}";

    public long? EventSentTimeEpoch { get; set; }

    public long? EventReceiveTimeEpoch { get; set; }

    /// <summary>
    /// 当从LP收到报价时，PE会给每个报价一个唯一的ID
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// LP报价的发生时间 - 这个时间一般是LP提供的时间
    /// 这个时间只设置一次，不会被修改
    /// </summary>
    public DateTimeOffset LPOutTime { get; set; }

    /// <summary>
    /// PE系统从LP拿到报价的时间
    /// 这个时间只设置一次，不会被修改
    /// </summary>
    public DateTimeOffset SourceInTime { get; set; }

    /// <summary>
    /// 每个节点收到报价的时间
    /// 这个时间会被每个节点修改
    /// </summary>
    public DateTimeOffset ServiceInTime { get; set; }

    /// <summary>
    /// PE系统每个节点发出的时间
    /// 这个时间会被每个节点修改
    /// </summary>
    public DateTimeOffset ServiceOutTime { get; set; }

    public int MarketId { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string FeederSource { get; set; } = string.Empty;

    public decimal Bid { get; set; }

    public decimal? BidVolume { get; set; }

    public decimal Ask { get; set; }

    public decimal? AskVolume { get; set; }

    public decimal Mid { get; set; }

    public int? DepthMarketId { get; set; }

    public string Model { get; set; } = string.Empty;
}

public sealed partial class PriceInfo
{
    public byte[] ToBytes()
    {
        int totalLength = 104 +
                          CorrelationId.Length + Symbol.Length + FeederSource.Length
                          + (BidVolume.HasValue ? 16 : 1) + (AskVolume.HasValue ? 16 : 1)
                          + (DepthMarketId.HasValue ? 8 : 1) + Model.Length
                          + (EventSentTimeEpoch.HasValue ? 8 : 1) + (EventReceiveTimeEpoch.HasValue ? 8 : 1);

        Span<byte> bytes = stackalloc byte[totalLength];

        int offset = 0;

        bytes.WriteString(CorrelationId, ref offset);
        bytes.WriteDateTimeOffset(LPOutTime, ref offset);
        bytes.WriteDateTimeOffset(SourceInTime, ref offset);
        bytes.WriteDateTimeOffset(ServiceInTime, ref offset);
        bytes.WriteDateTimeOffset(ServiceOutTime, ref offset);
        bytes.WriteInt(MarketId, ref offset);
        bytes.WriteString(Symbol, ref offset);
        bytes.WriteString(FeederSource, ref offset);
        bytes.WriteDecimal(Bid, ref offset);
        bytes.WriteBool(BidVolume.HasValue, ref offset);
        if (BidVolume.HasValue)
        {
            bytes.WriteDecimal(BidVolume.Value, ref offset);
        }
        bytes.WriteDecimal(Ask, ref offset);
        bytes.WriteBool(AskVolume.HasValue, ref offset);
        if (AskVolume.HasValue)
        {
            bytes.WriteDecimal(AskVolume.Value, ref offset);
        }
        bytes.WriteDecimal(Mid, ref offset);
        bytes.WriteBool(DepthMarketId.HasValue, ref offset);
        if (DepthMarketId.HasValue)
        {
            bytes.WriteInt(DepthMarketId.Value, ref offset);
        }
        bytes.WriteString(Model, ref offset);
        bytes.WriteBool(EventSentTimeEpoch.HasValue, ref offset);
        if (EventSentTimeEpoch.HasValue)
        {
            bytes.WriteLong(EventSentTimeEpoch.Value, ref offset);
        }
        bytes.WriteBool(EventReceiveTimeEpoch.HasValue, ref offset);
        if (EventReceiveTimeEpoch.HasValue)
        {
            bytes.WriteLong(EventReceiveTimeEpoch.Value, ref offset);
        }

        return bytes.ToArray();
    }

    public static PriceInfo FromBytes(ReadOnlySpan<byte> bytes)
    {
        int offset = 0;

        string correlationId = bytes.ReadString(ref offset);
        DateTimeOffset lpOutTime = bytes.ReadDateTimeOffset(ref offset);
        DateTimeOffset sourceInTime = bytes.ReadDateTimeOffset(ref offset);
        DateTimeOffset serviceInTime = bytes.ReadDateTimeOffset(ref offset);
        DateTimeOffset serviceOutTime = bytes.ReadDateTimeOffset(ref offset);
        int marketId = bytes.ReadInt(ref offset);
        string symbol = bytes.ReadString(ref offset);
        string feederSource = bytes.ReadString(ref offset);
        decimal bid = bytes.ReadDecimal(ref offset);
        decimal? bidVol = bytes.ReadBool(ref offset) ? bytes.ReadDecimal(ref offset) : null;
        decimal ask = bytes.ReadDecimal(ref offset);
        decimal? askVol = bytes.ReadBool(ref offset) ? bytes.ReadDecimal(ref offset) : null;
        decimal mid = bytes.ReadDecimal(ref offset);
        int? depthMarketId = bytes.ReadBool(ref offset) ? bytes.ReadInt(ref offset) : null;
        string model = bytes.ReadString(ref offset);
        long? sentTime = bytes.ReadBool(ref offset) ? bytes.ReadLong(ref offset) : null;
        long? receiveTime = bytes.ReadBool(ref offset) ? bytes.ReadLong(ref offset) : null;

        return new PriceInfo
        {
            CorrelationId = correlationId,
            LPOutTime = lpOutTime,
            SourceInTime = sourceInTime,
            ServiceInTime = serviceInTime,
            ServiceOutTime = serviceOutTime,
            MarketId = marketId,
            Symbol = symbol,
            FeederSource = feederSource,
            Bid = bid,
            BidVolume = bidVol,
            Ask = ask,
            AskVolume = askVol,
            Mid = mid,
            DepthMarketId = depthMarketId,
            Model = model,
            EventSentTimeEpoch = sentTime,
            EventReceiveTimeEpoch = receiveTime
        };
    }
}