using System;
using System.Collections.Generic;
using System.Text;
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
	[TemplatePart(Name = RemoveButtonName, Type = typeof(Button))]
	public partial class Chip : ToggleButton
	{
		private const string RemoveButtonName = "PART_RemoveButton";

		private bool _isMuted;

		public Chip()
		{
			Checked += RaiseIsCheckedChanged;
			Unchecked += RaiseIsCheckedChanged;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (GetTemplateChild(RemoveButtonName) is Button removeButton)
			{
				removeButton.Click += RaiseRemoveButtonClicked;
			}
		}

		private void OnIsCheckableChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!IsCheckable)
			{
				IsChecked = false;
			}
		}

		private void RaiseIsCheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!_isMuted)
			{
				IsCheckedChanged?.Invoke(sender, e);
			}
		}

		private void RaiseRemoveButtonClicked(object sender, RoutedEventArgs e)
		{
			// note: sender is the RemoveButton, do not pass it as the event sender
			// as ChipGroup expect the sender to be an instance of Chip

			if (CanRemove)
			{
				var removingArgs = new ChipRemovingEventArgs();
				Removing?.Invoke(this, removingArgs);

				if (!removingArgs.Cancel)
				{
					Removed?.Invoke(this, e);

					var param = RemovedCommandParameter;
					if (RemovedCommand is ICommand command && command.CanExecute(param))
					{
						command.Execute(param);
					}
				}
			}
		}

		internal void SetIsCheckedSilently(bool? value)
		{
			try
			{
				_isMuted = true;
				IsChecked = value;
			}
			finally
			{
				_isMuted = false;
			}
		}

		protected override void OnToggle()
		{
			if (!IsCheckable) return;

			base.OnToggle();
		}
	}
}
