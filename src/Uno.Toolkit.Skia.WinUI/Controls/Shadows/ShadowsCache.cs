using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SkiaSharp;
using Uno.Extensions;
using Uno.Logging;

namespace Uno.Toolkit.UI;
public class ShadowsCache
{
	private const int CleanupInterval = 30;

	private static readonly ILogger _logger = typeof(ShadowsCache).Log();

	private static readonly Predicate<CacheBucket> OneHitSinceAShortTime =
		(bucket) => bucket.Hit == 1 && (DateTime.UtcNow - bucket.LastHit).TotalMinutes > 1;

	private static readonly Predicate<CacheBucket> SeveralHitsSinceALongTime =
		(bucket) => bucket.Hit > 1 && (DateTime.UtcNow - bucket.LastHit).TotalMinutes > 3;

	private readonly ConcurrentDictionary<string, CacheBucket> _shadowsCache = new ConcurrentDictionary<string, CacheBucket>();

	private DateTime _lastCleanupTime = DateTime.UtcNow;

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

		CleanupIfNeeded();
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

	public async void CleanupIfNeeded()
	{
		if ((DateTime.UtcNow - _lastCleanupTime).TotalSeconds > CleanupInterval)
		{
			try
			{
				await CleanupAsync();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[ShadowsCache] Cleanup failed: {ex}");
			}
		}
	}

	/// <summary>
	/// Removing old shadows based on their stats.
	/// </summary>
	private Task CleanupAsync()
	{
		_lastCleanupTime = DateTime.UtcNow;

		return Task.Run(() =>
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			System.Diagnostics.Debug.WriteLine($"[ShadowsCache] Cleanup starting");
			int removedShadows = 0;
			foreach (var expiredBucket in _shadowsCache
				.Where(x => OneHitSinceAShortTime(x.Value) || SeveralHitsSinceALongTime(x.Value)))
			{
				if (_shadowsCache.TryRemove(expiredBucket.Key, out var _))
				{
					removedShadows++;
				}
			}

			stopwatch.Stop();
			System.Diagnostics.Debug.WriteLine($"[ShadowsCache] Cleanup done in {stopwatch.ElapsedMilliseconds} ms ({removedShadows} shadows removed)");
		});		
	}
}
