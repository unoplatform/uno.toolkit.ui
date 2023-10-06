using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Uno.Extensions;
using Uno.Logging;

namespace Uno.Toolkit.UI;

public class ShadowsCache
{
	private static readonly ILogger _logger = typeof(ShadowsCache).Log();

	private readonly ConcurrentDictionary<string, CacheBucket> _shadowsCache = new ConcurrentDictionary<string, CacheBucket>();

	public void AddOrUpdate(string key, SKImage image)
	{
		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowsCache] AddOrUpdate => key: {key}");
		}

		var bucket = _shadowsCache.AddOrUpdate(
			key,
			(key) =>
			{
				if (_logger.IsEnabled(LogLevel.Trace))
				{
					_logger.Trace($"[ShadowsCache] inserting new shadow in cache");
				}
				return new CacheBucket(image);
			},
			(key, existing) =>
			{
				existing.AddHit();
				if (_logger.IsEnabled(LogLevel.Trace))
				{
					_logger.Trace($"[ShadowsCache] shadow image found, {existing.Hit} hits, last one at {existing.LastHit:T}");
				}
				return existing;
			});
	}

	public bool TryGetValue(string key, out SKImage? image)
	{
		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowsCache] TryGet => key: {key}");
		}
		if (_shadowsCache.TryGetValue(key, out var bucket))
		{
			bucket.AddHit();
			image = bucket.Bitmap;
			if (_logger.IsEnabled(LogLevel.Trace))
			{
				_logger.Trace($"[ShadowsCache] shadow image found, {bucket.Hit} hits, last one at {bucket.LastHit:T}");
			}
			return true;
		}

		image = null;
		return false;
	}

	public bool Remove(string key)
	{
		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowsCache] Remove => key: {key}");
		}

		return _shadowsCache.TryRemove(key, out var _);
	}

	internal void Clear()
	{
		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace("Cleared [ShadowsCache]");
		}

		_shadowsCache.Clear();
	}

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
}
