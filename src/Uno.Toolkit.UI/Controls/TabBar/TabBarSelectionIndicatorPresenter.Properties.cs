#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	partial class TabBarSelectionIndicatorPresenter
	{
		#region DependencyProperty: Owner

		public static DependencyProperty OwnerProperty { get; } = DependencyProperty.Register(
			nameof(Owner),
			typeof(TabBar),
			typeof(TabBarSelectionIndicatorPresenter),
			new PropertyMetadata(null, OnPropertyChanged));

		public TabBar Owner
		{
			get => (TabBar)GetValue(OwnerProperty);
			set => SetValue(OwnerProperty, value);
		}

		#endregion
		#region DependencyProperty: IndicatorTransitionMode

		public static DependencyProperty IndicatorTransitionModeProperty { get; } = DependencyProperty.Register(
			nameof(IndicatorTransitionMode),
			typeof(IndicatorTransitionMode),
			typeof(TabBarSelectionIndicatorPresenter),
			new PropertyMetadata(IndicatorTransitionMode.Snap));

		public IndicatorTransitionMode IndicatorTransitionMode
		{
			get => (IndicatorTransitionMode)GetValue(IndicatorTransitionModeProperty);
			set => SetValue(IndicatorTransitionModeProperty, value);
		}

		#endregion
		#region DependencyProperty: TemplateSettings

		public static DependencyProperty TemplateSettingsProperty { get; } = DependencyProperty.Register(
			nameof(TemplateSettings),
			typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
			typeof(TabBarSelectionIndicatorPresenter),
			new PropertyMetadata(null));

		public TabBarSelectionIndicatorPresenterTemplateSettings TemplateSettings
		{
			get => (TabBarSelectionIndicatorPresenterTemplateSettings)GetValue(TemplateSettingsProperty);
			set => SetValue(TemplateSettingsProperty, value);
		}

		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			if (sender is TabBarSelectionIndicatorPresenter owner)
			{
				owner.OnPropertyChanged(args);
			}
		}
	}
}
