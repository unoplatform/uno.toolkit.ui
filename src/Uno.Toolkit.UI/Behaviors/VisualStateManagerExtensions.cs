using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using System.Linq;
using System.Text.RegularExpressions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Provides a way of manipulating the visual states of <see cref="Control"/> with attached property.
/// </summary>
/// <remarks>
/// <seealso cref="VisualStateManager.GoToState"/> is typically used with <see cref="Control"/>
/// where you would set <see cref="VisualStateManager.VisualStateGroupsProperty"/> on the root element of the ControlTemplate.
/// Because this class directly calls that method, it means that if you are setting <see cref="OverrideStatesProperty"/> on an element,
/// the <see cref="VisualStateManager.VisualStateGroupsProperty"/> should not be set on the very same element, but its first child.
/// </remarks>
public static class VisualStateManagerExtensions
{
	private static readonly ILogger Logger = typeof(VisualStateManagerExtensions).Log();

	#region DependencyProperty: OverrideStates

	/// <summary>
	/// Identifies the OverrideStates dependency property.
	/// </summary>
	public static DependencyProperty OverrideStatesProperty { get; } = DependencyProperty.RegisterAttached(
		"OverrideStates",
		typeof(string),
		typeof(VisualStateManagerExtensions),
		new PropertyMetadata(default(string), OnOverrideStatesChanged));

	// We need to include both a Set{PropertyName} and a Get{PropertyName} method for attached properties on Windows.
	// Otherwise, you may end up with errors such as "System.Void should not be used in C#"
	// See: https://learn.microsoft.com/en-us/windows/uwp/xaml-platform/custom-attached-properties#registering-a-custom-attached-property

	/// <summary>
	/// Sets the visual states of the control.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="value">A space, comma or semi-colon separated list of visual state names</param>
	public static void SetOverrideStates(Control obj, string value) => obj.SetValue(OverrideStatesProperty, value);

	
	/// <summary>
	/// Gets the overridden visual states of the control.
	/// </summary>
	/// <param name="obj"></param>
	public static string GetOverrideStates(Control obj) => (string)obj.GetValue(OverrideStatesProperty);

	#endregion

	private static void OnOverrideStatesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is Control control && e.NewValue is string { Length: >0 } value)
		{
			var states = value.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Where(x => !string.IsNullOrEmpty(x))
				.Select(x => x.Trim())
				.ToArray();

			var anySuccessed = false;
			foreach (var state in states)
			{
				var success = VisualStateManager.GoToState(control, state, useTransitions: control.IsLoaded);
				if (!success)
				{
					Logger.WarnIfEnabled(() => $"The control failed to transition to the '{state}' state.");
				}

				anySuccessed |= success;
			}

			if (states.Any() && !anySuccessed)
			{
				WarnForMisplacedVisualStateGroups(control);
			}
		}
	}

	[Conditional("DEBUG")]
	private static void WarnForMisplacedVisualStateGroups(Control control)
	{
		if (VisualStateManager.GetVisualStateGroups(control) is { Count: > 0 } &&
			control.GetChildren().FirstOrDefault() is Control templateRoot &&
			VisualStateManager.GetVisualStateGroups(templateRoot) is { Count: 0 })
		{
			Logger.WarnIfEnabled(() =>
				$"The visual state transition failed because the VisualStateGroups is set on the '{PrettyPrintFE(control)}' element itself. " +
				$"Consider moving it under its first child '{PrettyPrintFE(templateRoot)}'."
			);

			string PrettyPrintFE(FrameworkElement fe) => fe.GetType().Name + (string.IsNullOrEmpty(fe.Name) ? "" : ("#" + fe.Name));
		}
	}
}
