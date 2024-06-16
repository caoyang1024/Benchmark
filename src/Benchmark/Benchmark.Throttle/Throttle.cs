using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Benchmark.Throttle;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public sealed class Throttle(TimeSpan window, int limit, IDateTimeProvider dateTimeProvider)
    : IDisposable
{
    private readonly TimeSpan _window = TimeSpan.FromTicks(window.Ticks / limit);
    private readonly ConcurrentDictionary<string, (object, Queue<DateTimeOffset>)> _lockObjects = new();
    private const string DEFAULT_THROTTLE_KEY = "";

    public static Throttle Create(int limit)
    {
        return Create(TimeSpan.FromSeconds(1), limit);
    }

    public static Throttle Create(TimeSpan window, int limit)
    {
        return new Throttle(window, limit, new SystemDateTimeProvider());
    }

    public bool TryAcquire(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key can not be null or empty", nameof(key));
        }

        return TryAcquireInternal(key);
    }

    public bool TryAcquire()
    {
        return TryAcquireInternal(DEFAULT_THROTTLE_KEY);
    }

    private bool TryAcquireInternal(string key)
    {
        var now = dateTimeProvider.UtcNow;
        (object lockObject, Queue<DateTimeOffset> timeStamps) = _lockObjects.GetOrAdd(key, _ => (new object(), new Queue<DateTimeOffset>()));

        lock (lockObject)
        {
            // Remove expired timestamps
            while (timeStamps.Count > 0 && timeStamps.Peek() <= now - _window)
            {
                timeStamps.Dequeue();
            }

            // Check rate limit
            if (timeStamps.Count > 0)
            {
                return false;
            }

            // Add new timestamp
            timeStamps.Enqueue(now);
        }

        return true;
    }

    public void Dispose()
    {
        _lockObjects.Clear();
    }
}

public sealed class KeyedThrottle
{
    private readonly ConcurrentDictionary<TimeSpan, Throttle> _throttles = new();
    private readonly ConcurrentDictionary<string, Throttle> _keyedThrottles = new();

    public void AddOrUpdate(string key, TimeSpan window, int limit)
    {
        var rate = TimeSpan.FromTicks(window.Ticks / limit);

        Throttle throttle = _throttles.GetOrAdd(rate, _ => Throttle.Create(window, limit));

        _keyedThrottles.TryAdd(key, throttle);
    }

    public bool TryAcquire(string key)
    {
        if (_keyedThrottles.TryGetValue(key, out var throttle))
        {
            return throttle.TryAcquire(key);
        }

        throw new InvalidOperationException($"{key} key not found");
    }
}