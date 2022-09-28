#if __IOS__
using Foundation;

namespace Uno.Toolkit.UI.Helpers;

internal static class DictionaryExtensions
{
	public static NSDictionary<TKey, TValue> ToNSDictionary<TKey, TValue>(this NSDictionary dict)
		where TKey : class, ObjCRuntime.INativeObject
		where TValue: class, ObjCRuntime.INativeObject
		=> NSDictionary<TKey, TValue>.FromObjectsAndKeys(dict.Values, dict.Keys);
}
#endif
