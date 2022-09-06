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

		#region DependencyProperty: Sources

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
			set => SetValue(SourcesProperty, value);
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

		private readonly ConcurrentDictionary<ILoadable, IDisposable> _sourceSubcriptions = new();
		private readonly SerialDisposable _sourceCollectionSubscription = new();

		public CompositeLoadableSource()
		{
			Sources = new ObservableCollection<ILoadable>();
		}

		private void OnSourcesChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue is ObservableCollection<ILoadable> oldCollection)
			{
				oldCollection.CollectionChanged -= OnSourcesCollectionChanged;
				ClearSubscriptions();
			}
			if (e.NewValue is ObservableCollection<ILoadable> newCollection)
			{
				newCollection.CollectionChanged += OnSourcesCollectionChanged;

				// there would be items already in the collection from xaml collection initializer (uno-specific)
				AddSubscriptions(newCollection);
			}
		}

		private void OnSourcesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move) return;
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				ClearSubscriptions();

				// normally, reset is caused by clear, but just in case...
				AddSubscriptions(Sources);
			}
			else
			{
				AddSubscriptions(e.NewItems?.Cast<ILoadable>() ?? Enumerable.Empty<ILoadable>());
				RemoveSubscriptions(e.OldItems?.Cast<ILoadable>() ?? Enumerable.Empty<ILoadable>());
			}
		}

		private void AddSubscriptions(IEnumerable<ILoadable> items)
		{
			foreach (var item in items)
			{
				(item as FrameworkElement)?.InheritDataContextFrom(this);
				_sourceSubcriptions.GetOrAdd(item, x => x.BindIsExecuting(Update, propagateInitialValue: true));
			}
		}

		private void RemoveSubscriptions(IEnumerable<ILoadable> items)
		{
			foreach (var item in items)
			{
				(item as FrameworkElement)?.ClearValue(FrameworkElement.DataContextProperty);
				if (_sourceSubcriptions.TryGetValue(item, out var disposable))
					disposable.Dispose();
			}
		}

		private void ClearSubscriptions()
		{
			foreach (var kvp in _sourceSubcriptions)
			{
				(kvp.Key as FrameworkElement)?.ClearValue(FrameworkElement.DataContextProperty);
				kvp.Value.Dispose();
			}

			_sourceSubcriptions.Clear();
		}

		private void OnIsExecutingChanged(DependencyPropertyChangedEventArgs e) => IsExecutingChanged?.Invoke(this, new());

		private void Update()
		{
			IsExecuting = Sources.Any(x => x.IsExecuting);
		}
	}
}
