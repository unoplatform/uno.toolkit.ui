using System;
using System.Collections.Specialized;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents a collection of <see cref="EventToCommand"/> objects.
	/// Used with <see cref="CommandExtensions.EventCommandsProperty"/> to map multiple events to commands on a single element.
	/// </summary>
	public class EventToCommandCollection : DependencyObjectCollection
	{
		private DependencyObject? _associatedObject;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventToCommandCollection"/> class.
		/// </summary>
		public EventToCommandCollection()
		{
			VectorChanged += OnVectorChanged;
		}

		/// <summary>
		/// Associates all items in the collection with a target element.
		/// </summary>
		internal void Attach(DependencyObject associatedObject)
		{
			if (_associatedObject == associatedObject)
			{
				return;
			}

			Detach();
			_associatedObject = associatedObject;

			foreach (var item in this)
			{
				if (item is EventToCommand etc)
				{
					etc.Attach(associatedObject);
				}
			}
		}

		/// <summary>
		/// Detaches all items in the collection from the associated element.
		/// </summary>
		internal void Detach()
		{
			foreach (var item in this)
			{
				if (item is EventToCommand etc)
				{
					etc.Detach();
				}
			}

			_associatedObject = null;
		}

		private void OnVectorChanged(Windows.Foundation.Collections.IObservableVector<DependencyObject> sender, Windows.Foundation.Collections.IVectorChangedEventArgs e)
		{
			if (_associatedObject is null)
			{
				return;
			}

			switch (e.CollectionChange)
			{
				case Windows.Foundation.Collections.CollectionChange.ItemInserted:
					if ((int)e.Index < Count && this[(int)e.Index] is EventToCommand newItem)
					{
						newItem.Attach(_associatedObject);
					}
					break;

				case Windows.Foundation.Collections.CollectionChange.ItemRemoved:
					// The item has already been removed, so we can't access it
					// Items are detached in their finalizer or when explicitly detached
					break;

				case Windows.Foundation.Collections.CollectionChange.ItemChanged:
					if ((int)e.Index < Count && this[(int)e.Index] is EventToCommand changedItem)
					{
						changedItem.Attach(_associatedObject);
					}
					break;

				case Windows.Foundation.Collections.CollectionChange.Reset:
					foreach (var item in this)
					{
						if (item is EventToCommand etc)
						{
							etc.Attach(_associatedObject);
						}
					}
					break;
			}
		}
	}
}
