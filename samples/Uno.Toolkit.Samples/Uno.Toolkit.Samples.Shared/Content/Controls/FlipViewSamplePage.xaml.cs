using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.UI;
using Uno.Toolkit.Samples.Helpers;

#if __IOS__
using UIKit;
#endif

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using XamlWindow = Microsoft.UI.Xaml.Window;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlWindow = Windows.UI.Xaml.Window;
using System.Collections.ObjectModel;


#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Behaviors, nameof(FlipViewExtensions))]
	public sealed partial class FlipViewSamplePage : Page
	{
		public FlipViewSamplePage()
		{
			this.InitializeComponent();


			this.Loaded += (_, __) =>
			{
				var btn = this.SamplePageLayout.GetSampleChild<Button>(Design.Agnostic, "AddNewPageButton");

				var flipView = this.SamplePageLayout.GetSampleChild<FlipView>("flipView");
				btn.Click += (_, __) =>
				{
					var grid = new Grid();

					var bt1 = new Button
					{
						Content = "Previous",
						HorizontalAlignment = HorizontalAlignment.Left
					};
					FlipViewExtensions.SetPrevious(bt1, flipView);


					var bt2 = new Button
					{
						Content = "Next",
						HorizontalAlignment = HorizontalAlignment.Right
					};
					FlipViewExtensions.SetNext(bt2, flipView);

					grid.Children.Add(bt1);
					grid.Children.Add(bt2);

					flipView.Items.Add(grid);
				};
			};
		}
	}
}
