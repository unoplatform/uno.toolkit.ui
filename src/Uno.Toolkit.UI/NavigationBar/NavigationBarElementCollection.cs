using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;
using AppBarButton = Microsoft.UI.Xaml.Controls.AppBarButton;

namespace Uno.UI.ToolkitLib
{
	internal class NavigationBarElementCollection : IObservableVector<AppBarButton>
	{
		private readonly List<AppBarButton> _list = new List<AppBarButton>();

		public AppBarButton this[int index]
		{
			get => _list[index];
			set => SetAt(index, value);
		}

		private void SetAt(int index, AppBarButton item)
		{
			_list[index] = item;
			RaiseVectorChanged(CollectionChange.ItemChanged, index);
		}

		public int Count => _list.Count;

		public bool IsReadOnly => ((ICollection<AppBarButton>)_list).IsReadOnly;

		public event VectorChangedEventHandler<AppBarButton>? VectorChanged;

		public void Add(AppBarButton item) => Append(item);
		public void Append(AppBarButton item)
		{
			Insert(Count, item);
		}

		public void Clear()
		{
			_list.Clear();
			RaiseVectorChanged(CollectionChange.Reset, 0);
		}
		public bool Contains(AppBarButton item) => _list.Contains(item);

		public void CopyTo(AppBarButton[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		public IEnumerator<AppBarButton> GetEnumerator() => _list.GetEnumerator();

		public int IndexOf(AppBarButton item) => _list.IndexOf(item);

		public void Insert(int index, AppBarButton item)
		{
			_list.Insert(index, item);
			RaiseVectorChanged(CollectionChange.ItemInserted, index);
		}

		public bool Remove(AppBarButton item)
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
