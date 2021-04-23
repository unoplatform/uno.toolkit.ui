using AppKit;
using Uno.Toolkit.Samples;

namespace Uno.Toolkit.WinUI.Samples.macOS
{
	static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
			NSApplication.SharedApplication.Delegate = new App();
			NSApplication.Main(args);  
		}
	}
}

