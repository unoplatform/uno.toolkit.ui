#if IS_WINUI
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Uno.Toolkit.Samples.ViewModels;

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(ShadowContainer),
		Description = "Add many colored shadows to your controls.",
		DataType = typeof(ShadowContainerViewModel))]
	public sealed partial class ShadowContainerSamplePage : Page
	{
		public ShadowContainerSamplePage()
		{
			this.InitializeComponent();
		}

		public class ShadowContainerViewModel : ViewModelBase
		{
			public ObservableCollection<string> CbbItems { get; } = new ObservableCollection<string>
			{
				"Item 1",
				"Item 2",
				"Item 3"
			};
		}
	}
}
#endif
