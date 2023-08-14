using System;
using Uno.Extensions.Markup;

namespace Uno.Toolkit.Markup
{
	public readonly record struct ResourceValue<T>
	{
		public ResourceValue(string key, bool isThemeResource = false)
		{
			Key = key;
			IsThemeResource = isThemeResource;
		}

		public string Key { get; }

		public bool IsThemeResource { get; }

		public static implicit operator Action<IDependencyPropertyBuilder<T>>(ResourceValue<T> resource) =>
			resource.IsThemeResource ?
				ThemeResource.Get<T>(resource.Key)
				: StaticResource.Get<T>(resource.Key);

	}
}
