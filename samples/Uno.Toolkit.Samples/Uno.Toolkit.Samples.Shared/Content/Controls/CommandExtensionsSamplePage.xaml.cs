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
	[SamplePage(SampleCategory.Behaviors, nameof(ControlExtensions), SourceSdk.UnoToolkit, DataType = typeof(ControlExtensionsSamplePageVM))]
	public sealed partial class ControlExtensionsSamplePage : Page
	{
		public ControlExtensionsSamplePage()
		{
			this.InitializeComponent();
		}

		public class ControlExtensionsSamplePageVM : ViewModelBase
		{
			public string InputDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string SelectionDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string NavigationDebugText { get => GetProperty<string>(); set => SetProperty(value); }

			public ICommand DebugInputCommand => new Command(DebugInput);
			public ICommand DebugSelectionCommand => new Command(DebugSelection);
			public ICommand DebugNavigationCommand => new Command(DebugNavigation);

			private void DebugInput(object parameter) => InputDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugSelection(object parameter) => SelectionDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugNavigation(object parameter) => NavigationDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
		}
	}
}
