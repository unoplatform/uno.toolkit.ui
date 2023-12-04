﻿using System.Collections.Generic;
using System;
using Windows.Foundation;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif

namespace Uno.Toolkit.UI;

internal interface IResponsiveCallback
{
	void OnSizeChanged(Size size, ResponsiveLayout layout);
}

public partial class ResponsiveLayout : DependencyObject
{
	#region DependencyProperty: Narrowest

	public static DependencyProperty NarrowestProperty { get; } = DependencyProperty.Register(
		nameof(Narrowest),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Narrowest
	{
		get => (double)GetValue(NarrowestProperty);
		set => SetValue(NarrowestProperty, value);
	}

	#endregion
	#region DependencyProperty: Narrow

	public static DependencyProperty NarrowProperty { get; } = DependencyProperty.Register(
		nameof(Narrow),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Narrow
	{
		get => (double)GetValue(NarrowProperty);
		set => SetValue(NarrowProperty, value);
	}

	#endregion
	#region DependencyProperty: Normal

	public static DependencyProperty NormalProperty { get; } = DependencyProperty.Register(
		nameof(Normal),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Normal
	{
		get => (double)GetValue(NormalProperty);
		set => SetValue(NormalProperty, value);
	}

	#endregion
	#region DependencyProperty: Wide

	public static DependencyProperty WideProperty { get; } = DependencyProperty.Register(
		nameof(Wide),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Wide
	{
		get => (double)GetValue(WideProperty);
		set => SetValue(WideProperty, value);
	}

	#endregion
	#region DependencyProperty: Widest

	public static DependencyProperty WidestProperty { get; } = DependencyProperty.Register(
		nameof(Widest),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Widest
	{
		get => (double)GetValue(WidestProperty);
		set => SetValue(WidestProperty, value);
	}

	#endregion

	public static ResponsiveLayout Create(double narrowest, double narrow, double normal, double wide, double widest) => new()
	{
		Narrowest = narrowest,
		Narrow = narrow,
		Normal = normal,
		Wide = wide,
		Widest = widest,
	};
}

internal class ResponsiveHelper
{
	private static readonly Lazy<ResponsiveHelper> _instance = new Lazy<ResponsiveHelper>(() => new ResponsiveHelper());
	private readonly List<WeakReference> _references = new();
	private static readonly ResponsiveHelper _debugInstance = new();
	private static bool UseDebuggableInstance;

	public ResponsiveLayout Layout { get; private set; } = ResponsiveLayout.Create(150, 300, 600, 800, 1080);
	public Size WindowSize { get; private set; } = Size.Empty;

	public static ResponsiveHelper GetForCurrentView() => UseDebuggableInstance ? _debugInstance : _instance.Value;

	private ResponsiveHelper() { }

	public void HookupEvent(Window window)
	{
		WindowSize = new Size(window.Bounds.Width, window.Bounds.Height);

		window.SizeChanged += OnWindowSizeChanged;
	}

	internal static void SetDebugSize(Size size) => _debugInstance.OnWindowSizeChanged(size);
	private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e) => OnWindowSizeChanged(e.Size);

	private void OnWindowSizeChanged(Size size)
	{
		WindowSize = size;

		_references.RemoveAll(reference => !reference.IsAlive);

		foreach (var reference in _references.ToArray())
		{
			if (reference.IsAlive && reference.Target is IResponsiveCallback callback)
			{
				callback.OnSizeChanged(WindowSize, Layout);
			}
		}
	}

	internal void Register(IResponsiveCallback host)
	{
		var wr = new WeakReference(host);
		_references.Add(wr);
	}

	internal static IDisposable UsingDebuggableInstance()
	{
		UseDebuggableInstance = true;

		return Disposable.Create(() => UseDebuggableInstance = false);
	}
}
