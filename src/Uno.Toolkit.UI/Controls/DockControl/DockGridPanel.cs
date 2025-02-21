using System;
using System.Collections.Specialized;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class DockGridPanel : Grid
{
	#region DependencyProperty: Orientation, proc: OnOrientationChanged

	public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
		nameof(Orientation),
		typeof(Orientation),
		typeof(DockGridPanel),
		new PropertyMetadata(default(Orientation), OnOrientationChanged));

	public Orientation Orientation
	{
		get => (Orientation)GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
		(sender as DockGridPanel)?.OnOrientationChanged();

	#endregion

	public DockGridPanel()
	{
		Children.CollectionChanged += OnChildrenChanged;
	}

	private void OnOrientationChanged()
	{
		Children.ToString();
	}

	private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e is { Action: NotifyCollectionChangedAction.Add, NewItems: { }})
		{
			foreach (var item in e.NewItems)
			{
				if (item is not DockPane pane)
					throw new InvalidOperationException($"Non DockPane child inserted: {item?.GetType().Name}");

				if (Orientation == Orientation.Vertical)
				{
					RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
					SetRow(pane, RowDefinitions.Count - 1);
				}
				else
				{
					ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
					SetColumn(pane, RowDefinitions.Count - 1);
				}
			}
		}
		else
		{
			Console.WriteLine($"OnChildrenChanged: {e.Action}");
			throw new NotImplementedException(e.Action.ToString());
		}
	}
}
public partial class DockPaneItemsGrid : Grid
{
	public DockPaneItemsGrid()
	{
		Loaded += (s, e) => this.FindFirstAncestor<LayoutPane>()?.OnItemsPanelPrepared(this);
	}
}
