using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI
{
	#region Chip event handlers
	public delegate void ChipRemovingEventHandler(object sender, ChipRemovingEventArgs e);
	public class ChipRemovingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets whether the <see cref="Chip.Removed"/> event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}

	public delegate void ChipRemovedEventHandler(object sender, EventArgs e);
	#endregion


	#region ChipGroup event handlers
	public delegate void ChipItemEventHandler(object sender, ChipItemEventArgs e);
	public class ChipItemEventArgs : EventArgs
	{
		internal ChipItemEventArgs(object item) => Item = item;

		/// <summary>
		/// Gets the item associated with the event
		/// </summary>
		public object Item { get; }
	}

	public delegate void ChipItemRemovingEventHandler(object sender, ChipItemRemovingEventArgs e);
	public class ChipItemRemovingEventArgs : ChipItemEventArgs
	{
		public ChipItemRemovingEventArgs(object item) : base(item) { }

		/// <summary>
		/// Gets or sets whether the <see cref="ChipGroup.ItemRemoved"/> event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}

	#endregion
}
