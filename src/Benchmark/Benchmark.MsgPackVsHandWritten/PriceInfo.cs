using System;

namespace Benchmark.MsgPackVsHandWritten;

public sealed partial class PriceInfo
{
    /// <summary>
    /// 当从LP收到报价时，PE会给每个报价一个唯一的ID
    /// </summary>
    public string CorrelationId { get; init; }

    /// <summary>
    /// LP报价的发生时间 - 这个时间一般是LP提供的时间
    /// 这个时间只设置一次，不会被修改
    /// </summary>
    public DateTimeOffset LPOutTime { get; init; }

    /// <summary>
    /// PE系统从LP拿到报价的时间
    /// 这个时间只设置一次，不会被修改
    /// </summary>
    public DateTimeOffset SourceInTime { get; init; }

    /// <summary>
    /// 每个节点收到报价的时间
    /// 这个时间会被每个节点修改
    /// </summary>
    public DateTimeOffset ServiceInTime { get; init; }

    /// <summary>
    /// PE系统每个节点发出的时间
    /// 这个时间会被每个节点修改
    /// </summary>
    public DateTimeOffset ServiceOutTime { get; init; }

    public int MarketId { get; init; }

    public string Symbol { get; init; }

    public string FeederSource { get; init; }

    public decimal Bid { get; init; }

    public decimal? BidVolume { get; init; }

    public decimal Ask { get; init; }

    public decimal? AskVolume { get; init; }

    public decimal Mid { get; init; }

    public int? DepthMarketId { get; init; }

    public string Model { get; init; }

    public string FullName => $"{nameof(PriceInfo)}:{MarketId}:{Symbol}:{FeederSource}";
}

public sealed partial class PriceInfo
{
    public byte[] ToBytes()
    {
        int totalLength = 104 + CorrelationId.Length + Symbol.Length + FeederSource.Length
                          + (BidVolume.HasValue ? 16 : 1) + (AskVolume.HasValue ? 16 : 1)
                          + (DepthMarketId.HasValue ? 8 : 1) + Model.Length;

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
            Model = model
        };
    }
}