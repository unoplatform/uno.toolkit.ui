using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.Samples.Entities
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class SamplePageAttribute : Attribute
	{
		public SamplePageAttribute(SampleCategory category, string title)
		{
			Category = category;
			Title = title;
		}

		/// <summary>
		/// Sample category with null reserved for Home/Overview.
		/// </summary>
		public SampleCategory Category { get; }

		public string Title { get; }

		public string Description { get; set; }

		public string DocumentationLink { get; set; }

		public Type DataType { get; set; }

		/// <summary>
		/// Sort order with the same <see cref="Category"/>.
		/// </summary>
		public int SortOrder { get; set; } = int.MaxValue;
	}
}
