using Uno.Disposables;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents a control that indicates that the UI is waiting on a task to complete.
	/// </summary>
	[TemplateVisualState(GroupName = VisualStateNames.GroupName, Name = VisualStateNames.Loading)]
	[TemplateVisualState(GroupName = VisualStateNames.GroupName, Name = VisualStateNames.Loaded)]
	public partial class LoadingView : ContentControl
	{
		private class VisualStateNames
		{
			// LoadingStates
			public const string GroupName = "LoadingStates";
			public const string Loading = nameof(Loading);
			public const string Loaded = nameof(Loaded);
		}

		#region DependencyProperty: UseTransitions

		public static DependencyProperty UseTransitionsProperty { get; } = DependencyProperty.Register(
			nameof(UseTransitions),
			typeof(bool),
			typeof(LoadingView),
			new PropertyMetadata(true));

		/// <summary>
		/// Gets and sets the whether transitions will play when going between states.
		/// </summary>
		public bool UseTransitions
		{
			get => (bool)GetValue(UseTransitionsProperty);
			set => SetValue(UseTransitionsProperty, value);
		}

		#endregion

		#region DependencyProperty: Source

		public static DependencyProperty SourceProperty { get; } = DependencyProperty.Register(
			nameof(Source),
			typeof(ILoadable),
			typeof(LoadingView),
			new PropertyMetadata(default(ILoadable), (s, e) => ((LoadingView)s).OnSourceChanged(e)));

		/// <summary>
		/// Gets and sets the source <see cref="ILoadable" /> associated with this control.
		/// </summary>
		public ILoadable Source
		{
			get => (ILoadable)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		#endregion
		#region DependencyProperty: LoadingContent

		public static DependencyProperty LoadingContentProperty { get; } = DependencyProperty.Register(
			nameof(LoadingContent),
			typeof(object),
			typeof(LoadingView),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the content to be displayed during loading/waiting.
		/// </summary>
		public object LoadingContent
		{
			get => (object)GetValue(LoadingContentProperty);
			set => SetValue(LoadingContentProperty, value);
		}

		#endregion
		#region DependencyProperty: LoadingContentTemplate

		public static DependencyProperty LoadingContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(LoadingContentTemplate),
			typeof(DataTemplate),
			typeof(LoadingView),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the content template to be used when displaying LoadingContent during loading/waiting.
		/// </summary>
		public DataTemplate LoadingContentTemplate
		{
			get => (DataTemplate)GetValue(LoadingContentTemplateProperty);
			set => SetValue(LoadingContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: LoadingContentTemplateSelector

		public static DependencyProperty LoadingContentTemplateSelectorProperty { get; } = DependencyProperty.Register(
			nameof(LoadingContentTemplateSelector),
			typeof(DataTemplateSelector),
			typeof(LoadingView),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the content template to be used when displayin LoadingContent during loading/waiting.
		/// </summary>
		public DataTemplateSelector LoadingContentTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(LoadingContentTemplateSelectorProperty);
			set => SetValue(LoadingContentTemplateSelectorProperty, value);
		}

		#endregion

		private readonly SerialDisposable _subscription = new();
		private bool _isReady;
		private string _currentState = string.Empty;

		public LoadingView()
		{
			DefaultStyleKey = typeof(LoadingView);
		}
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_isReady = true;

			UpdateVisualState();
		}

		private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			(e.NewValue as FrameworkElement)?.InheritDataContextFrom(this);
			(e.OldValue as FrameworkElement)?.ClearValue(FrameworkElement.DataContextProperty);

			_subscription.Disposable = Source?.BindIsExecuting(UpdateVisualState);
		}

		private DataTemplate? loadingContentTemplatePlaceholder;
		private DataTemplateSelector? loadingContentTemplateSelectorPlaceholder;
		private void UpdateVisualState()
		{
			if (!_isReady) return;

			var targetState = Source?.IsExecuting ?? true
				? VisualStateNames.Loading
				: VisualStateNames.Loaded;
			if (targetState == _currentState)
			{
				return;
			}
			_currentState = targetState;

			if (_currentState == VisualStateNames.Loaded)
			{
				loadingContentTemplatePlaceholder = LoadingContentTemplate;
				loadingContentTemplateSelectorPlaceholder = LoadingContentTemplateSelector;
				// Clear content template
				LoadingContentTemplate = new DataTemplate();
				LoadingContentTemplateSelector = new DataTemplateSelector();
			}
			else
			{
				LoadingContentTemplate = loadingContentTemplatePlaceholder!;
				LoadingContentTemplateSelector = loadingContentTemplateSelectorPlaceholder!;

			}
			VisualStateManager.GoToState(this, targetState, IsLoaded && UseTransitions);


		}
	}
}
