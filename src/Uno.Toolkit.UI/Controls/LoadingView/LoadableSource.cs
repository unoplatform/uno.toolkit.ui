using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;
using Uno.Toolkit;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents an <see cref="ILoadable" /> that forwards the <see cref="ILoadable.IsExecuting"/> state of its <see cref="Source" />.
	/// </summary>
	public partial class LoadableSource : FrameworkElement, ILoadable
	{
		public event EventHandler? IsExecutingChanged;

		#region DependencyProperty: Source

		public static DependencyProperty SourceProperty { get; } = DependencyProperty.Register(
			nameof(Source),
			typeof(ILoadable),
			typeof(LoadableSource),
			new PropertyMetadata(default(ILoadable), (s, e) => ((LoadableSource)s).OnSourceChanged(e)));

		/// <summary>
		/// Gets and sets the <see cref="ILoadable" /> to forward its state.
		/// </summary>
		public ILoadable Source
		{
			get => (ILoadable)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		#endregion
		#region DependencyProperty: IsExecuting

		public static DependencyProperty IsExecutingProperty { get; } = DependencyProperty.Register(
			nameof(IsExecuting),
			typeof(bool),
			typeof(LoadableSource),
			new PropertyMetadata(default(bool), (s, e) => ((LoadableSource)s).OnIsExecutingChanged(e)));

		public bool IsExecuting
		{
			get => (bool)GetValue(IsExecutingProperty);
			set => SetValue(IsExecutingProperty, value);
		}

		#endregion

		private readonly SerialDisposable _subscription = new();

		private void OnIsExecutingChanged(DependencyPropertyChangedEventArgs e)
		{
			IsExecutingChanged?.Invoke(this, new());
		}

		private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			var source = Source;

			_subscription.Disposable = source?.BindIsExecuting(x => IsExecuting = x, propagateInitialValue: false);
			IsExecuting = source?.IsExecuting ?? false;
		}
	}
}
