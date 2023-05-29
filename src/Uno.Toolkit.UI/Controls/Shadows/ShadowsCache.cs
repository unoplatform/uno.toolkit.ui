using System;
using System.Collections.Concurrent;

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

	private readonly ConcurrentDictionary<string, CacheBucket> _shadowsCache = new ConcurrentDictionary<string, CacheBucket>();

	public void AddOrUpdate(string key, SKImage image)
	{
		System.Diagnostics.Debug.WriteLine($"ShadowsCache => AddOrUpdate key: {key}");

		var bucket = _shadowsCache.AddOrUpdate(
			key,
			(key) =>
			{
				System.Diagnostics.Debug.WriteLine($"ShadowsCache => inserting new shadow in cache");
				return new CacheBucket(image);
			},
			(key, existing) =>
			{
				existing.AddHit();
				System.Diagnostics.Debug.WriteLine($"ShadowsCache => shadow image found, {existing.Hit} hits, last one at {existing.LastHit:T}");
				return existing;
			});
	}

	public bool TryGetValue(string key, out SKImage? image)
	{
		System.Diagnostics.Debug.WriteLine($"ShadowsCache => TryGet key: {key}");

		if (_shadowsCache.TryGetValue(key, out var bucket))
		{
			bucket.AddHit();
			image = bucket.Bitmap;
			System.Diagnostics.Debug.WriteLine($"ShadowsCache => shadow image found, {bucket.Hit} hits, last one at {bucket.LastHit:T}");
			return true;
		}

		image = null;
		return false;
	}

	public bool Remove(string key)
	{
		System.Diagnostics.Debug.WriteLine($"ShadowsCache => Remove key: {key}");

		return _shadowsCache.TryRemove(key, out var _);
	}
}
