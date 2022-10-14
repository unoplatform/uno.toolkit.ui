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

namespace Uno.Toolkit.UI.Behaviors;
public static class SelectorExtensions
{
	/// <summary>
	/// Backing property for the <see cref="PipsPager"/> that will be linked to the desired <see cref="Selector"/> control.
	/// </summary>
	public static readonly DependencyProperty PipsPagerProperty =
		DependencyProperty.RegisterAttached("PipsPager", typeof(PipsPager), typeof(SelectorExtensions), new PropertyMetadata(null, OnPipsPagerChanged));

	static void OnPipsPagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue == args.OldValue || args.NewValue is not Selector flipView)
			return;

		var pipsPager = (PipsPager)args.NewValue;

		flipView.Items.VectorChanged += OnItemsVectorChanged;

		var selectedIndexBinding = new Binding
		{
			Mode = BindingMode.TwoWay,
			Source = flipView,
			Path = new PropertyPath(nameof(flipView.SelectedIndex))
		};

		pipsPager.SetBinding(PipsPager.SelectedPageIndexProperty, selectedIndexBinding);

		pipsPager.NumberOfPages = flipView.Items.Count;


		void OnItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event) =>
			pipsPager.NumberOfPages = flipView.Items.Count;
	}

	public static void SetPipsPager(Selector element, PipsPager value) =>
		element.SetValue(PipsPagerProperty, value);

	public static PipsPager GetPipsPager(Selector element) =>
		(PipsPager)element.GetValue(PipsPagerProperty);
}

