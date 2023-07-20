using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Uno.Toolkit.Samples")]
#if IS_WINUI
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Windows.Desktop")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Mobile")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.WASM")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Skia.Gtk")]
#else
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.UWP")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.Mobile")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.WASM")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.Skia.Gtk")]
#endif

[assembly: AssemblyMetadata("IsTrimmable", "True")]

[assembly: InternalsVisibleTo("Uno.Toolkit.RuntimeTests")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI.Material")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI.Cupertino")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI")]
