using Windows.Foundation.Metadata;

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material Design styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	/// <remarks>
	/// This class is like an alias for the latest version of MaterialToolkitResources,
	/// which is currently pointing to <see cref="MaterialToolkitResourcesV2"/>.
	/// </remarks>
	[Deprecated("Resource initialization for the Toolkit Material theme should now be done using the MaterialToolkitTheme class instead.", DeprecationType.Deprecate, 3)]
	public sealed class MaterialToolkitResources : MaterialToolkitResourcesV2
	{
	}
}
