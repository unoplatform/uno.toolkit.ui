using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Uno.Toolkit.UI;

public class ShadowCollection : ObservableCollection<Shadow>
{
<<<<<<< HEAD
	public bool HasInnerShadow() => this.Any(s => s.IsInner);

	public string ToKey(double width, double height, Windows.UI.Color? contentBackground) =>
		FormattableString.Invariant($"w{width},h{height}") +
		(contentBackground.HasValue ? $",cb{contentBackground.Value}:" : ":") +
		string.Join("/", this.Select(x => x.ToKey()));
=======
	public string ToKey() => string.Join("/", this.Select(x => x.ToKey()));
>>>>>>> f633ff0 (fix(shadows): background handling)
}
