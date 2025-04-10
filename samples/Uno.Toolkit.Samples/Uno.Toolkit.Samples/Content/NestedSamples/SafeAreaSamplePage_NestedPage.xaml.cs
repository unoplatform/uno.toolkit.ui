using static Uno.Toolkit.UI.SafeArea;


namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SafeAreaSamplePage_NestedPage : Page
	{
		public SafeAreaSamplePage_NestedPage()
		{
			this.InitializeComponent();
		}

		private void MarginChecked(object sender, RoutedEventArgs e)
		{
			SafeArea.SetMode(MyBorder, InsetMode.Margin);
		}

		private void PaddingChecked(object sender, RoutedEventArgs e)
		{
			SafeArea.SetMode(MyBorder, InsetMode.Padding);
		}

		private void LeftChecked(object sender, RoutedEventArgs e)
		{
			AddMask(InsetMask.Left);
		}

		private void TopChecked(object sender, RoutedEventArgs e)
		{
			AddMask(InsetMask.Top);
		}

		private void RightChecked(object sender, RoutedEventArgs e)
		{
			AddMask(InsetMask.Right);
		}

		private void BottomChecked(object sender, RoutedEventArgs e)
		{
			AddMask(InsetMask.Bottom);
		}

		private void LeftUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveMask(InsetMask.Left);
		}

		private void TopUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveMask(InsetMask.Top);
		}

		private void RightUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveMask(InsetMask.Right);
		}

		private void BottomUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveMask(InsetMask.Bottom);
		}

		private void AddMask(InsetMask insetMask)
		{
			var mask = SafeArea.GetInsets(MyBorder);

			if (!mask.HasFlag(insetMask))
			{
				SafeArea.SetInsets(MyBorder, mask | insetMask);
			}
		}

		private void RemoveMask(InsetMask insetMask)
		{
			var mask = SafeArea.GetInsets(MyBorder);

			if (mask.HasFlag(insetMask))
			{
				SafeArea.SetInsets(MyBorder, mask & ~insetMask);
			}
		}

		private void UpdateInsets(object sender, TextChangedEventArgs e)
		{
			var leftInset = double.TryParse(OverrideInsetLeft.Text, out var left) ? left : 0;
			var topInset = double.TryParse(OverrideInsetTop.Text, out var top) ? top : 0;
			var rightInset = double.TryParse(OverrideInsetRight.Text, out var right) ? right : 0;
			var bottomInset = double.TryParse(OverrideInsetBottom.Text, out var bottom) ? bottom : 0;

			var newInsets = new Thickness(leftInset, topInset, rightInset, bottomInset);
			SafeArea.SetSafeAreaOverride(MyBorder, newInsets.Equals(new Thickness(0)) ? default(Thickness?) : newInsets);
		}

		private void ClearAllMasks(object sender, RoutedEventArgs e)
		{
			LeftMask.IsChecked = false;
			TopMask.IsChecked = false;
			RightMask.IsChecked = false;
			BottomMask.IsChecked = false;

			SafeArea.SetInsets(MyBorder, InsetMask.None);
		}

		private void AllTwenty(object sender, RoutedEventArgs e)
		{
			SafeArea.SetSafeAreaOverride(MyBorder, new Thickness(20));
		}

		private void ExitSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}
	}
}
