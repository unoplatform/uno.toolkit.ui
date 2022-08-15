using System;
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
	[SamplePage(SampleCategory.Behaviors, nameof(InputExtensions), SourceSdk.UnoToolkit, DataType = typeof(InputExtensionsSamplePageVM))]
	public sealed partial class InputExtensionsSamplePage : Page
	{
		public InputExtensionsSamplePage()
		{
			this.InitializeComponent();
		}

		public class InputExtensionsSamplePageVM : ViewModelBase
		{
			public string Username { get => GetProperty<string>(); set => SetProperty(value); }
			public string Password { get => GetProperty<string>(); set => SetProperty(value); }

			public string DebugText { get => GetProperty<string>(); set => SetProperty(value); }
			
			public ICommand LoginCommand => new Command(Login);

			private void Login(object parameter)
			{
				DebugText = string.Concat(new[]
				{
					$"{DateTime.Now} Logged in",
					Username?.Length > 0 ? $" as ({Username}" : null,
					Username?.Length > 0 && Password?.Length > 0 ? $":{Password}" : null,
					Username?.Length > 0 ? ")" : null,
				}.Where(x => x != null));
			}
		}
	}
}
