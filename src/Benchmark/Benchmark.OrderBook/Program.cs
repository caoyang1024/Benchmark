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
    public const int Levels = 20;
    public const decimal Mid = 999.99m;

    [Benchmark]
    public void TestArrayOrderBook()
    {
        ArrayOrderBook orderBook = new(Levels);
        decimal shift = 0.5m;

        for (int i = 0; i < Levels; i++)
        {
            orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - shift, decimal.One));
            orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + shift, decimal.One));

            shift += 0.5m;
        }

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 1.0m, 0.0m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 1.0m, 0.0m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 15.0m, 0.1m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 15.0m, 0.1m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 3.0m, 0.0m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 3.0m, 0.0m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 16.0m, 0.1m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 16.0m, 0.1m));

        //foreach (var (price, quantity) in orderBook.Asks)
        //{
        //    Console.WriteLine($"\t\t\t\t{price}\t\t\t{quantity}");
        //}
        //foreach (var (price, quantity) in orderBook.Bids)
        //{
        //    Console.WriteLine($"{price}\t\t\t{quantity}");
        //}
    }

    [Benchmark]
    public void TestSortedListOrderBook()
    {
        SortedListOrderBook orderBook = new();
        decimal shift = 0.5m;

        for (int i = 0; i < Levels; i++)
        {
            orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - shift, decimal.One));
            orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + shift, decimal.One));

            shift += 0.5m;
        }

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 1.0m, 0.0m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 1.0m, 0.0m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 15.0m, 0.1m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 15.0m, 0.1m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 3.0m, 0.0m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 3.0m, 0.0m));

        orderBook.Update(OrderBookSide.Bid, new OrderBookLevel(Mid - 16.0m, 0.1m));
        orderBook.Update(OrderBookSide.Ask, new OrderBookLevel(Mid + 16.0m, 0.1m));

        //foreach (var (price, quantity) in orderBook.Asks)
        //{
        //    Console.WriteLine($"\t\t\t\t{price}\t\t\t{quantity}");
        //}
        //foreach (var (price, quantity) in orderBook.Bids)
        //{
        //    Console.WriteLine($"{price}\t\t\t{quantity}");
        //}
    }
}