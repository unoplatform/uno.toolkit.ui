using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.ViewModels;


namespace Uno.Toolkit.Samples.Content.Helpers
{
	[SamplePage(SampleCategory.Helpers, "Binding Extensions", SourceSdk.Uno, DataType = typeof(BindingExtensionsVM), IconPath = Icons.Helpers.MarkupExtension)]
	public sealed partial class BindingExtensionsSamplePage : Page
	{
		public BindingExtensionsSamplePage()
		{
			this.InitializeComponent();
		}
	}

	public class BindingExtensionsVM : ViewModelBase
	{
		public string PropertyOnSameLevelAsItems { get => GetProperty<string>(); set => SetProperty(value); }

		public string[] Items { get; }

		public BindingExtensionsVM()
		{
			PropertyOnSameLevelAsItems = "Asd";
			Items = Enumerable.Range(0, 2)
				.Select(x => $"Item #{x}")
				.ToArray();
		}
	}
}
