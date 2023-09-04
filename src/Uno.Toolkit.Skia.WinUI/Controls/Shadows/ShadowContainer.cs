using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SkiaSharp.Views.Windows;
using Uno.Disposables;

#if __ANDROID__
using Android.Views;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Provides the possibility to add many-colored shadows to its content.
/// </summary>
/// <remarks>
/// For now it renders badly on WASM due to a bug on the wasm skiasharp construction of the SKXamlCanvas.
/// It should be fixed when this PR will be merged: https://github.com/mono/SkiaSharp/pull/2443
/// </remarks>
[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(Canvas))]
public partial class ShadowContainer : ContentControl
{
	private const string PART_Canvas = "PART_Canvas";
	private const string PART_ShadowOwner = "PART_ShadowOwner";

	private static readonly ShadowsCache Cache = new ShadowsCache();

	private readonly SerialDisposable _eventSubscriptions = new();
	
	private Grid? _panel;
	private Canvas? _canvas;
	private SKXamlCanvas? _shadowHost;

	public ShadowContainer()
	{
		DefaultStyleKey = typeof(ShadowContainer);

		Shadows = new();

		Loaded += ShadowContainerLoaded;
		Unloaded += ShadowContainerUnloaded;
	}

	private void ShadowContainerLoaded(object sender, RoutedEventArgs e)
	{
		BindToPaintingProperties();

		InvalidateCanvasLayout();
		InvalidateShadows();
	}

	private void ShadowContainerUnloaded(object sender, RoutedEventArgs e)
	{
		_eventSubscriptions.Disposable = null;
	}

	private void BindToPaintingProperties()
	{
		var backgroundNestedDisposable = new SerialDisposable();
		var shadowsNestedDisposable = new SerialDisposable();
		var contentNestedDisposable = new SerialDisposable();

		_eventSubscriptions.Disposable = new CompositeDisposable
		{
			this.RegisterDisposablePropertyChangedCallback(BackgroundProperty, OnBackgroundChanged),
			this.RegisterDisposablePropertyChangedCallback(ShadowsProperty, OnShadowsChanged),
			this.RegisterDisposablePropertyChangedCallback(ContentProperty, OnContentChanged),

			backgroundNestedDisposable,
			shadowsNestedDisposable,
			contentNestedDisposable,
		};

		// manually proc inner registration once
		BindToBackgroundMemberProperties(Background);
		BindToShadowCollectionChanged(Shadows);
		BindToContent(Content);

		// This method should not fire any of InvalidateXyz-methods directly,
		// in order to avoid duplicated invalidate calls.
		// Which is why the BindToXyz has been separated from the OnXyzChanged.

		void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp)
		{
			BindToBackgroundMemberProperties(Background);

			InvalidateShadows();
		}
		void OnShadowsChanged(DependencyObject sender, DependencyProperty dp)
		{
			BindToShadowCollectionChanged(Shadows);

			InvalidateCanvasLayout();
			InvalidateShadows();
		}
		void OnContentChanged(DependencyObject sender, DependencyProperty dp)
		{
			BindToContent(Content);

			InvalidateShadows();
		}

		void OnShadowPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			_isShadowDirty = true;

