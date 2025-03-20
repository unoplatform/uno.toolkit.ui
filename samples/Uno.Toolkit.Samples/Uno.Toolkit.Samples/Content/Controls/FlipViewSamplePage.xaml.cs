using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

#if __IOS__
using UIKit;





using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlWindow = Windows.UI.Xaml.Window;
using System.Collections.ObjectModel;




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

				var flipView = this.SamplePageLayout.GetSampleChild<FlipView>(Design.Agnostic, "flipView");
				var flipViewItems = this.SamplePageLayout.GetSampleChild<TextBlock>(Design.Agnostic, "flipViewItems");
				flipViewItems.Text = $"{flipView.Items.Count}";

				flipView.Items.VectorChanged += (_, __) =>
				{
					flipViewItems.Text = $"{flipView.Items.Count}";
				};

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
