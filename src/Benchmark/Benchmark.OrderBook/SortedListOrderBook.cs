using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmark.OrderBook;

public class SortedListOrderBook
{
    private readonly SortedList<decimal, OrderBookLevel> _bids = new(new DecimalComparer());
    private readonly SortedList<decimal, OrderBookLevel> _asks = new();

    public ReadOnlySpan<OrderBookLevel> Bids => _bids.Values.ToArray();
    public ReadOnlySpan<OrderBookLevel> Asks => _asks.Values.ToArray();

    public void Update(OrderBookSide side, params OrderBookLevel[] levels)
    {
        foreach (var level in levels)
        {
            Update(side, level);
        }
    }

    public void Update(OrderBookSide side, OrderBookLevel level)
    {
        var list = side switch
        {
            OrderBookSide.Bid => _bids,
            OrderBookSide.Ask => _asks,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

        if (level.Quantity == decimal.Zero)
        {
            list.Remove(level.Price);
        }
        else if (list.TryGetValue(level.Price, out var price))
        {
            price.Quantity = level.Quantity;
        }
        else
        {
            list.Add(level.Price, level);
        }
    }
}

public sealed class DecimalComparer : IComparer<decimal>
{
    public int Compare(decimal x, decimal y)
    {
        return y.CompareTo(x);
    }
}