			if (Uno.Toolkit.UI.Shadow.IsShadowSizeProperty(e.PropertyName ?? ""))
			{
				InvalidateCanvasLayout();
			}
			InvalidateShadows();
		}
		void OnInnerPropertyChanged(DependencyObject sender, DependencyProperty dp)
		{
			InvalidateShadows();
		}
		void OnContentPropertyChanged(DependencyObject sender, DependencyProperty dp)
		{
			// among the content's nested properties (CornerRadius, Margin) that gets sent here,
			// only Margin is used in UpdateCanvasLayout
			if (dp == FrameworkElement.MarginProperty)
			{
				InvalidateCanvasLayout();
			}
			InvalidateShadows();
		}
		void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			InvalidateCanvasLayout();
			InvalidateShadows();
		}

		void BindToBackgroundMemberProperties(Brush? background)
		{
			backgroundNestedDisposable.Disposable = background switch
			{
				SolidColorBrush scb => new CompositeDisposable
				{
					scb.RegisterDisposablePropertyChangedCallback(SolidColorBrush.ColorProperty, OnInnerPropertyChanged),
					scb.RegisterDisposablePropertyChangedCallback(Brush.OpacityProperty, OnInnerPropertyChanged),
				},

				null => null,
				_ => throw new NotSupportedException($"'{background.GetType().Name}' background brush is not supported."),
			};
		}
		void BindToShadowCollectionChanged(ShadowCollection? shadows)
		{
			if (shadows != null)
			{
				shadows.CollectionChanged += OnShadowCollectionChanged;
				BindItems(shadows);

				shadowsNestedDisposable.Disposable = Disposable.Create(() =>
				{
					shadows.CollectionChanged -= OnShadowCollectionChanged;
					UnbindItems(shadows);
				});

				_isShadowDirty = true;

				void OnShadowCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
				{
					if (e.Action != NotifyCollectionChangedAction.Move)
					{
						UnbindItems(e.OldItems?.Cast<Shadow>());
						BindItems(e.NewItems?.Cast<Shadow>());

						_isShadowDirty = true;

						InvalidateCanvasLayout();
					}
				}
				void BindItems(IEnumerable<Shadow>? shadows)
				{
					foreach (var item in shadows ?? Array.Empty<Shadow>())
					{
						item.PropertyChanged += OnShadowPropertyChanged;
					}
				}
				void UnbindItems(IEnumerable<Shadow>? shadows)
				{
					foreach (var item in shadows ?? Array.Empty<Shadow>())
					{
						item.PropertyChanged -= OnShadowPropertyChanged;
					}
				}
			}
			else
			{
				shadowsNestedDisposable.Disposable = null;

				_isShadowDirty = true;
			}
		}
		void BindToContent(object? content)
		{
			if (content is FrameworkElement contentAsFE)
			{
				contentNestedDisposable.Disposable = new CompositeDisposable
				{
					RegisterSizeChangedHandler(contentAsFE, OnContentSizeChanged),
					RegisterNestedPropertyChangedSafe<FrameworkElement>(GetCornerRadiusPropertyFor(content)),
					RegisterNestedPropertyChangedSafe<Rectangle>(Rectangle.RadiusXProperty),
					RegisterNestedPropertyChangedSafe<Rectangle>(Rectangle.RadiusYProperty),
					RegisterNestedPropertyChangedSafe<FrameworkElement>(FrameworkElement.MarginProperty),
				};

				static IDisposable RegisterSizeChangedHandler(FrameworkElement fe, SizeChangedEventHandler handler)
				{
					fe.SizeChanged += handler;
					return Disposable.Create(() => fe.SizeChanged -= handler);
				}
				IDisposable RegisterNestedPropertyChangedSafe<T>(DependencyProperty? dp, DependencyPropertyChangedCallback? callback = null)
				{
					if (contentAsFE is T && dp is { })
					{
						return contentAsFE.RegisterDisposablePropertyChangedCallback(dp, callback ?? OnContentPropertyChanged);
					}

					return Disposable.Empty;
				}
			}
			else
			{
				contentNestedDisposable.Disposable = null;
			}
		}
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_canvas = GetTemplateChild(nameof(PART_Canvas)) as Canvas;
		_panel = GetTemplateChild(nameof(PART_ShadowOwner)) as Grid;

		var skiaCanvas = new SKXamlCanvas();
		skiaCanvas.PaintSurface += OnSurfacePainted;

#if __IOS__ || __MACCATALYST__
		skiaCanvas.Opaque = false;
