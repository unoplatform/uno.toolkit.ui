namespace Uno.Toolkit.WrapperApp.GuestHosting;

/// <summary>
/// Describes a theme sample app that this wrapper can host in a secondary AssemblyLoadContext.
/// </summary>
/// <param name="DisplayName">Name shown on the app-picker button.</param>
/// <param name="ProjectFolderName">Folder name under <c>samples/</c> (also the guest payload folder name).</param>
/// <param name="AssemblyName">Simple name of the guest's entry assembly (without extension).</param>
internal sealed record GuestAppInfo(string DisplayName, string ProjectFolderName, string AssemblyName);

/// <summary>
/// The set of theme sample apps hostable by this wrapper.
/// </summary>
internal static class GuestAppCatalog
{
	/// <summary>
	/// Gets the hostable theme sample apps, in picker order.
	/// </summary>
	/// <remarks>
	/// Keep this catalog aligned with ToolkitSampleApp.csproj's guest payloads and desktop
	/// project references, and build/workflow/scripts/build-wasm-guest-heads.sh.
	/// </remarks>
	public static IReadOnlyList<GuestAppInfo> Apps { get; } =
	[
		new GuestAppInfo("Material", "Uno.Toolkit.Samples.Material", "MaterialSampleApp"),
		new GuestAppInfo("Cupertino", "Uno.Toolkit.Samples.Cupertino", "CupertinoSampleApp"),
		new GuestAppInfo("Simple", "Uno.Toolkit.Samples.Simple", "SimpleSampleApp"),
	];
}
