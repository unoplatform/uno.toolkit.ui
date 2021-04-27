using System;
using Microsoft.UI.Xaml;
using Uno.Toolkit.Samples;

namespace Uno.Toolkit.WinUI.Samples.Wasm
{
	public class Program
	{
		private static App _app;

		static int Main(string[] args)
		{
			Microsoft.UI.Xaml.Application.Start(_ => _app = new App());

			return 0;
		}
	}
}
