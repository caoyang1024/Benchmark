using System;

namespace Benchmark.OrderBook;

public class ArrayOrderBook(int levels)
{
    private readonly OrderBookLevel[] _bids = new OrderBookLevel[levels];
    private readonly OrderBookLevel[] _asks = new OrderBookLevel[levels];

    private int _bidsCount;
    private int _asksCount;

    public ReadOnlySpan<OrderBookLevel> Bids => _bids.AsSpan(0, _bidsCount);
    public ReadOnlySpan<OrderBookLevel> Asks => _asks.AsSpan(0, _asksCount);

    public void Update(OrderBookSide side, params OrderBookLevel[] levels)
    {
        foreach (var level in levels)
        {
            Update(side, level);
        }
    }

    public void Update(OrderBookSide side, OrderBookLevel level)
    {
        switch (side)
        {
            case OrderBookSide.Bid:
                UpdateLevel(_bids, ref _bidsCount, level, descending: true);
                break;

            case OrderBookSide.Ask:
                UpdateLevel(_asks, ref _asksCount, level, descending: false);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    private void UpdateLevel(OrderBookLevel[] arr, ref int count, OrderBookLevel level, bool descending)
    {
        int index = Array.BinarySearch(arr, 0, count, level, new OrderBookLevelComparer(descending));

        if (index >= 0)
        {
            if (level.Quantity == 0)
            {
                // Delete
                for (int i = index; i < count - 1; i++)
                {
                    arr[i] = arr[i + 1];
                }
                count--;
            }
            else
            {
                // Update
                arr[index] = level;
            }
        }
        else
        {
            if (level.Quantity > 0)
            {
                // Insert
                index = ~index; // Get the insertion point
                for (int i = count; i > index; i--)
                {
                    arr[i] = arr[i - 1];
                }
                arr[index] = level;
                count++;
            }
        }
    }
}