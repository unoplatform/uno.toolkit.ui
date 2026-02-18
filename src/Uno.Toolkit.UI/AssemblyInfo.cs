using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("CupertinoSampleApp")]
[assembly: InternalsVisibleTo("MaterialSampleApp")]

[assembly: AssemblyMetadata("IsTrimmable", "True")]

[assembly: InternalsVisibleTo("Uno.Toolkit.RuntimeTests")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI.Material")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI.Cupertino")]
[assembly: InternalsVisibleTo("Uno.Toolkit.UI")]

#if IS_WINUI
[assembly: InternalsVisibleTo("Uno.Toolkit.Skia.WinUI")]
#else
[assembly: InternalsVisibleTo("Uno.Toolkit.Skia.UI")]
#endif
