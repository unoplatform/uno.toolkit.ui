#if DEBUG // Hot-reload tests are only relevant in debug configuration
using System.Reflection;
using System.Reflection.Metadata;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.UI.RuntimeTests;

[assembly: MetadataUpdateHandler(typeof(HotReloadTestHelper.UpdateTracker))]

namespace Uno.Toolkit.RuntimeTests.Helpers;

/// <summary>
/// Wraps <see cref="HotReloadHelper"/> file edits so a test only resumes once the delta for the edited
/// file's assembly has been applied, both after the edit and after its revert.
/// </summary>
/// <remarks>
/// The dev-server applies one edit as one delta per updated assembly, including the app head's generated
/// <c>uno.hot-reload.info</c> file. <see cref="HotReloadHelper"/> resumes on the first metadata update, which can
/// be the head's, before the edited page has been replaced in the visual tree.
/// </remarks>
internal static class HotReloadTestHelper
{
	public static async ValueTask<IAsyncDisposable> UpdateSourceFile<T>(string originalText, string replacementText, CancellationToken ct = default)
		where T : FrameworkElement, new()
	{
		var assembly = typeof(T).Assembly;
		var baseline = UpdateTracker.GetUpdateCount(assembly);
		var revert = await HotReloadHelper.UpdateSourceFile<T>(originalText, replacementText, ct);
		await WaitForUpdate(assembly, baseline, ct);

		return new RevertAndWait(revert, assembly, ct);
	}

	/// <param name="filePathRelativeToProject">A file of Uno.Toolkit.RuntimeTests, relative to the app head project.</param>
	public static async ValueTask<IAsyncDisposable> UpdateSourceFile(string filePathRelativeToProject, string originalText, string replacementText, CancellationToken ct = default)
	{
		var assembly = typeof(HotReloadTestHelper).Assembly;
		var baseline = UpdateTracker.GetUpdateCount(assembly);
		var revert = await HotReloadHelper.UpdateSourceFile(filePathRelativeToProject, originalText, replacementText, ct);
		await WaitForUpdate(assembly, baseline, ct);

		return new RevertAndWait(revert, assembly, ct);
	}

	private static async Task WaitForUpdate(Assembly assembly, int baseline, CancellationToken ct)
	{
		using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
		timeout.CancelAfter(HotReloadHelper.DefaultMetadataUpdateTimeout);

		try
		{
			while (UpdateTracker.GetUpdateCount(assembly) == baseline)
			{
				await Task.Delay(50, timeout.Token);
			}
		}
		catch (OperationCanceledException) when (!ct.IsCancellationRequested)
		{
			throw new TimeoutException($"No hot-reload delta was applied to '{assembly.GetName().Name}' within {HotReloadHelper.DefaultMetadataUpdateTimeout}.");
		}

		// The visual tree is updated on the UI thread once the delta is applied.
		await UnitTestsUIContentHelper.WaitForIdle();
	}

	private sealed class RevertAndWait(IAsyncDisposable revert, Assembly assembly, CancellationToken ct) : IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			var baseline = UpdateTracker.GetUpdateCount(assembly);
			await revert.DisposeAsync();
			await WaitForUpdate(assembly, baseline, ct);
		}
	}

	internal static class UpdateTracker
	{
		private static readonly object _gate = new();
		private static readonly Dictionary<Assembly, int> _updateCounts = new();

		public static int GetUpdateCount(Assembly assembly)
		{
			lock (_gate)
			{
				return _updateCounts.GetValueOrDefault(assembly);
			}
		}

		// Called by the runtime for every applied delta (MetadataUpdateHandlerAttribute).
		internal static void UpdateApplication(Type[]? updatedTypes)
		{
			if (updatedTypes is null)
			{
				return;
			}

			lock (_gate)
			{
				foreach (var assembly in updatedTypes.Select(t => t.Assembly).Distinct())
				{
					_updateCounts[assembly] = _updateCounts.GetValueOrDefault(assembly) + 1;
				}
			}
		}
	}
}
#endif
