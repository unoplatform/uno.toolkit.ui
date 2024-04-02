using Microsoft.UI.Xaml;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class ToolkitTheme
{
	public static partial class ChipGroup
	{
		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "InputChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Input => new("InputChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedSuggestion => new("ElevatedSuggestionChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "SuggestionChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Suggestion => new("SuggestionChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedFilter => new("ElevatedFilterChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "FilterChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Filter => new("FilterChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> ElevatedAssist => new("ElevatedAssistChipGroupStyle");

			[ResourceKeyDefinition(typeof(Style), "AssistChipGroupStyle", TargetType = typeof(global::Uno.Toolkit.UI.ChipGroup))]
			public static StaticResourceKey<Style> Assist => new("AssistChipGroupStyle");
		}
	}
}
