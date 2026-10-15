using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class ToolkitTheme
{
	public static partial class SkeletonView
	{
		public static partial class Resources
		{
			public static partial class Default
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "SkeletonViewBackground")]
					public static ThemeResourceKey<Brush> Default => new("SkeletonViewBackground");
				}

				public static partial class ShimmerBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "SkeletonViewShimmerBrush")]
					public static ThemeResourceKey<Brush> Default => new("SkeletonViewShimmerBrush");
				}

				[ResourceKeyDefinition(typeof(CornerRadius), "SkeletonViewCornerRadius")]
				public static ThemeResourceKey<CornerRadius> CornerRadius => new("SkeletonViewCornerRadius");
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "DefaultSkeletonView", TargetType = typeof(global::Uno.Toolkit.UI.SkeletonView))]
			public static StaticResourceKey<Style> Default => new("DefaultSkeletonView");

			[ResourceKeyDefinition(typeof(Style), "DefaultSkeletonPresenter", TargetType = typeof(global::Uno.Toolkit.UI.SkeletonPresenter))]
			public static StaticResourceKey<Style> Presenter => new("DefaultSkeletonPresenter");
		}
	}
}
