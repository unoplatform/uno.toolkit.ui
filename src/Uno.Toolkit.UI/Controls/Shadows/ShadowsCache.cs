using System;
using System.Collections.Generic;

using SkiaSharp;

namespace Uno.Toolkit.UI;

public class ShadowsCache
{
    private class CacheBucket
    {
        public CacheBucket(SKImage bitmap)
        {
            Bitmap = bitmap;
            AddHit();
        }

        public SKImage Bitmap { get; }

        public int Hit { get; private set; }

        public DateTime LastHit { get; private set; }

        public void AddHit()
        {
            Hit++;
            LastHit = DateTime.UtcNow;
        }
    }

    private readonly Dictionary<string, CacheBucket> _shadowsCache = new Dictionary<string, CacheBucket>();

    private readonly object _syncRoot = new object();

    public void AddOrUpdate(string key, SKImage image)
    {
        System.Diagnostics.Debug.WriteLine($"ShadowsCache => AddOrUpdate key: {key}");
        lock (_syncRoot)
        {
            if (_shadowsCache.TryGetValue(key, out var bucket))
            {
                bucket.AddHit();
                System.Diagnostics.Debug.WriteLine($"ShadowsCache => shadow image found, {bucket.Hit} hits, last one at {bucket.LastHit:T}");
                return;
            }

            _shadowsCache.Add(key, new CacheBucket(image));
            System.Diagnostics.Debug.WriteLine($"ShadowsCache => inserting new shadow in cache");
        }
    }

    public bool TryGet(string key, out SKImage? image)
    {
        System.Diagnostics.Debug.WriteLine($"ShadowsCache => TryGet key: {key}");
        lock (_syncRoot)
        {
            if (_shadowsCache.TryGetValue(key, out var bucket))
            {
                bucket.AddHit();
                image = bucket.Bitmap;
                System.Diagnostics.Debug.WriteLine($"ShadowsCache => shadow image found, {bucket.Hit} hits, last one at {bucket.LastHit:T}");
                return true;
            }
        }

        image = null;
        return false;
    }

	public bool Remove(string key)
	{
		System.Diagnostics.Debug.WriteLine($"ShadowsCache => Remove key: {key}");
		lock (_syncRoot)
		{
			return _shadowsCache.Remove(key);
		}

	}
}
