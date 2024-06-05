using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.OrderBook;

public class Program
{
    public static void Main(string[] args)
    {
        //OrderBookTest test = new();
        //test.TestArrayOrderBook();
        //test.TestSortedListOrderBook();

        BenchmarkDotNet.Running.BenchmarkRunner.Run<OrderBookTest>();
    }
}

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class OrderBookTest
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
    public void TestArrayOrderBookInsert()
    {
        ArrayOrderBook orderBook = new(Levels);

        for (int i = 0; i < Levels; i++)
        {
            orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Bids[i], decimal.One));
            orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Asks[i], decimal.One));
        }
    }

    [Benchmark]
    public void TestSortedListOrderBookInsert()
    {
        SortedListOrderBook orderBook = new();

        for (int i = 0; i < Levels; i++)
        {
            orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Bids[i], decimal.One));
            orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Asks[i], decimal.One));
        }
    }
}