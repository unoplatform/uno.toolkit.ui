using System;
using Windows.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup;

public static partial class Theme
{
	public static partial class ChipGroup
	{
		public static partial class Resources
		{
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "MaterialInputChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Input => new("MaterialInputChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialElevatedSuggestionChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedSuggestion => new("MaterialElevatedSuggestionChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialSuggestionChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Suggestion => new("MaterialSuggestionChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialElevatedFilterChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedFilter => new("MaterialElevatedFilterChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialFilterChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Filter => new("MaterialFilterChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialElevatedAssistChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedAssist => new("MaterialElevatedAssistChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialAssistChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Assist => new("MaterialAssistChipGroupStyle");
		}
	}
}
