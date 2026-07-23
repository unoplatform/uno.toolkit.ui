using System;
using Windows.Foundation;

namespace Uno.Toolkit.UI;

/// <summary>
/// Creates event-handler delegates that reference their target through a <see cref="WeakReference{T}"/>,
/// so a process-lifetime or otherwise global event source cannot strongly root the target (and, through
/// it, its host graph — and, across a collectible <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
/// boundary, a previewed app's ALC). Once the target has been collected the wrapper detaches itself, via
/// <c>detach</c>, the next time the event is raised.
/// </summary>
/// <remarks>
/// <c>onEvent</c> and <c>detach</c> MUST be static / non-capturing delegates so the returned handler
/// captures only the <see cref="WeakReference{T}"/> — never the target instance. A capturing lambda would
/// defeat the purpose by rooting its closure (and whatever it captured) through the returned delegate.
/// </remarks>
internal static class WeakEventHelper
{
	public static EventHandler<TArgs> CreateWeakHandler<TTarget, TArgs>(
		TTarget target,
		Action<TTarget, object?, TArgs> onEvent,
		Action<EventHandler<TArgs>> detach)
		where TTarget : class
	{
		AssertStatic(onEvent, detach);

		var weakTarget = new WeakReference<TTarget>(target);
		EventHandler<TArgs> h = null!;
		h = (s, e) =>
		{
			if (weakTarget.TryGetTarget(out var self))
			{
				onEvent(self, s, e);
			}
			else
			{
				detach(h);
			}
		};
		return h;
	}

	public static TypedEventHandler<TSender, TArgs> CreateWeakHandler<TTarget, TSender, TArgs>(
		TTarget target,
		Action<TTarget, TSender, TArgs> onEvent,
		Action<TSender, TypedEventHandler<TSender, TArgs>> detach)
		where TTarget : class
		where TSender : class
	{
		AssertStatic(onEvent, detach);

		var weakTarget = new WeakReference<TTarget>(target);
		TypedEventHandler<TSender, TArgs> h = null!;
		h = (s, e) =>
		{
			if (weakTarget.TryGetTarget(out var self))
			{
				onEvent(self, s, e);
			}
			else
			{
				detach(s, h);
			}
		};
		return h;
	}

	private static void AssertStatic(Delegate onEvent, Delegate detach)
	{
		// The whole point of the wrapper is that the event source holds only a WeakReference back. That
		// guarantee is defeated if a caller passes a capturing lambda: its closure display class would be
		// rooted by the returned delegate (Target != null) and would in turn root whatever it captured.
		// Fully qualified: on net*-android an unqualified `Debug` binds to the inherited
		// Android.Views.ViewGroup.Debug(int) method (CS0119), not System.Diagnostics.Debug.
		System.Diagnostics.Debug.Assert(onEvent.Target is null, "CreateWeakHandler: onEvent must be a static/non-capturing delegate, otherwise it reintroduces a strong reference.");
		System.Diagnostics.Debug.Assert(detach.Target is null, "CreateWeakHandler: detach must be a static/non-capturing delegate, otherwise it reintroduces a strong reference.");
	}
}
