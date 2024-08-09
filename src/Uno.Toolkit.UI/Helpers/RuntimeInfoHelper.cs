using System.Runtime.InteropServices;

namespace Uno.Toolkit.UI
{
	internal static class RuntimeInfoHelper
	{
		public static bool IsBrowser { get; } =
			// Origin of the value : https://github.com/mono/mono/blob/a65055dbdf280004c56036a5d6dde6bec9e42436/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L115
			RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")) || // Legacy Value (Bootstrapper 1.2.0-dev.29 or earlier).
			RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
	}
}
