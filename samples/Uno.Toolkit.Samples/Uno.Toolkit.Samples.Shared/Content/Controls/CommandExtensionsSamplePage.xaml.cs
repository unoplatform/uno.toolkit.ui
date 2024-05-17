using System;
using System.Windows.Input;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;
using static System.FormattableString;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Behaviors, nameof(CommandExtensions), SourceSdk.UnoToolkit, DataType = typeof(CommandExtensionsSamplePageVM))]
	public sealed partial class CommandExtensionsSamplePage : Page
	{
		public CommandExtensionsSamplePage()
		{
			this.InitializeComponent();
		}

		public class CommandExtensionsSamplePageVM : ViewModelBase
		{
			public string InputDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string SelectionDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string NavigationDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ItemsRepeaterDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ElementDebugText { get => GetProperty<string>(); set => SetProperty(value); }

			public ICommand DebugInputCommand => new Command(DebugInput);
			public ICommand DebugSelectionCommand => new Command(DebugSelection);
			public ICommand DebugNavigationCommand => new Command(DebugNavigation);
			public ICommand DebugItemsRepeaterCommand => new Command(DebugItemsRepeater);
			public ICommand DebugElementTappedCommand => new Command(DebugElement);

			private void DebugInput(object parameter) => InputDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugSelection(object parameter) => SelectionDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugNavigation(object parameter) => NavigationDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugItemsRepeater(object parameter) => ItemsRepeaterDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugElement(object parameter) => ElementDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
		}
	}
}
