using Microsoft.Extensions.Logging;

namespace Uno.Toolkit.WrapperApp.GuestHosting;

/// <summary>
/// Self-driving hosting verification (launch with <c>--smoke</c> on desktop or <c>?smoke</c>
/// in the browser): loads and reloads every guest, verifies failed requests preserve the active
/// app, and checks that unloaded guest ALCs are actually reclaimed. On desktop the process exits with
/// <c>0</c> (pass) / <c>1</c> (fail) so CI can gate on it; in the browser the verdict is
/// logged as <c>[HOSTING-SMOKE] RESULT: …</c> for a driving harness to scrape.
/// </summary>
internal static class GuestHostingSmoke
{
	private const string SmokeFlagName = "smoke";

	// ALC reclamation is only deterministic on Release desktop builds: Debug JIT root
	// retention and residual WASM roots are documented, accepted limitations (see
	// specs/toolkit-wrapper/progress.md), so those configurations report reclamation
	// without failing on it.
#if DEBUG || __WASM__
	private static readonly bool _reclamationIsAuthoritative = false;
#else
	private static readonly bool _reclamationIsAuthoritative = true;
#endif

	private static readonly ILogger _logger =
		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.CreateLogger("Uno.Toolkit.WrapperApp.GuestHosting.GuestHostingSmoke");

	/// <summary>
	/// Gets whether the smoke was requested by a launch selector.
	/// </summary>
	public static bool IsRequested => GuestAppDeepLink.GetLaunchFlag(SmokeFlagName);

	/// <summary>
	/// Runs the load/reload/switch/failure/unload scenarios and returns the verdict. Never throws.
	/// </summary>
	public static async Task<bool> RunAsync(GuestAppLoader loader)
	{
		var passed = true;
		try
		{
			foreach (var app in GuestAppCatalog.Apps)
			{
				_logger.LogInformation("[HOSTING-SMOKE] Loading {App}…", app.DisplayName);
				await loader.LoadAsync(app);
				if (!await CheckHostedAppAsync(loader, app))
				{
					passed = false;
					break;
				}

				passed &= await CheckReclamationAsync(loader);

				if (ReferenceEquals(app, GuestAppCatalog.Apps[0]))
				{
					_logger.LogInformation("[HOSTING-SMOKE] Unloading the first guest before loading another app…");
					await loader.UnloadAsync();
					passed &= CheckUnloaded(loader);
					passed &= await CheckReclamationAsync(loader);

					_logger.LogInformation("[HOSTING-SMOKE] Loading {App} after explicit unload…", app.DisplayName);
					await loader.LoadAsync(app);
					passed &= await CheckHostedAppAsync(loader, app);
				}

				_logger.LogInformation("[HOSTING-SMOKE] Reloading {App}…", app.DisplayName);
				await loader.LoadAsync(app);
				passed &= await CheckHostedAppAsync(loader, app);
				passed &= await CheckReclamationAsync(loader);
			}

			if (loader.CurrentApp is { } activeApp)
			{
				passed &= await CheckMissingGuestAsync(loader, activeApp);
				passed &= await CheckCanceledLoadAsync(loader, activeApp);

				_logger.LogInformation("[HOSTING-SMOKE] Unloading the last guest…");
				await loader.UnloadAsync();
				passed &= CheckUnloaded(loader);
				passed &= await CheckReclamationAsync(loader);

				_logger.LogInformation("[HOSTING-SMOKE] Unloading again with no active guest…");
				await loader.UnloadAsync();
				passed &= CheckUnloaded(loader);
				passed &= await CheckReclamationAsync(loader);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[HOSTING-SMOKE] Failed with an exception.");
			passed = false;
		}

		_logger.LogInformation("[HOSTING-SMOKE] RESULT: {Result}", passed ? "PASS" : "FAIL");
		return passed;
	}

	/// <summary>
	/// Ends the smoke run: exits the process with the verdict on desktop; no-ops in the
	/// browser (the logged RESULT line is the verdict there).
	/// </summary>
	public static void Exit(bool passed)
	{
#if !__WASM__
		Environment.Exit(passed ? 0 : 1);
#endif
	}

	private static async Task<bool> CheckHostedAppAsync(GuestAppLoader loader, GuestAppInfo app)
	{
		if (!ReferenceEquals(loader.CurrentApp, app) || !await loader.VerifyHostedAppAsync())
		{
			_logger.LogError("[HOSTING-SMOKE] {App} did not present a working guest shell.", app.DisplayName);
			return false;
		}

		_logger.LogInformation("[HOSTING-SMOKE] {App} is hosted and its shell is ready.", app.DisplayName);
		return true;
	}

	private static async Task<bool> CheckMissingGuestAsync(GuestAppLoader loader, GuestAppInfo activeApp)
	{
		_logger.LogInformation("[HOSTING-SMOKE] Loading a missing guest must preserve the active app.");
		try
		{
			await loader.LoadAsync(new GuestAppInfo("Missing guest", "MissingGuestSampleApp", "MissingGuestSampleApp"));
		}
		catch (GuestAppLoadException)
		{
			return await CheckHostedAppAsync(loader, activeApp);
		}

		_logger.LogError("[HOSTING-SMOKE] Loading a missing guest did not fail.");
		return false;
	}

	private static async Task<bool> CheckCanceledLoadAsync(GuestAppLoader loader, GuestAppInfo activeApp)
	{
		_logger.LogInformation("[HOSTING-SMOKE] Canceling before load must preserve the active app.");
		using var cancellation = new CancellationTokenSource();
		cancellation.Cancel();
		try
		{
			await loader.LoadAsync(activeApp, cancellationToken: cancellation.Token);
		}
		catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
		{
			return await CheckHostedAppAsync(loader, activeApp);
		}

		_logger.LogError("[HOSTING-SMOKE] Loading with a canceled token did not cancel.");
		return false;
	}

	private static bool CheckUnloaded(GuestAppLoader loader)
	{
		if (loader.CurrentApp is not null)
		{
			_logger.LogError("[HOSTING-SMOKE] A guest remains active after unloading.");
			return false;
		}

		return true;
	}

	private static async Task<bool> CheckReclamationAsync(GuestAppLoader loader)
	{
		bool? collected = null;
		for (var attempt = 0; attempt < 5; attempt++)
		{
			collected = await loader.VerifyPreviousAlcCollectedAsync();
			if (collected != false)
			{
				break;
			}

			// Finalizer-driven unpinning can lag a collection pass, especially in the browser.
			await Task.Delay(200);
		}

		if (collected is null)
		{
			// Nothing has been unloaded yet (first load of the run).
			return true;
		}

		if (collected == true)
		{
			_logger.LogInformation("[HOSTING-SMOKE] Previous guest ALC reclaimed.");
			return true;
		}

		if (_reclamationIsAuthoritative)
		{
			_logger.LogError("[HOSTING-SMOKE] Previous guest ALC was NOT reclaimed.");
			return false;
		}

		_logger.LogWarning("[HOSTING-SMOKE] Previous guest ALC not reclaimed — expected on Debug/WASM (documented root retention), not failing the smoke.");
		return true;
	}
}
