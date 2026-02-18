namespace Uno.Toolkit.Samples.Entities
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class SamplePageAttribute : Attribute
	{
		public SamplePageAttribute(SampleCategory category, string title, SourceSdk source = SourceSdk.WinUI)
		{
			Category = category;
			Title = title;
			Source = source;
		}

		/// <summary>
		/// Sample category with null reserved for Home/Overview.
		/// </summary>
		public SampleCategory Category { get; }

		/// <remarks>
		/// Symbol will take precedence over Path if specified.
		/// Attribute property can only be primitive value, nullable not included. So 'default' is used in lieu.
		/// </remarks>
		public Symbol IconSymbol { get; set; } = default;

		public string IconPath { get; set; }

		public string Title { get; }

		public string Description { get; set; }

		public string DocumentationLink { get; set; }

		/// <summary>
		/// The type of ViewModel associated with the sample. Make sure the type has a parameterless ctor, as it will be <see cref="Activator.CreateInstance"/>'d.
		/// </summary>
		/// <remarks>The DataContext of the page will be an instance of <see cref="Sample"/>. This DataType will be available from <see cref="Sample.Data"/>.</remarks>
		public Type DataType { get; set; }

		public SourceSdk Source { get; }

		/// <summary>
		/// Sort order with the same <see cref="Category"/>.
		/// </summary>
		public int SortOrder { get; set; } = int.MaxValue;
	}
}
