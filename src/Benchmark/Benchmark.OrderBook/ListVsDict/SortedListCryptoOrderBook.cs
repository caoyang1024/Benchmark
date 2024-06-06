using System;
using System.Collections.Generic;
using Benchmark.OrderBook.Entities;

namespace Benchmark.OrderBook.ListVsDict;

public class SortedListCryptoOrderBook
{
    private readonly SortedList<decimal, OrderBookLevel> _bids = new();
    private readonly SortedList<decimal, OrderBookLevel> _asks = new();

    public void Update(OrderBookSide side, decimal price, decimal quantity)
    {
        switch (side)
        {
            case OrderBookSide.Bid:

                if (quantity == decimal.Zero)
                {
                    _bids.Remove(price);
                }
                else
                {
                    _bids[price] = new OrderBookLevel(price, quantity);
                }

                break;

            case OrderBookSide.Ask:

                if (quantity == decimal.Zero)
                {
                    _asks.Remove(price);
                }
                else
                {
                    _asks[price] = new OrderBookLevel(price, quantity);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public void Update(OrderBookSide side, params OrderBookLevel[] levels)
    {
        foreach (var level in levels)
        {
            Update(side, level.Price, level.Quantity);
        }
    }

    public OrderBookSnapshot GetSnapshot()
    {
        return new OrderBookSnapshot(_bids.Values.AsReadOnly(), _asks.Values.AsReadOnly());
    }
}