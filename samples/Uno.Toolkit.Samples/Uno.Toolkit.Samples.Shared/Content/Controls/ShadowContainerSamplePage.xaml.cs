using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.Controls
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[SamplePage(SampleCategory.Controls, nameof(ShadowContainer), Description = "Add many colored shadows to your controls.")]
	public sealed partial class ShadowContainerSamplePage : Page
	{
		private ShadowCollection _shadows;

		public ShadowContainerSamplePage()
		{
			this.InitializeComponent();

			this.Loaded += (s, e) =>
			{
				var shadowContainer = SamplePageLayout.GetSampleChild<ShadowContainer>(Design.Agnostic, "ShadowContainer");
				_shadows = shadowContainer.Shadows;

				var shadowsItemsControl = SamplePageLayout.GetSampleChild<ItemsControl>(Design.Agnostic, "ShadowsItemsControl");
				shadowsItemsControl.ItemsSource = _shadows;
			};
		}

		private void AddShadow(object sender, RoutedEventArgs e)
		{
			var defaultShadow = (Shadow)Resources["DefaultShadow"];

			_shadows.Add(defaultShadow.Clone());
		}

		private void RemoveShadow(object sender, RoutedEventArgs e)
		{
			if (_shadows.Count == 0)
			{
				return;
			}

			_shadows.RemoveAt(_shadows.Count - 1);
		}
	}
}
