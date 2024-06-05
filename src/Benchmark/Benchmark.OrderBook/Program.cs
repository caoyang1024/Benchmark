using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.OrderBook;

public class Program
{
    public static void Main(string[] args)
    {
        OrderBookTest test = new();

        test.Setup();

        test.TestArrayOrderBookInsert();
        test.TestArrayOrderBookUpdate();
        var arr = test.TestArrayOrderBookDelete();

        test.TestSortedListOrderBookInsert();
        test.TestSortedListOrderBookUpdate();
        var sl = test.TestSortedListOrderBookDelete();

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

    private readonly ArrayOrderBook _arrayOrderBook = new(Levels);
    private readonly SortedListOrderBook _sortedListOrderBook = new();

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
        for (int i = 0; i < Levels; i++)
        {
            _arrayOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Bids[i], decimal.One));
            _arrayOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Asks[i], decimal.One));
        }
    }

    [Benchmark]
    public void TestSortedListOrderBookInsert()
    {
        for (int i = 0; i < Levels; i++)
        {
            _sortedListOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Bids[i], decimal.One));
            _sortedListOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Asks[i], decimal.One));
        }
    }

    [Benchmark]
    public void TestArrayOrderBookUpdate()
    {
        for (int i = 0; i < Levels; i++)
        {
            var b = Bids[i];

            _arrayOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(b, 5.9m));

            var a = Asks[i];

            _arrayOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(a, 51.9m));
        }
    }

    [Benchmark]
    public void TestSortedListOrderBookUpdate()
    {
        for (int i = 0; i < Levels; i++)
        {
            var b = Bids[i];

            _sortedListOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(b, 5.9m));

            var a = Asks[i];

            _sortedListOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(a, 51.9m));
        }
    }

    [Benchmark]
    public ArrayOrderBook TestArrayOrderBookDelete()
    {
        for (int i = 0; i < Levels; i++)
        {
            if (i % 2 == 0)
            {
                var b = Bids[i];

                _arrayOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(b, 0.0m));

                var a = Asks[i];

                _arrayOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(a, 0.0m));
            }
        }

        return _arrayOrderBook;
    }

    [Benchmark]
    public SortedListOrderBook TestSortedListOrderBookDelete()
    {
        for (int i = 0; i < Levels; i++)
        {
            if (i % 2 == 0)
            {
                var b = Bids[i];

                _sortedListOrderBook.Update(OrderBookSide.Bid, new OrderBookLevel(b, 0.0m));

                var a = Asks[i];

                _sortedListOrderBook.Update(OrderBookSide.Ask, new OrderBookLevel(a, 0.0m));
            }
        }

        return _sortedListOrderBook;
    }
}