using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmark.OrderBook.Entities;
/*
 * 好处似乎并不明显?
 */

public interface IOrderBook
{
    public OrderBookType OrderBookType { get; }

    public OrderBookSnapshot GetSnapshot();
}

public interface IAMDOrderBook : IOrderBook
{
    public void Add(OrderBookSide side, int position, decimal price, decimal quantity);

    public void Modify(OrderBookSide side, int position, decimal price, decimal quantity);

    public void Delete(OrderBookSide side, int position);
}

public interface IRBPOrderBook : IOrderBook
{
    public void Replace();
}

public interface IUBPOrderBook : IOrderBook
{
    public void Update(OrderBookSide side, IEnumerable<OrderBookLevel> levels);

    public void Update(OrderBookSide side, decimal price, decimal quantity);
}

public sealed class AMDOrderBook : IAMDOrderBook
{
    private readonly SortedList<int, OrderBookLevel> _bids = new();
    private readonly SortedList<int, OrderBookLevel> _asks = new();

    public OrderBookType OrderBookType => OrderBookType.AMD;

    public OrderBookSnapshot GetSnapshot()
    {
        var bids = _bids.Values.ToArray();
        var asks = _asks.Values.ToArray();

        return new OrderBookSnapshot(bids, asks);
    }

    public void Add(OrderBookSide side, int position, decimal price, decimal quantity)
    {
        switch (side)
        {
            case OrderBookSide.Bid:
                _bids.Add(position, new OrderBookLevel(price, quantity));
                break;

            case OrderBookSide.Ask:
                _asks.Add(position, new OrderBookLevel(price, quantity));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public void Modify(OrderBookSide side, int position, decimal price, decimal quantity)
    {
        switch (side)
        {
            case OrderBookSide.Bid:
                _bids[position] = new OrderBookLevel(price, quantity);
                break;

            case OrderBookSide.Ask:
                _asks[position] = new OrderBookLevel(price, quantity);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public void Delete(OrderBookSide side, int position)
    {
        switch (side)
        {
            case OrderBookSide.Bid:
                _bids.Remove(position);
                break;

            case OrderBookSide.Ask:
                _asks.Remove(position);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }
}

public sealed class RBPOrderBook : IRBPOrderBook
{
    public OrderBookType OrderBookType => OrderBookType.RBP;

    public OrderBookSnapshot GetSnapshot()
    {
        throw new NotImplementedException();
    }

    public void Replace()
    {
        throw new NotImplementedException();
    }
}

public sealed class UBPOrderBook : IUBPOrderBook
{
    public OrderBookType OrderBookType => OrderBookType.UBP;

    public OrderBookSnapshot GetSnapshot()
    {
        throw new NotImplementedException();
    }

    public void Update(OrderBookSide side, IEnumerable<OrderBookLevel> levels)
    {
        throw new NotImplementedException();
    }

    public void Update(OrderBookSide side, decimal price, decimal quantity)
    {
        throw new NotImplementedException();
    }
}

public interface IOrderBookFactory
{
    public IAMDOrderBook GetAMDOrderBook();

    public IRBPOrderBook GetRBPOrderBook();

    public IUBPOrderBook GetUBPOrderBook();
}

public enum OrderBookType
{
    /// <summary>
    /// Add Modify Delete
    /// </summary>
    AMD,

    /// <summary>
    /// Replace By Position
    /// </summary>
    RBP,

    /// <summary>
    /// Update By Price
    /// </summary>
    UBP
}

public enum OrderBookSide
{
    Bid,
    Ask
}

public readonly record struct OrderBookLevel(decimal Price, decimal Quantity);

public sealed class OrderBookSnapshot(IReadOnlyList<OrderBookLevel> bids, IReadOnlyList<OrderBookLevel> asks)
{
    public IReadOnlyList<OrderBookLevel> Bids { get; } = bids;

    public IReadOnlyList<OrderBookLevel> Asks { get; } = asks;
}