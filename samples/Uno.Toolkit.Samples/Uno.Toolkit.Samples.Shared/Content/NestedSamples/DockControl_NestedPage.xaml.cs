using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;
using Uno.Extensions;


#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.Samples.Content.NestedSamples;

public sealed partial class DockControl_NestedPage : Page
{
	private int _itemCounter = 0;
	public DockControl_NestedPage()
	{
		this.InitializeComponent();
	}

	private void AddDocument(object sender, RoutedEventArgs e)
	{
		_itemCounter++;

		SUT.AddItem(new DocumentItem
		{
			Header = $"Document {_itemCounter}.txt",
			Title = $"title: Document {_itemCounter}.txt", // should be unused
			Content = $"contnet: asdasd {_itemCounter}",
		});
	}

	private void AddTool(object sender, RoutedEventArgs e)
	{
		_itemCounter++;

		SUT.AddItem(new ToolItem
		{
			Header = $"Tool {_itemCounter}",
			Title = $"title: Tool Window {_itemCounter}",
			Content = $"content: Tool {_itemCounter}",
		});
	}
	private void RefreshTV(object sender, RoutedEventArgs e)
	{
		SUT.RefreshTabViews();
	}
	private void ResetPanes(object sender, RoutedEventArgs e)
	{
		SUT.ResetPanes();
	}
}
