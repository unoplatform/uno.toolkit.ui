#if DEBUG && !__WASM__ && !__ANDROID__ && !__IOS__
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

using DependencyObjectExtensions = Uno.Toolkit.UI.DependencyObjectExtensions;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards that the process-lifetime dependency-property reflection cache does not strongly root its
/// owner <see cref="Type"/> keys. A Type from a collectible assembly (modelling a previewed app loaded
/// into a collectible AssemblyLoadContext) must remain collectible, otherwise the cache pins the Type's
/// LoaderAllocator — and the whole ALC — for the process lifetime.
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class DependencyObjectExtensionsLeakTests
{
	[TestMethod]
	public async Task ReflectionCache_DoesNotRoot_CollectibleTypeKey()
	{
		var typeRef = PopulateCacheFromCollectibleAssembly();

		await CollectAndWait();

		Assert.IsFalse(
			typeRef.IsAlive,
			"A Type from a collectible (RunAndCollect) assembly should be collectible after use; " +
			"if it survives, the reflection cache is strongly rooting the Type key. " +
			"A ConditionalWeakTable-keyed cache releases the entry with its Type; a Dictionary<Type,...> pins it.");
	}

	// Emits a collectible dynamic assembly, uses one of its Types as a cache key, then returns a weak
	// reference to that Type. The assembly/Type is only referenced from within this non-inlined frame so
	// it can be collected once we drop out of it (subject only to the cache not rooting it).
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference PopulateCacheFromCollectibleAssembly()
	{
		var asmName = new AssemblyName("Uno.Toolkit.RuntimeTests.Collectible." + Guid.NewGuid().ToString("N"));
		var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
		var moduleBuilder = asmBuilder.DefineDynamicModule("Main");
		var typeBuilder = moduleBuilder.DefineType("CollectibleOwner", TypeAttributes.Public);
		var owner = typeBuilder.CreateType()!;

		// Populate the cache for this collectible Type. The DP does not exist on it; that is fine — the
		// cache still records a (Type -> per-property dictionary) entry keyed by the collectible Type.
		_ = owner.FindDependencyProperty("SomeMissingProperty");

		Assert.IsTrue(
			DependencyObjectExtensions.TestHook_ReflectionCacheContains(owner),
			"The reflection cache should hold an entry for the Type immediately after lookup.");

		return new WeakReference(owner);
	}

	private static async Task CollectAndWait()
	{
		for (var i = 0; i < 8; i++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			await UnitTestUIContentHelperEx.WaitForIdle();
		}
	}
}
#endif
