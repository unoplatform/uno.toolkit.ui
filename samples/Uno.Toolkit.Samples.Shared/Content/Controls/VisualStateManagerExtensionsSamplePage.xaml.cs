using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Behaviors, nameof(VisualStateManagerExtensions), DataType = typeof(ViewModel))]
	public sealed partial class VisualStateExtensionsSamplePage : Page
	{
		public VisualStateExtensionsSamplePage()
		{
			this.InitializeComponent();
		}

		public class ViewModel : ViewModelBase
		{
			public string PageState { get => GetProperty<string>(); set => SetProperty(value); }

			public ICommand ChangePageStateCommand => new Command(ChangePageState);

			public ViewModel()
			{
				PageState = "Blue";
			}

			private void ChangePageState(object parameter)
			{
				if (parameter is string state)
				{
					PageState = state;
				}
			}
		}
	}
}
