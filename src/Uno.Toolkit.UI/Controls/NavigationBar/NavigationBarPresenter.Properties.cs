#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	partial class NavigationBarPresenter
	{
		
		public static DependencyProperty NavigationBarContentProperty { get; } = DependencyProperty.Register(
			nameof(NavigationBarContent),
			typeof(object),
			typeof(NavigationBarPresenter),
			new PropertyMetadata(default));

		/// <summary>
		/// Gets or sets the NavigationBar Content
		/// </summary>
		public object NavigationBarContent
		{
			get => GetValue(NavigationBarContentProperty);
			set => SetValue(NavigationBarContentProperty, value);
		}

	}
}
