namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class M3MaterialVerticalBarSampleNestedPage : Page
	{
		public M3MaterialVerticalBarSampleNestedPage()
		{
			this.InitializeComponent();
		}

		private void OnSelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
		{
			if (args.NewItem is TabBarItem tabBarItem)
			{
				foreach (var page in PageContainer.Children)
				{
					var pageGrid = page as Grid;
					if (pageGrid != null)
					{
						pageGrid.Visibility = pageGrid.Name == (string)tabBarItem.Tag ? Visibility.Visible : Visibility.Collapsed;
					}
				}
			}
		}

		private void NavigateBack(object sender, RoutedEventArgs e)
		{
			// Normally we would've just called `Frame.GoBack();` if we only have a single frame.
			// However, a nested frame is used to show-case fullscreen sample, so we need some
			// custom handling to hide the nested frame on back navigation when the stack is empty.
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}
	}
}
