using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Uno.Toolkit.Samples")]
#if IS_WINUI
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Droid")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.iOS")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Mobile")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Windows.Desktop")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.macOS")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.WASM")]
[assembly: InternalsVisibleTo("Uno.Toolkit.WinUI.Samples.Skia.Gtk")]
#else
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.Droid")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.iOS")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.UWP")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.macOS")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.WASM")]
[assembly: InternalsVisibleTo("Uno.Toolkit.Samples.Skia.Gtk")]
#endif
