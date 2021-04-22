using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace Uno.Toolkit.UI.Samples.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new Uno.Toolkit.UI.Samples.App(), args);
			host.Run();
		}
	}
}
