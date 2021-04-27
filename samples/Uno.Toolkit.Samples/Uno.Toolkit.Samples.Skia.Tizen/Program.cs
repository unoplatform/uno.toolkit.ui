using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace Uno.Toolkit.Samples.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new Uno.Toolkit.Samples.App(), args);
			host.Run();
		}
	}
}
