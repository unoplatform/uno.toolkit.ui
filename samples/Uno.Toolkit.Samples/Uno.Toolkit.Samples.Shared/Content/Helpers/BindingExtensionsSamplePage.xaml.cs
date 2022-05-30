using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.Helpers;
using Uno.Toolkit.Samples.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.Samples.Content.Helpers
{
	[SamplePage(SampleCategory.Helpers, "Binding Extensions", SourceSdk.Uno, DataType = typeof(BindingExtensionsVM))]
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
