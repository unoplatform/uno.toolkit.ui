using System.Collections.ObjectModel;
using System.Linq;

namespace Uno.Toolkit.UI;

public class ShadowCollection : ObservableCollection<Shadow>
{
    public string ToKey(double width, double height) => this.Aggregate($"w{width},h{height}", (key, shadow) => key + shadow.ToKey());
}