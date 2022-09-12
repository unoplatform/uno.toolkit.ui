using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents an <see cref="ILoadable" /> aggregate that is <see cref="ILoadable.IsExecuting"/>
	/// when any of its nested <see cref="Sources" /> is <see cref="ILoadable.IsExecuting"/>.
	/// </summary>
	[ContentProperty(Name = nameof(Sources))]
	public partial class CompositeLoadableSource : FrameworkElement, ILoadable
	{
		public event EventHandler? IsExecutingChanged;

		#region DependencyProperty: Sources [get-only]

		public static DependencyProperty SourcesProperty { get; } = DependencyProperty.Register(
			nameof(Sources),
			typeof(ObservableCollection<ILoadable>),
			typeof(CompositeLoadableSource),
			new PropertyMetadata(default(ObservableCollection<ILoadable>), (s, e) => ((CompositeLoadableSource)s).OnSourcesChanged(e)));

		/// <summary>
		/// Gets and sets the collection of nested <see cref="ILoadable" />.
		/// </summary>
		public ObservableCollection<ILoadable> Sources
		{
			get => (ObservableCollection<ILoadable>)GetValue(SourcesProperty);
			private set => SetValue(SourcesProperty, value);
		}

		#endregion
		#region DependencyProperty: IsExecuting

		public static DependencyProperty IsExecutingProperty { get; } = DependencyProperty.Register(
			nameof(IsExecuting),
			typeof(bool),
			typeof(CompositeLoadableSource),
			new PropertyMetadata(default(bool), (s, e) => ((CompositeLoadableSource)s).OnIsExecutingChanged(e)));

		public bool IsExecuting
		{
			get => (bool)GetValue(IsExecutingProperty);
			set => SetValue(IsExecutingProperty, value);
		}

		#endregion

		private readonly ConcurrentDictionary<ILoadable, IDisposable> _sourcesInnerSubcriptions = new();
		private readonly SerialDisposable _sourceCollectionSubscription = new();

		public CompositeLoadableSource()
		{
			Sources = new ObservableCollection<ILoadable>();
			Loaded += (s, e) => SubscribeAll();
			Unloaded += (s, e) => UnsubscribeAll();
		}

		private void OnSourcesChanged(DependencyPropertyChangedEventArgs e)
		{
			UnsubscribeAll();
			SubscribeAll();
		}

		private void SubscribeAll()
		{
			if (Sources is { })
			{
				Sources.CollectionChanged += OnSourcesCollectionChanged;

				// there would be items already in the collection from xaml collection initializer (uno-specific)
				AddInnerSubscriptions(Sources);
			}
		}

		private void UnsubscribeAll()
		{
			_sourceCollectionSubscription.Disposable = null;
			ClearInnerSubscriptions();
		}

		private void OnSourcesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move) return;
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				ClearInnerSubscriptions();

				// normally, reset is caused by clear, but just in case...
				AddInnerSubscriptions(Sources);
			}
			else
			{
				AddInnerSubscriptions(e.NewItems?.Cast<ILoadable>() ?? Enumerable.Empty<ILoadable>());
				RemoveInnerSubscriptions(e.OldItems?.Cast<ILoadable>() ?? Enumerable.Empty<ILoadable>());
			}
		}

		private void AddInnerSubscriptions(IEnumerable<ILoadable> items)
		{
			foreach (var item in items)
			{
				(item as FrameworkElement)?.InheritDataContextFrom(this);
				_sourcesInnerSubcriptions.GetOrAdd(item, x => x.BindIsExecuting(UpdateOnDispatcher, propagateInitialValue: true));
			}
			
			void UpdateOnDispatcher()
			{
				if (Dispatcher.HasThreadAccess)
				{
					Update();
				}
				else
				{
					_ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Update);
				}
			}
		}

		private void RemoveInnerSubscriptions(IEnumerable<ILoadable> items)
		{
			foreach (var item in items)
			{
				(item as FrameworkElement)?.ClearValue(FrameworkElement.DataContextProperty);
				if (_sourcesInnerSubcriptions.TryGetValue(item, out var disposable))
					disposable.Dispose();
			}
		}

		private void ClearInnerSubscriptions()
		{
			foreach (var kvp in _sourcesInnerSubcriptions)
			{
				(kvp.Key as FrameworkElement)?.ClearValue(FrameworkElement.DataContextProperty);
				kvp.Value.Dispose();
			}

			_sourcesInnerSubcriptions.Clear();
		}

		private void OnIsExecutingChanged(DependencyPropertyChangedEventArgs e) => IsExecutingChanged?.Invoke(this, new());

		private void Update()
		{
			IsExecuting = Sources.Any(x => x.IsExecuting);
		}
	}
}
