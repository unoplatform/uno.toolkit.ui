using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes <see cref="TabBarItem"/> to Microsoft UI Automation.
	/// </summary>
	public partial class TabBarItemAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TabBarItemAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="TabBarItem"/> instance to create the peer for.</param>
		public TabBarItemAutomationPeer(TabBarItem owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(TabBarItem);

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.TabItem;

		protected override string GetNameCore()
		{
			var name = base.GetNameCore();
			if (Owner is not TabBarItem item ||
				!string.IsNullOrEmpty(AutomationProperties.GetName(item)) ||
				AutomationProperties.GetLabeledBy(item) is not null ||
				item.GetContentTemplateRoot() is not { } contentTemplateRoot)
			{
				return name;
			}

			var contentName = GetContentName(contentTemplateRoot);
			return string.IsNullOrEmpty(contentName) ? name : contentName;
		}

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.SelectionItem ? this : base.GetPatternCore(patternInterface);

		/// <inheritdoc />
		public bool IsSelected => (Owner as TabBarItem)?.IsSelected ?? false;

		/// <inheritdoc />
		public IRawElementProviderSimple? SelectionContainer
		{
			get
			{
				if (Owner is not TabBarItem tbi ||
					tbi.FindFirstParent<TabBar>() is not { } tabBar ||
					FrameworkElementAutomationPeer.CreatePeerForElement(tabBar) is not { } peer)
				{
					return null;
				}

				return ProviderFromPeer(peer);
			}
		}

		/// <inheritdoc />
		public void Select()
		{
			if (Owner is not TabBarItem tbi)
			{
				return;
			}

			if (tbi.IsSelected)
			{
				return;
			}

			if (!tbi.IsSelectable)
			{
				throw new InvalidOperationException("The TabBarItem is not selectable.");
			}

			tbi.IsSelected = true;
		}

		/// <inheritdoc />
		public void AddToSelection()
		{
			if (Owner is not TabBarItem tbi || tbi.IsSelected)
			{
				return;
			}

			if (tbi.FindFirstParent<TabBar>() is { } tabBar &&
				tabBar.GetSelectedTabBarItem() is not null)
			{
				throw new InvalidOperationException("TabBar does not support multiple selection.");
			}

			Select();
		}

		/// <inheritdoc />
		public void RemoveFromSelection()
		{
			if (Owner is not TabBarItem { IsSelected: true } tbi)
			{
				return;
			}

			if (tbi.FindFirstParent<TabBar>()?.TryClearSelection(tbi) != true)
			{
				tbi.IsSelected = false;
			}
		}

		private static string GetContentName(UIElement element)
		{
			if (FrameworkElementAutomationPeer.CreatePeerForElement(element) is { } peer)
			{
				var name = peer.GetName();
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
			}

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
			{
				if (VisualTreeHelper.GetChild(element, i) is UIElement child)
				{
					var name = GetContentName(child);
					if (!string.IsNullOrEmpty(name))
					{
						return name;
					}
				}
			}

			return string.Empty;
		}
	}
}
