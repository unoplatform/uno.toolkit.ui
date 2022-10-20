using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.Foundation.Collections;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using PipsPager = Microsoft.UI.Xaml.Controls.PipsPager;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Extensions for <see cref="Selector"/>
/// </summary>
[Bindable]
public static partial class SelectorExtensions
{
	readonly static Dictionary<Selector, VectorChangedEventHandler<object>> events = new();

	/// <summary>
	/// Backing property for the <see cref="PipsPager"/> that will be linked to the desired <see cref="Selector"/> control.
	/// </summary>
	#region DependencyProperty: PipsPager
	public static DependencyProperty PipsPagerProperty { get; } =
	DependencyProperty.RegisterAttached("PipsPager", typeof(PipsPager), typeof(SelectorExtensions), new PropertyMetadata(null, OnPipsPagerChanged));

	public static void SetPipsPager(Selector element, PipsPager value) =>
		element.SetValue(PipsPagerProperty, value);

	public static PipsPager GetPipsPager(Selector element) =>
		(PipsPager)element.GetValue(PipsPagerProperty);
	#endregion

	static void OnPipsPagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue == args.OldValue)
			return;

		if (args.OldValue is PipsPager oldPipsPager)
			oldPipsPager.SetBinding(PipsPager.SelectedPageIndexProperty, null);

		var selector = (Selector)dependencyObject;

		if (args.NewValue is not PipsPager pipsPager)
		{
			UnsubscribeFromSelectorEvents(selector);
			return;
		}

		var selectedIndexBinding = new Binding
		{
			Mode = BindingMode.TwoWay,
			Source = selector,
			Path = new PropertyPath(nameof(selector.SelectedIndex))
		};

		pipsPager.SetBinding(PipsPager.SelectedPageIndexProperty, selectedIndexBinding);
		pipsPager.NumberOfPages = selector.Items.Count;

		UnsubscribeFromSelectorEvents(selector);
		VectorChangedEventHandler<object> eventHandler = OnItemsVectorChanged;
		events[selector] = eventHandler;
		selector.Items.VectorChanged += eventHandler;
		selector.Unloaded += (_, __) =>
		{
			selector.Items.VectorChanged -= eventHandler;
		};


		void OnItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event) =>
			pipsPager.NumberOfPages = selector.Items.Count;
	}

	static void UnsubscribeFromSelectorEvents(in Selector selector)
	{
		if (events.TryGetValue(selector, out var @event))
		{
			selector.Items.VectorChanged -= @event;
			events.Remove(selector);
		}
	}

	#region SelectionOffset Attached Property
	public static double GetSelectionOffset(DependencyObject obj)
	{
		return (double)obj.GetValue(SelectionOffsetProperty);
	}

	public static void SetSelectionOffset(DependencyObject obj, double value)
	{
		obj.SetValue(SelectionOffsetProperty, value);
	}

	/// <summary>
	/// Property that can be used to observe the position of the currently selected item within a <see cref="Selector"/>
	/// </summary>
	public static DependencyProperty SelectionOffsetProperty { get; } =
		DependencyProperty.RegisterAttached("SelectionOffset", typeof(double), typeof(SelectorExtensions), new PropertyMetadata(0d));
	#endregion
}

