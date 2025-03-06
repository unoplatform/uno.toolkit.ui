#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

public partial class DockItem : DependencyObject
{
	#region DependencyProperty: Header

	public static DependencyProperty HeaderProperty { get; } = DependencyProperty.Register(
		nameof(Header),
		typeof(object),
		typeof(DockItem),
		new PropertyMetadata(default(object)));

	public object Header
	{
		get => (object)GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	#endregion
	#region DependencyProperty: Title

	public static DependencyProperty TitleProperty { get; } = DependencyProperty.Register(
		nameof(Title),
		typeof(object),
		typeof(DockItem),
		new PropertyMetadata(default(object)));

	public object Title
	{
		get => (object)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	#endregion
	#region DependencyProperty: Content

	public static DependencyProperty ContentProperty { get; } = DependencyProperty.Register(
		nameof(Content),
		typeof(object),
		typeof(DockItem),
		new PropertyMetadata(default(object)));

	public object Content
	{
		get => (object)GetValue(ContentProperty);
		set => SetValue(ContentProperty, value);
	}

	#endregion
}

public partial class DocumentItem : DockItem{ }
public partial class ToolItem : DockItem { }
