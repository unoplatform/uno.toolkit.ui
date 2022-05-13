using System.Windows.Input;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI
{
	public partial class Chip // Properties
	{
		#region DependencyProperty: CanRemove = false

		public static DependencyProperty CanRemoveProperty { get; } = DependencyProperty.Register(
			nameof(CanRemove),
			typeof(bool),
			typeof(Chip),
			new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets whether the remove button is visible.
		/// </summary>
		public bool CanRemove
		{
			get => (bool)GetValue(CanRemoveProperty);
			set => SetValue(CanRemoveProperty, value);
		}

		#endregion

		#region DependencyProperty: Elevation

		public static DependencyProperty ElevationProperty { get; } = DependencyProperty.Register(
			nameof(Elevation),
			typeof(double),
			typeof(Chip),
			new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets the elevation of the Chip.
		/// </summary>
		public double Elevation
		{
			get => (double)GetValue(ElevationProperty);
			set => SetValue(ElevationProperty, value);
		}

		#endregion

		#region DependencyProperty: Icon

		public static DependencyProperty IconProperty { get; } = DependencyProperty.Register(
			nameof(Icon),
			typeof(object),
			typeof(Chip),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the icon of the Chip.
		/// </summary>
		public object Icon
		{
			get => (object)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		#endregion

		#region DependencyProperty: IconTemplate

		public static DependencyProperty IconTemplateProperty { get; } = DependencyProperty.Register(
			nameof(IconTemplate),
			typeof(DataTemplate),
			typeof(Chip),
			new PropertyMetadata(default(DataTemplate)));

		/// <summary>
		/// Gets or sets the data template that is used to display the icon of the Chip.
		/// </summary>
		public DataTemplate IconTemplate
		{
			get => (DataTemplate)GetValue(IconTemplateProperty);
			set => SetValue(IconTemplateProperty, value);
		}

		#endregion

		#region DependencyProperty: IsCheckable = true

		public static DependencyProperty IsCheckableProperty { get; } = DependencyProperty.Register(
			nameof(IsCheckable),
			typeof(bool),
			typeof(Chip),
			new PropertyMetadata(true, (s, e) => (s as Chip)?.OnIsCheckableChanged(e)));

		/// <summary>
		/// Gets or sets whether the chip can be checked. Used to prevent showing selection state.
		/// </summary>
		/// <remarks>
		/// When nested under the <see cref="ChipGroup"/>, this property will be overwritten by <see cref="ChipGroup.SelectionMode"/>.
		/// </remarks>
		public bool IsCheckable
		{
			get => (bool)GetValue(IsCheckableProperty);
			set => SetValue(IsCheckableProperty, value);
		}

		#endregion

		#region DependencyProperty: RemovedCommand

		public static DependencyProperty RemovedCommandProperty { get; } = DependencyProperty.Register(
			nameof(RemovedCommand),
			typeof(ICommand),
			typeof(Chip),
			new PropertyMetadata(default));

		/// <summary>
		/// Gets or sets the command to invoke when the remove button is pressed.
		/// </summary>
		public ICommand RemovedCommand
		{
			get => (ICommand)GetValue(RemovedCommandProperty);
			set => SetValue(RemovedCommandProperty, value);
		}

		#endregion

		#region DependencyProperty: RemovedCommandParameter

		public static DependencyProperty RemovedCommandParameterProperty { get; } = DependencyProperty.Register(
			nameof(RemovedCommandParameter),
			typeof(object),
			typeof(Chip),
			new PropertyMetadata(default));

		/// <summary>
		/// Gets or sets the parameter to pass to the <see cref="RemovedCommand"/> property.
		/// </summary>
		public object RemovedCommandParameter
		{
			get => (object)GetValue(RemovedCommandParameterProperty);
			set => SetValue(RemovedCommandParameterProperty, value);
		}

		#endregion
	}

	public partial class Chip // Events
	{
		/// <summary>
		/// Occurs when the `Chip` is checked or unchecked.
		/// </summary>
		/// <remarks>
		/// This event is bypassed when set with <see cref="SetIsCheckedSilently(bool?)"/>
		/// </remarks>
		internal event RoutedEventHandler IsCheckedChanged;

		/// <summary>
		/// Occurs when the remove button is pressed.
		/// </summary>
		/// <remarks>
		/// When used outside of a <see cref="ChipGroup"/>, this event does not cause the Chip to be removed from the view.
		/// </remarks>
		public event RoutedEventHandler Removed;

		/// <summary>
		/// Occurs when the remove button is pressed, but before <see cref="Removed" /> event allowing for cancellation.
		/// </summary>
		public event ChipRemovingEventHandler Removing;
	}
}
