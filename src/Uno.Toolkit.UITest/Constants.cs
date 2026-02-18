using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest
{
	public class Constants
	{
		public readonly static string WebAssemblyDefaultUri = "http://localhost:5000/";
		public readonly static string iOSAppName = "uno.platform.toolkit.material";
		public readonly static string AndroidAppName = "uno.platform.toolkit.material";
		public readonly static string iOSDeviceNameOrId = "iPad (10th generation)";

		public readonly static Platform CurrentPlatform = Platform.Android;
	}
}
