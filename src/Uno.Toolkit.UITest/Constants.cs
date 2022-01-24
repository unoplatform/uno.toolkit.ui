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
        public readonly static string WebAssemblyDefaultUri = "http://localhost:59402/";
        public readonly static string iOSAppName = "com.nventive.Uno.Toolkit.Samples";
        public readonly static string AndroidAppName = "Uno.Toolkit.Samples";
        public readonly static string iOSDeviceNameOrId = "iPad Pro (12.9-inch) (4th generation)";

        public readonly static Platform CurrentPlatform = Platform.Android;
    }
}
