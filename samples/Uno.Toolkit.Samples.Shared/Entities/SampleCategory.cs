using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.Samples.Entities
{
	public enum SampleCategory
	{
		/// <summary>
		/// Reserved for samples placed on top with no category, eg: Home, Overview
		/// </summary>
		None,

		/// <summary>
		/// Samples featuring bahaviors (static class with attached properties) that enhance an existing control.
		/// </summary>
		Behaviors,

		/// <summary>
		/// Samples featuring controls.
		/// </summary>
		Controls,

		/// <summary>
		/// Samples featuring static helper classes, markup-extensions, and other uncategorized stuffs.
		/// </summary>
		Helpers,

		/// <summary>
		/// Samples uses explicitly for UI Testing purposes, not to be discoverable by default.
		/// </summary>
		Tests,
	}
}
