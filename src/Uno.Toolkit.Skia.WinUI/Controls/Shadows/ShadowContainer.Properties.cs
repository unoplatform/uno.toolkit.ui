using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class ShadowContainer : ContentControl
{
	private readonly SerialDisposable _shadowsCollectionChanged = new();
	private readonly SerialDisposable _shadowPropertiesChanged = new();
	private readonly SerialDisposable _cornerRadiusChanged = new();
	private readonly CompositeDisposable _activeShadowRegistrations = new CompositeDisposable();

	#region DependencyProperty: Shadows

	public static readonly DependencyProperty ShadowsProperty =
		DependencyProperty.Register(
			nameof(Shadows),
			typeof(ShadowCollection),
			typeof(ShadowContainer),
			new(new ShadowCollection(), OnShadowsChanged));

	/// <summary>
	/// The collection of shadows that will be displayed under your control.
	/// A ShadowCollection can be stored in a resource dictionary to have a consistent style through your app.
	/// The ShadowCollection implements INotifyCollectionChanged.
	/// </summary>
	public ShadowCollection Shadows
	{
		get => (ShadowCollection)GetValue(ShadowsProperty);
		set => SetValue(ShadowsProperty, value);
	}

	#endregion

	// True if a shadow property has changed dynamically or if we add or removed a shadow
	private bool _shadowPropertyChanged;

	private static void OnShadowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ShadowContainer shadowContainer)
		{
			shadowContainer.UpdateShadows();
		}
	}

	private void OnShadowCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Add:
				for (int i = 0; i < e.NewItems!.Count; i++)
				{
					OnShadowInserted((Shadow)e.NewItems[i]!);
				}

				OnShadowSizeChanged();
				InvalidateFromShadowPropertyChange();

				break;
			case NotifyCollectionChangedAction.Remove:
				for (int i = 0; i < e.OldItems!.Count; i++)
				{
					OnShadowRemoved((Shadow)e.OldItems[i]!);
				}

				OnShadowSizeChanged();
				InvalidateFromShadowPropertyChange();
				break;

			case NotifyCollectionChangedAction.Reset:
				OnShadowSizeChanged();
				InvalidateFromShadowPropertyChange();
				break;
		}
	}

	private void UpdateShadows()
	{
		_shadowsCollectionChanged.Disposable = null;
		_shadowPropertiesChanged.Disposable = null;

		if (Shadows is not { } shadows)
		{
			return;
		}

		foreach (var shadow in shadows)
		{
			_activeShadowRegistrations.Add(() => shadow.PropertyChanged -= ShadowPropertyChanged);
			shadow.PropertyChanged += ShadowPropertyChanged;
		}

		_shadowsCollectionChanged.Disposable = Disposable.Create(() => shadows.CollectionChanged -= OnShadowCollectionChanged);
		shadows.CollectionChanged += OnShadowCollectionChanged;

		_shadowPropertiesChanged.Disposable = _activeShadowRegistrations;

		OnShadowSizeChanged();
		_shadowHost?.Invalidate();
	}

	private void OnShadowInserted(Shadow shadow)
	{
		_activeShadowRegistrations.Add(() => shadow.PropertyChanged -= ShadowPropertyChanged);
		shadow.PropertyChanged += ShadowPropertyChanged;
	}

	private void OnShadowRemoved(Shadow shadow)
	{
		shadow.PropertyChanged -= ShadowPropertyChanged;
	}

	private void ShadowPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is null)
		{
			return;
		}

		if (UI.Shadow.IsShadowSizeProperty(e.PropertyName))
		{
			OnShadowSizeChanged();
		}

		InvalidateFromShadowPropertyChange();
	}

	private void OnShadowSizeChanged()
	{
		if (_currentContent != null && _currentContent.ActualWidth > 0 && _currentContent.ActualHeight > 0)
		{
			UpdateCanvasSize(_currentContent.ActualWidth, _currentContent.ActualHeight, Shadows);
		}
	}

	private void InvalidateFromShadowPropertyChange()
	{
		_shadowPropertyChanged = true;
		_shadowHost?.Invalidate();
	}
}
