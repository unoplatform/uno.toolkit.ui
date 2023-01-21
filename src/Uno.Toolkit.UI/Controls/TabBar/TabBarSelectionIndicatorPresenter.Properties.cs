#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	partial class TabBarSelectionIndicatorPresenter
	{
		#region IndicatorTransitionMode Dependency Property
		public IndicatorTransitionMode IndicatorTransitionMode
		{
			get { return (IndicatorTransitionMode)GetValue(IndicatorTransitionModeProperty); }
			set { SetValue(IndicatorTransitionModeProperty, value); }
		}

		public static DependencyProperty IndicatorTransitionModeProperty { get; } =
			DependencyProperty.Register(nameof(IndicatorTransitionMode), typeof(IndicatorTransitionMode), typeof(TabBarSelectionIndicatorPresenter), new PropertyMetadata(IndicatorTransitionMode.Snap));
		#endregion

		#region Owner Depdendency Property
		public TabBar Owner
		{
			get { return (TabBar)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}

		public static DependencyProperty OwnerProperty { get; } =
			DependencyProperty.Register(nameof(Owner), typeof(TabBar), typeof(TabBarSelectionIndicatorPresenter), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region TemplateSettings
		public TabBarSelectionIndicatorPresenterTemplateSettings TemplateSettings
		{
			get => (TabBarSelectionIndicatorPresenterTemplateSettings)GetValue(TemplateSettingsProperty);
			private set => SetValue(TemplateSettingsProperty, value);
		}
		public static DependencyProperty TemplateSettingsProperty { get; } =
			DependencyProperty.Register(nameof(TemplateSettings), typeof(TabBarSelectionIndicatorPresenterTemplateSettings), typeof(TabBarSelectionIndicatorPresenter), new PropertyMetadata(null));
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
