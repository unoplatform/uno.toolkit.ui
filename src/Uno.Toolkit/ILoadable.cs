using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit
{
	/// <summary>
	/// Describes if this instance is currently in a busy state and notifies subscribers that said state when has changed.
	/// </summary>
	public interface ILoadable
	{
		/// <summary>
		/// Indicates whether the instance is doing work.
		/// </summary>
		bool IsExecuting { get; }

		/// <summary>
		/// Event that fires when the <see cref="IsExecuting"/> state has changed.
		/// </summary>
		event EventHandler? IsExecutingChanged;
	}
}
