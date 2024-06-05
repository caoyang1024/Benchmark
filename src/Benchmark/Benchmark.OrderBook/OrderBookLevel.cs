using System;
using System.Collections.Generic;

namespace Benchmark.OrderBook;

public record struct OrderBookLevel(decimal Price, decimal Quantity) : IComparable<OrderBookLevel>
{
    public decimal Price { get; } = Price;

    public readonly int CompareTo(OrderBookLevel other)
    {
        return Price.CompareTo(other.Price);
    }
}

public class OrderBookLevelComparer(bool descending) : IComparer<OrderBookLevel>
{
    public int Compare(OrderBookLevel x, OrderBookLevel y)
    {
        return descending ? y.Price.CompareTo(x.Price) : x.Price.CompareTo(y.Price);
    }
}