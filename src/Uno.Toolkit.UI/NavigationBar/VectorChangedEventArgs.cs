using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;

namespace Uno.UI.ToolkitLib
{
	internal class VectorChangedEventArgs : IVectorChangedEventArgs
	{
		public VectorChangedEventArgs(CollectionChange change, uint index)
		{
			CollectionChange = change;
			Index = index;
		}

		public CollectionChange CollectionChange { get; }

		public uint Index { get; }
	}
}
