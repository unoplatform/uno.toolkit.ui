using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.Helpers;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows.Input;
using System.Net.WebSockets;
using Windows.Storage;

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

namespace Uno.Toolkit.Samples.Content.Helpers;

#if __ANDROID__ || __IOS__
[SamplePage(SampleCategory.Helpers, "MediaGalleryHelper", SourceSdk.Uno, IconSymbol = Symbol.BrowsePhotos, DataType = typeof(MediaGalleryHelperSampleVM))]
#endif
public sealed partial class MediaGalleryHelperSamplePage : Page
{
	public MediaGalleryHelperSamplePage()
	{
		this.InitializeComponent();
		this.Loaded += (s, e) =>
		{
			if ((DataContext as Sample)?.Data is MediaGalleryHelperSampleVM vm)
			{
				vm.XamlRoot = this.XamlRoot;
			}
		};
	}
}

public class MediaGalleryHelperSampleVM : ViewModelBase
{
	public XamlRoot XamlRoot { get; set; }

#if __ANDROID__ || __IOS__
	public ICommand CheckAccessCommand => new Command(async (_) =>
	{
		var success = await MediaGallery.CheckAccessAsync();
		await new ContentDialog
		{
			Title = "Permission check",
			Content = $"Access {(success ? "granted" : "denied")}.",
			CloseButtonText = "OK",
			XamlRoot = XamlRoot
		}.ShowAsync();
	});

	public ICommand SaveCommand => new Command(async (_) =>
	{
		if (await MediaGallery.CheckAccessAsync())
		{
			var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UnoLogo.png", UriKind.Absolute));
			using var stream = await file.OpenStreamForReadAsync();
			await MediaGallery.SaveAsync(MediaFileType.Image, stream, "UnoLogo.png");
		}
		else
		{
			await new ContentDialog
			{
				Title = "Permission required",
				Content = "The app requires access to the device's gallery to save the image.",
				CloseButtonText = "OK",
				XamlRoot = XamlRoot
			}.ShowAsync();
		}
	});

	public ICommand SaveRandomNameCommand => new Command(async (_) =>
	{
		if (await MediaGallery.CheckAccessAsync())
		{
			var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UnoLogo.png", UriKind.Absolute));
			using var stream = await file.OpenStreamForReadAsync();

			var fileName = Guid.NewGuid() + ".png";
			await MediaGallery.SaveAsync(MediaFileType.Image, stream, fileName);
		}
		else
		{
			await new ContentDialog
			{
				Title = "Permission required",
				Content = "The app requires access to the device's gallery to save the image.",
				CloseButtonText = "OK",
				XamlRoot = XamlRoot
			}.ShowAsync();
		}
	});
#endif
}
