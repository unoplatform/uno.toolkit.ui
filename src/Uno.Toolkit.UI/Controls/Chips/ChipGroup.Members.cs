using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	public partial class ChipGroup // Properties
	{
		#region DependencyProperty: CanRemove = false

		public static DependencyProperty CanRemoveProperty { get; } = DependencyProperty.Register(
			nameof(CanRemove),
			typeof(bool),
			typeof(ChipGroup),
			new PropertyMetadata(false, (s, e) => (s as ChipGroup)?.ApplyCanRemoveProperty()));

		/// <summary>
		/// Gets or sets the value of each <see cref="Chip.CanRemove"/>.
		/// </summary>
		public bool CanRemove
		{
			get => (bool)GetValue(CanRemoveProperty);
			set => SetValue(CanRemoveProperty, value);
		}

		#endregion

		#region DependencyProperty: IconTemplate

		public static DependencyProperty IconTemplateProperty { get; } = DependencyProperty.Register(
			nameof(IconTemplate),
			typeof(DataTemplate),
			typeof(ChipGroup),
			new PropertyMetadata(null, (s, e) => (s as ChipGroup)?.ApplyIconTemplate()));

		/// <summary>
		/// Gets or sets the value of each <see cref="Chip.IconTemplate"/>.
		/// </summary>
		public DataTemplate IconTemplate
		{
			get => (DataTemplate)GetValue(IconTemplateProperty);
			set => SetValue(IconTemplateProperty, value);
		}

		#endregion

		#region DependencyProperty: SelectedItem

		public static DependencyProperty SelectedItemProperty { get; } = DependencyProperty.Register(
			nameof(SelectedItem),
			typeof(object),
			typeof(ChipGroup),
			new PropertyMetadata(default, (s, e) => (s as ChipGroup)?.OnSelectedItemChanged()));

		/// <summary>
		/// Gets or sets the selected item.
		/// </summary>
		/// <remarks>
		/// This property only works for <see cref="ChipSelectionMode.SingleOrNone"/>.
		/// </remarks>
		public object? SelectedItem
		{
			get => (object)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		#endregion

		#region DependencyProperty: SelectedItems

		public static DependencyProperty SelectedItemsProperty { get; } = DependencyProperty.Register(
			nameof(SelectedItems),
			typeof(IList),
			typeof(ChipGroup),
			new PropertyMetadata(default, (s, e) => (s as ChipGroup)?.OnSelectedItemsChanged()));

		/// <summary>
		/// Gets or sets the selected items.
		/// </summary>
		/// <remarks>
		/// The value will be null if the selection is empty.
		/// This property only works for <see cref="ChipSelectionMode.Multiple"/>.
		/// </remarks>
		public IList? SelectedItems
		{
			get => (IList)GetValue(SelectedItemsProperty);
			set => SetValue(SelectedItemsProperty, value);
		}

		#endregion

		#region DependencyProperty: SelectionMemberPath

		public static DependencyProperty SelectionMemberPathProperty { get; } = DependencyProperty.Register(
			nameof(SelectionMemberPath),
			typeof(string),
			typeof(ChipGroup),
			new PropertyMetadata(default, (s, e) => (s as ChipGroup)?.OnSelectionMemberPathChanged(e)));
#pragma warning disable CS1574 // XML comment has cref attribute 'IsChecked' that could not be resolved
		/// <summary>
		/// Gets or sets the path which each <see cref="Chip.IsChecked"/> is data-bind to.
		/// </summary>
#pragma warning restore CS1574
		public string SelectionMemberPath
		{
			get => (string)GetValue(SelectionMemberPathProperty);
			set => SetValue(SelectionMemberPathProperty, value);
		}

		#endregion

		#region DependencyProperty: SelectionMode = ChipSelectionMode.Single

		public static DependencyProperty SelectionModeProperty { get; } = DependencyProperty.Register(
			nameof(SelectionMode),
			typeof(ChipSelectionMode),
			typeof(ChipGroup),
			new PropertyMetadata(ChipSelectionMode.Single, (s, e) => (s as ChipGroup)?.OnSelectionModeChanged(e)));

		/// <summary>
		/// Gets or sets the selection behavior.
		/// </summary>
		/// <remarks>
		/// Changing this value will cause <see cref="SelectedItem"/> and <see cref="SelectedItems"/> to be re-coerced.
		/// </remarks>
		public ChipSelectionMode SelectionMode
		{
			get => (ChipSelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		#endregion
	}

	public partial class ChipGroup // Events
	{
		/// <summary>
		/// Occurs when a <see cref="Chip"/> item is pressed.
		/// </summary>
		public event ChipItemEventHandler? ItemClick;

		/// <summary>
		/// Occurs when a <see cref="Chip"/> item is checked.
		/// </summary>
		public event ChipItemEventHandler? ItemChecked;

		/// <summary>
		/// Occurs when a <see cref="Chip"/> item is unchecked.
		/// </summary>
		public event ChipItemEventHandler? ItemUnchecked;

		/// <summary>
		/// Occurs when a <see cref="Chip"/> item is removed.
		/// </summary>
		public event ChipItemEventHandler? ItemRemoved;

		/// <summary>
		/// Occurs when a <see cref="Chip"/> item is about to be removed.
		/// </summary>
		public event ChipItemRemovingEventHandler? ItemRemoving;
	}
}
