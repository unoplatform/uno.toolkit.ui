using System.Collections.ObjectModel;
using System.Linq;

namespace Uno.Toolkit.UI;

public class ShadowCollection : ObservableCollection<Shadow>
{
	public string ToKey(double width, double height) =>
		$"w{width},h{height}:" +
		string.Concat(this.Select(x => x.ToKey() + "/"));
}
