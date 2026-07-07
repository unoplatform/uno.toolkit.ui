using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload
{
	// Keep ignored in CI; run locally in Debug without a debugger attached.
	// Temporarily disabled: Sample app auto-exits and test fails.
	//Tracking with https://github.com/unoplatform/uno.toolkit.ui/issues/1480
	[Ignore]
	[TestClass]
	[RunsInSecondaryApp(ignoreIfNotSupported: true)]
	public class LoadingViewHrTest
	{
		[TestMethod]
		public async Task HR_keeps_Loaded_state_by_preserving_Source(CancellationToken ct)
		{
			await UIHelper.Load(new LoadingViewPage(), ct);

			var lv = UIHelper.GetChild<LoadingView>(name: "LV");
			var marker = UIHelper.GetChild<TextBlock>(name: "Marker");

			lv.Source = new TestLoadable(isExecuting: false);

			lv.Content = new Border();

			Assert.IsNotNull(lv.Source);
			Assert.IsFalse(lv.Source!.IsExecuting);
			Assert.AreEqual("Original marker", marker.Text);

			await using (await HotReloadHelper.UpdateSourceFile<LoadingViewPage>(originalText: "Original marker", replacementText: "Updated marker", ct))
			{
				await TestHelper.WaitFor(() =>
					UIHelper.GetChild<TextBlock>(name: "Marker").Text == "Updated marker", ct);
			}

			var lvAfter = UIHelper.GetChild<LoadingView>(name: "LV");

			Assert.IsNotNull(lvAfter.Source);
			Assert.IsFalse(lvAfter.Source.IsExecuting);

			Assert.IsNotNull(lvAfter.Content);
		}
	}

	internal sealed class TestLoadable(bool isExecuting = false) : ILoadable
	{
		public event EventHandler? IsExecutingChanged;

		bool _isExecuting = isExecuting;
		public bool IsExecuting
		{
			get => _isExecuting;
			set { if (_isExecuting != value) { _isExecuting = value; IsExecutingChanged?.Invoke(this, EventArgs.Empty); } }
		}
	}
}
