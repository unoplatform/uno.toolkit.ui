using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.Disposables;

namespace Uno.Toolkit.UI;

public partial class ShadowContainer : ContentControl
{
	#region DependencyProperty: Shadows

	public static DependencyProperty ShadowsProperty { get; } = DependencyProperty.Register(
		nameof(Shadows),
		typeof(ShadowCollection),
		typeof(ShadowContainer),
		new PropertyMetadata(default(ShadowCollection)));

	/// <summary>
	/// The collection of shadows that will be displayed under your control.
	/// A ShadowCollection can be stored in a resource dictionary to have a consistent style through your app.
	/// The ShadowCollection implements INotifyCollectionChanged.
	/// </summary>
	public ShadowCollection Shadows
	{
		get => (ShadowCollection)GetValue(ShadowsProperty);
		set => SetValue(ShadowsProperty, value);
	}

	#endregion
}
