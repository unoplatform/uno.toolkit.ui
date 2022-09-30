#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	partial class NavigationBarPresenter
	{
		
		public static DependencyProperty ContentProperty { get; } = DependencyProperty.Register(
			nameof(Content),
			typeof(object),
			typeof(NavigationBarPresenter),
			new PropertyMetadata(default));

		/// <summary>
		/// Gets or sets the NavigationBar Content
		/// </summary>
		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

	}
}
