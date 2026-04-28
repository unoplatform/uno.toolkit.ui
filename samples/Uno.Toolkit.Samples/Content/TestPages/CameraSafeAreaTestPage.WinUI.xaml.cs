using System;
#if __IOS__
using System.Linq;
using UIKit;
#endif

namespace Uno.Toolkit.Samples.Content.TestPages
{
	[SamplePage(SampleCategory.Tests, "CameraSafeAreaTest", SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
	public sealed partial class CameraSafeAreaTestPage : Page
	{
		private int _tapCount;

		public CameraSafeAreaTestPage()
		{
			this.InitializeComponent();

#if !__IOS__
			PresentPickerButton.IsEnabled = false;
			PickerStatusText.Text = "Picker is iPad-only — only the 'Tap me' counter is exercised on this platform.";
#endif
		}

		private void OnTapClicked(object sender, RoutedEventArgs e)
		{
			_tapCount++;
			TapCountText.Text = $"Taps: {_tapCount}";
		}

		private void OnPresentPickerClicked(object sender, RoutedEventArgs e)
		{
#if __IOS__
			try
			{
				var picker = new UIImagePickerController
				{
					SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
					ModalPresentationStyle = UIModalPresentationStyle.FormSheet,
				};

				picker.Canceled += (s, args) =>
				{
					PickerStatusText.Text = "Picker canceled — now retry the 'Tap me' button.";
					picker.DismissViewController(true, null);
				};

				picker.FinishedPickingMedia += (s, args) =>
				{
					PickerStatusText.Text = "Picker finished — now retry the 'Tap me' button.";
					picker.DismissViewController(true, null);
				};

				var rootVC = ResolveRootViewController();
				if (rootVC is null)
				{
					PickerStatusText.Text = "No root view controller available.";
					return;
				}

				PickerStatusText.Text = "Presenting picker...";
				rootVC.PresentViewController(picker, true, null);
			}
			catch (Exception ex)
			{
				PickerStatusText.Text = $"Failed to present picker: {ex.Message}";
			}
#endif
		}

#if __IOS__
		private static UIViewController? ResolveRootViewController()
		{
			var keyWindowRoot = UIApplication.SharedApplication.KeyWindow?.RootViewController;
			if (keyWindowRoot is not null)
			{
				return keyWindowRoot;
			}

			return UIApplication.SharedApplication.ConnectedScenes
				.OfType<UIWindowScene>()
				.SelectMany(s => s.Windows)
				.FirstOrDefault(w => w.IsKeyWindow)?.RootViewController;
		}
#endif
	}
}
