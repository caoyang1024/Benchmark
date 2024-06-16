using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Benchmark.Throttle;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public sealed class Throttle(TimeSpan window, int limit, IDateTimeProvider dateTimeProvider)
    : IDisposable
{
    private readonly TimeSpan _window = TimeSpan.FromMilliseconds(window.TotalMilliseconds / limit);
    private readonly ConcurrentDictionary<string, (object, Queue<DateTime>)> _lockObjects = new();
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
        (object lockObject, Queue<DateTime> timeStamps) = _lockObjects.GetOrAdd(key, _ => (new object(), new Queue<DateTime>()));

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