#endif

		_shadowHost = skiaCanvas;
		_canvas?.Children.Insert(0, _shadowHost!);
	}

	private void InvalidateCanvasLayout()
	{
		if (Content is not FrameworkElement contentAsFE ||
				_panel == null ||
				_canvas == null ||
				_shadowHost == null)
		{
			return;
		}

		var childWidth = contentAsFE.ActualWidth;
		var childHeight = contentAsFE.ActualHeight;
		if (childWidth == 0 || childHeight == 0)
		{
			return;
		}

		double absoluteMaxOffsetX = 0;
		double absoluteMaxOffsetY = 0;
		double maxBlurRadius = 0;
		double maxSpread = 0;

		if (Shadows is { Count: > 0 } shadows)
		{
			absoluteMaxOffsetX = shadows.Max(s => Math.Abs(s.OffsetX));
			absoluteMaxOffsetY = shadows.Max(s => Math.Abs(s.OffsetY));
			maxBlurRadius = shadows.Max(s => s.BlurRadius);
			maxSpread = shadows.Max(s => s.Spread);
		}

		//		_canvas.Height = childHeight;
		//		_canvas.Width = childWidth;
		//#if __ANDROID__ || __IOS__
		//		_canvas.GetDispatcherCompat().Schedule(() => _canvas.InvalidateMeasure());
		//#endif
		//		double newHostHeight = childHeight + maxBlurRadius * 2 + absoluteMaxOffsetY * 2 + maxSpread * 2;
		//		double newHostWidth = childWidth + maxBlurRadius * 2 + absoluteMaxOffsetX * 2 + maxSpread * 2;
		//		_shadowHost.Height = newHostHeight;
		//		_shadowHost.Width = newHostWidth;

		//		double diffWidthShadowHostChild = newHostWidth - childWidth;
		//		double diffHeightShadowHostChild = newHostHeight - childHeight;

		//		float left = (float)(-diffWidthShadowHostChild / 2 + contentAsFE.Margin.Left);
		//		float top = (float)(-diffHeightShadowHostChild / 2 + contentAsFE.Margin.Top);

		_canvas.Width = Math.Max(contentAsFE.ActualWidth - contentAsFE.Margin.Left - contentAsFE.Margin.Right, 0);
		_canvas.Height = Math.Max(contentAsFE.ActualHeight - contentAsFE.Margin.Top - contentAsFE.Margin.Bottom, 0);
	
		_canvas.HorizontalAlignment = contentAsFE.HorizontalAlignment;
		_canvas.VerticalAlignment = contentAsFE.VerticalAlignment;

		double newHostSpreedHeight = maxBlurRadius + absoluteMaxOffsetY + maxSpread;
		double newHostSpreedWidth = maxBlurRadius + absoluteMaxOffsetX + maxSpread;

		double newHostHeight = contentAsFE.ActualHeight + newHostSpreedHeight * 2;
		double newHostWidth = contentAsFE.ActualWidth + newHostSpreedWidth * 2;

		_shadowHost.Height = newHostHeight;
		_shadowHost.Width = newHostWidth;

		double top = 0;
		double left = 0;

		if (contentAsFE.VerticalAlignment == VerticalAlignment.Center)
		{
			_canvas.Margin = contentAsFE.Margin;
			_canvas.Margin = contentAsFE.Margin;

			if (contentAsFE.Margin == new Thickness(0, 0, 0, 0))
			{
				left = -newHostSpreedWidth;
				top = -newHostSpreedHeight;
			}
			else
			{
				left = -newHostSpreedWidth
							- (contentAsFE.HorizontalAlignment == HorizontalAlignment.Left ? 0 : 0)
							- (contentAsFE.HorizontalAlignment == HorizontalAlignment.Right ? contentAsFE.ActualWidth : 0)
							- (contentAsFE.HorizontalAlignment == HorizontalAlignment.Stretch ? +contentAsFE.Margin.Left / 2 + contentAsFE.Margin.Right / 2 : 0)
							- (contentAsFE.HorizontalAlignment == HorizontalAlignment.Center ? contentAsFE.ActualWidth / 2 : 0)

							;
				top = -newHostSpreedHeight - contentAsFE.ActualHeight / 2;
			}

		}
		else
		{
			left = -(newHostSpreedWidth
									+ (contentAsFE.HorizontalAlignment == HorizontalAlignment.Left ? -contentAsFE.Margin.Left : 0)
									+ (contentAsFE.HorizontalAlignment == HorizontalAlignment.Right && contentAsFE.VerticalAlignment != VerticalAlignment.Center ? contentAsFE.Margin.Right == 0 ? 0 :
													contentAsFE.ActualWidth + contentAsFE.Margin.Right - _canvas.Margin.Right : 0)
									+ (contentAsFE.HorizontalAlignment == HorizontalAlignment.Stretch ? contentAsFE.Margin.Right : 0)
									+ (contentAsFE.HorizontalAlignment == HorizontalAlignment.Center ? contentAsFE.Margin.Left : 0)
									);
			top = -(newHostSpreedHeight
											+ (contentAsFE.VerticalAlignment == VerticalAlignment.Top ? -contentAsFE.Margin.Top : 0)
											+ (contentAsFE.VerticalAlignment == VerticalAlignment.Bottom ? +contentAsFE.Margin.Bottom + contentAsFE.ActualHeight : 0)
											+ (contentAsFE.VerticalAlignment == VerticalAlignment.Stretch ? contentAsFE.Margin.Bottom : 0)
											+ (contentAsFE.VerticalAlignment == VerticalAlignment.Center ? contentAsFE.Margin.Top : 0)
										);
		}
		Canvas.SetLeft(_shadowHost, left);
		Canvas.SetTop(_shadowHost, top);
	}

	private void InvalidateShadows(bool force = false)
	{
		_shadowHost?.Invalidate();
	}

	private static DependencyProperty? GetCornerRadiusPropertyFor(object? content)
	{
		return content switch
		{
			Control => Control.CornerRadiusProperty,

			Border => Border.CornerRadiusProperty,
			Grid => Grid.CornerRadiusProperty,
			StackPanel => StackPanel.CornerRadiusProperty,

			Shape => null, // note: shapes have special handling, see: GetShadowShapeContext
			DependencyObject @do => @do.FindDependencyPropertyUsingReflection<CornerRadius>("CornerRadiusProperty"),
			_ => null,
		};
	}

	private static CornerRadius? GetCornerRadiusFor(object? content)
	{
		return content switch
		{
			Control corner => corner.CornerRadius,

			Border border => border.CornerRadius,
			Grid grid => grid.CornerRadius,
			StackPanel stackpanel => stackpanel.CornerRadius,

			Shape => null, // note: shapes have special handling, see: GetShadowShapeContext
			DependencyObject @do => @do.FindDependencyPropertyUsingReflection<CornerRadius>("CornerRadiusProperty") is { } dp
				? (CornerRadius)@do.GetValue(dp)
				: null,
			_ => null,
		};
	}
}
