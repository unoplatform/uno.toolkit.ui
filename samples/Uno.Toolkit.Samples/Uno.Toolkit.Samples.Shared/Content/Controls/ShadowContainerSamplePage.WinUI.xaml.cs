#if IS_WINUI
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(ShadowContainer), Description = "Add many colored shadows to your controls.")]
	public sealed partial class ShadowContainerSamplePage : Page
	{
		private ShadowCollection _shadows;

		public ShadowContainerSamplePage()
		{
			this.InitializeComponent();

			this.Loaded += (s, e) =>
			{
				if (SamplePageLayout.GetSampleChild<ShadowContainer>(Design.Agnostic, "ShadowContainer") is { } shadowContainer)
				{
					_shadows = shadowContainer.Shadows;

					var shadowsItemsControl = SamplePageLayout.GetSampleChild<ItemsControl>(Design.Agnostic, "ShadowsItemsControl");
					shadowsItemsControl.ItemsSource = _shadows;
				}
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
#endif
