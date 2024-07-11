using System;

namespace Benchmark.Serializer;

// 1000 bytes
public class LargeMessage : IMessage
{
    public int Id { get; set; }                 // 8
    public decimal Bid { get; set; }            // 16
    public decimal Ask { get; set; }            // 16
    public decimal BidVolume { get; set; }      // 16
    public decimal AskVolume { get; set; }      // 16
    public decimal BidPrice { get; set; }       // 16
    public decimal AskPrice { get; set; }       // 16

    public decimal Bid1 { get; set; }            // 16
    public decimal Ask1 { get; set; }            // 16
    public decimal BidVolume1 { get; set; }      // 16
    public decimal AskVolume1 { get; set; }      // 16
    public decimal BidPrice1 { get; set; }       // 16
    public decimal AskPrice1 { get; set; }       // 16

    public decimal Bid2 { get; set; }            // 16
    public decimal Ask2 { get; set; }            // 16
    public decimal BidVolume2 { get; set; }      // 16
    public decimal AskVolume2 { get; set; }      // 16
    public decimal BidPrice2 { get; set; }       // 16
    public decimal AskPrice2 { get; set; }       // 16

    public decimal Bid3 { get; set; }            // 16
    public decimal Ask3 { get; set; }            // 16
    public decimal BidVolume3 { get; set; }      // 16
    public decimal AskVolume3 { get; set; }      // 16
    public decimal BidPrice3 { get; set; }       // 16
    public decimal AskPrice3 { get; set; }       // 16

    public decimal Bid4 { get; set; }            // 16
    public decimal Ask4 { get; set; }            // 16
    public decimal BidVolume4 { get; set; }      // 16
    public decimal AskVolume4 { get; set; }      // 16
    public decimal BidPrice4 { get; set; }       // 16
    public decimal AskPrice4 { get; set; }       // 16

    public string Text { get; set; }            // ~10
    public Guid Guid { get; set; }              // 8
    public string Metadata { get; set; }        // ~10
}