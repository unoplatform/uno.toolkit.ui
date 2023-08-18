using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Uno.Toolkit.UI;

<<<<<<< HEAD
public class ShadowCollection : ObservableCollection<Shadow>
{
	public bool HasInnerShadow() => this.Any(s => s.IsInner);

	public string ToKey(double width, double height, Windows.UI.Color? contentBackground)
		=> string.Format(CultureInfo.InvariantCulture, $"w{width},h{height}") +
		(contentBackground.HasValue ? $",cb{contentBackground.Value}:" : ":") +
		string.Join("/", this.Select(x => x.ToKey()));
}
=======
public class ShadowCollection : ObservableCollection<Shadow> { }
>>>>>>> 6d09cf1 (fix(shadows): background theming issue)
