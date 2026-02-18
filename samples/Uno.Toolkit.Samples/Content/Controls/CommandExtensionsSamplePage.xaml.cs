using Uno.Toolkit.Samples.ViewModels;
using static System.FormattableString;


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
			public string[] Fruits { get; } = new[] { "Apple", "Banana", "Cactus" };

			public string InputDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ToggleSwitchText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ListViewDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string SelectorDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string NavigationDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ItemsRepeaterDebugText { get => GetProperty<string>(); set => SetProperty(value); }
			public string ElementDebugText { get => GetProperty<string>(); set => SetProperty(value); }

			public ICommand DebugInputCommand => new Command(DebugInput);
			public ICommand DebugToggleSwitchCommand => new Command(DebugToggleSwitch);
			public ICommand DebugListViewCommand => new Command(DebugListView);
			public ICommand DebugSelectorCommand => new Command(DebugSelector);
			public ICommand DebugNavigationCommand => new Command(DebugNavigation);
			public ICommand DebugItemsRepeaterCommand => new Command(DebugItemsRepeater);
			public ICommand DebugElementTappedCommand => new Command(DebugElement);

			private void DebugInput(object parameter) => InputDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugToggleSwitch(object parameter) => ToggleSwitchText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugListView(object parameter) => ListViewDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugSelector(object parameter) => SelectorDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugNavigation(object parameter) => NavigationDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugItemsRepeater(object parameter) => ItemsRepeaterDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
			private void DebugElement(object parameter) => ElementDebugText = Invariant($"{DateTime.Now:HH:mm:ss}: parameter={parameter}");
		}
	}
}
