using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Uno.Toolkit.UI;

public class ShadowCollection : ObservableCollection<Shadow>
{
	public bool HasInnerShadow() => this.Any(s => s.IsInner);

	public string ToKey(double width, double height, Windows.UI.Color? contentBackground)
		=> $"w{width.ToString(CultureInfo.InvariantCulture)},h{height.ToString(CultureInfo.InvariantCulture)}" +
		(contentBackground.HasValue ? $",cb{contentBackground.Value}:" : ":") +
		string.Join("/", this.Select(x => x.ToKey()));
}
