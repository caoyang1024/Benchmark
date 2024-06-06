using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.OrderBook.ListVsDict;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class Benchmark1
{
    public const int Levels = 200;
    public const int Low = 100;
    public const int High = 10000;
    public const int Mid = (Low + High) / 2;

    public decimal[] Bids = new decimal[Levels];
    public decimal[] Asks = new decimal[Levels];

    [GlobalSetup]
    public void Setup()
    {
        for (int i = 0; i < Levels; i++)
        {
            Bids[i] = Random.Shared.Next(Low, Mid);
            Asks[i] = Random.Shared.Next(Mid, High);
        }
    }

    [Benchmark]
    public void TestSortedListOrderBook()
    {
        SortedListCryptoOrderBook sortedListCryptoOrderBook = new();

        for (int i = 0; i < Levels; i++)
        {
            sortedListCryptoOrderBook.Update(OrderBookSide.Bid, Bids[i], decimal.One);
            sortedListCryptoOrderBook.Update(OrderBookSide.Ask, Asks[i], decimal.One);
        }

        var snapshot = sortedListCryptoOrderBook.GetSnapshot();
    }

    [Benchmark]
    public void TestSortedDictOrderBook()
    {
        SortedDictCryptoOrderBook sortedDictCryptoOrderBook = new();

        for (int i = 0; i < Levels; i++)
        {
            sortedDictCryptoOrderBook.Update(OrderBookSide.Bid, Bids[i], decimal.One);
            sortedDictCryptoOrderBook.Update(OrderBookSide.Ask, Asks[i], decimal.One);
        }

        var snapshot = sortedDictCryptoOrderBook.GetSnapshot();
    }
}