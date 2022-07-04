namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Specifies constants that define text and icon colors for the status bar.
	/// </summary>
	public enum StatusBarForegroundTheme
	{
		/// <summary>Leave the foreground in the last set color, and dispose any event registered by the extension associated to the current page.</summary>
		None,

		/// <summary>The foreground will take a light/white color.</summary>
		Light,
		/// <summary>The foreground will take a dark/black color.</summary>
		Dark,

		/// <summary>The foreground will adjust in accordingly to the current theme: light/white in the dark mode, and dark/black in the light mode.</summary>
		Auto,
		/// <summary>The foreground will adjust in accordingly to the current theme: dark/black in the dark mode, and light/white in the light mode.</summary>
		AutoInverse
	}
}
