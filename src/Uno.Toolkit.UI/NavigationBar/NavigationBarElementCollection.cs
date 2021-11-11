using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.UI.ToolkitLib
{
	internal class NavigationBarElementCollection : IObservableVector<ICommandBarElement>
	{
		private readonly List<ICommandBarElement> _list = new List<ICommandBarElement>();

		public ICommandBarElement this[int index]
		{
			get => _list[index];
			set => SetAt(index, value);
		}

		private void SetAt(int index, ICommandBarElement item)
		{
			_list[index] = item;
			RaiseVectorChanged(CollectionChange.ItemChanged, index);
		}

		public int Count => _list.Count;

		public bool IsReadOnly => ((ICollection<ICommandBarElement>)_list).IsReadOnly;

		public event VectorChangedEventHandler<ICommandBarElement>? VectorChanged;

		public void Add(ICommandBarElement item) => Append(item);
		public void Append(ICommandBarElement item)
		{
			Insert(Count, item);
		}

		public void Clear()
		{
			_list.Clear();
			RaiseVectorChanged(CollectionChange.Reset, 0);
		}
		public bool Contains(ICommandBarElement item) => _list.Contains(item);

		public void CopyTo(ICommandBarElement[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		public IEnumerator<ICommandBarElement> GetEnumerator() => _list.GetEnumerator();

		public int IndexOf(ICommandBarElement item) => _list.IndexOf(item);

		public void Insert(int index, ICommandBarElement item)
		{
			_list.Insert(index, item);
			RaiseVectorChanged(CollectionChange.ItemInserted, index);
		}

		public bool Remove(ICommandBarElement item)
		{
			var index = _list.IndexOf(item);

			if (index != -1)
			{
				RemoveAt(index);

				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
			RaiseVectorChanged(CollectionChange.ItemRemoved, index);
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		private void RaiseVectorChanged(CollectionChange change, int changeIndex)
		{
			VectorChangedEventArgs spArgs;

			spArgs = new VectorChangedEventArgs(change, (uint)changeIndex);
			VectorChanged?.Invoke(this, spArgs);
		}
	}
}
